using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Game.DriveManagement
{
    public class FourWheeler : MonoBehaviour
    {
        private const float MIN_STEERING_INPUT = 0.0001f;
        private const float MAX_STEERING_INPUT = 0.999f;

        private const float MIN_ACCELERATING_INPUT = 0.0001f;
        private const float MAX_ACCELERATING_INPUT = 0.999f;

        [Header("Wheel Settings: ")]
        [SerializeField] private WheelInfo wheelFL;
        [SerializeField] private WheelInfo wheelFR;
        [SerializeField] private WheelInfo wheelRL;
        [SerializeField] private WheelInfo wheelRR;

        [Space(10.0f)]

        [Header("Damping while touching ground settings: ")]
        [Range(0.0f, 10.0f)]
        [SerializeField] private float linearDamp = 5.0f;
        [Range(0.0f, 10.0f)]
        [SerializeField] private float angularDamp = 5.0f;
        [Range(0.05f, 1.0f)]
        [SerializeField] private float dampingTime = 0.1f;

        [Space(10.0f)]

        [Header("Anti Rollbar Settings: ")]
        [SerializeField] private bool antiRollbarEnabled = false;

        [Min(1.0f)]
        [SerializeField] private float stiffness = 100.0f;

        [Min(0.1f)]
        [SerializeField] private float rollAngle = 1.0f;

        [Min(1.5f)]
        [SerializeField] private float resetOrientationTime = 3.0f;
        [SerializeField] private float resetPositionOffsetY = 5.0f;

        [Header("Throttle Settings: ")]
        [Min(0.0f)]
        [SerializeField] private float throttleForce = 100.0f;

        [Min(1.0f)]
        [SerializeField] private float throttleTime = 5.0f;

        [Min(0.1f)]
        [SerializeField] private float throttleMinSpeedPercent = 0.4f;

        [Header("Other Settings: ")]
        [Min(250.0f)]
        [SerializeField] private float mass = 1000.0f;
        [SerializeField] private LayerMask detectGroundLayerMask;

        [Min(0.1f)]
        [SerializeField] private float springStrength = 1.0f;

        [Min(0.1f)]
        [SerializeField] private float damperStrength = 1.0f;

        [SerializeField] private bool interpolatedInput = true;
        [Min(0.001f)]
        [SerializeField] private float steeringInputTime = 1.0f;

        [Min(0.001f)]
        [SerializeField] private float accelerateInputTime = 1.0f;

        [SerializeField] private float gravity = 10.0f;

        [Tooltip("Speed at which visual of wheel rotate in their X axis.")]
        [Min(1.0f)]
        [SerializeField] private float maxFakeWheelSpeed = 50.0f;

        [SerializeField] private AnimationCurve fakeWheelSpeedCurve;
        [Tooltip("Max torque of car in N-m")]
        [Min(1.0f)]
        [SerializeField] private float maxTorque = 1000.0f;

        [SerializeField] private AnimationCurve powerCurve;

        [Tooltip("Top speed of a car in m/s")]
        [Min(1.0f)]
        [SerializeField] private float forwardSpeed = 50.0f;

        [Tooltip("reverse speed of a car in m/s")]
        [Min(0.5f)]
        [SerializeField] private float reverseSpeed = 10.0f;
        [Min(0.1f)]
        [SerializeField] private float turnRadius = 5.0f;


        private Rigidbody carRB;
        private Collider carCollider;
        private RaycastHit hit;

        private Vector2 input = Vector2.zero;
        private Vector2 currentInput = Vector2.zero;

        
        private bool throttlePressed = false;
        private bool shouldThrottle = false;
        private float currentThrottleTime = 0.0f;


        private int groundChecksCount = 0;
        private Coroutine resetOrientationCoroutine = null;

        private float tempVelocityX = 0.0f;
        private float tempVelocityY = 0.0f;

        private float wheelBase = 0.0f;
        private float rearTrack = 0.0f;

        private Coroutine dampingCoroutine = null;
        public Vector2 Input { get => input; set => input = value; }
        public bool IsGrounded { get => Vector3.Dot(transform.up, Vector3.up) > 0.0f; }
        public float ForwardSpeed { get => forwardSpeed; set => forwardSpeed = value; }
        public float ReverseSpeed { get => reverseSpeed; set => reverseSpeed = value; }
        public bool ThrottlePressed { get => throttlePressed; set => throttlePressed = value; }

        public float GetNormalLeftWheelAngle()
        {
            float leftWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2.0f)));
            return leftWheelAngle;
        }

        public float GetNormalRightWheelAngle()
        {
            float rightWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2.0f)));
            return rightWheelAngle;
        }
        public float GetCurrentSpeed()
        {
            return Vector3.Dot(transform.forward, carRB.linearVelocity);
        }

        public float GetAcceleratingDirection()
        {
            float currentSpeed = GetCurrentSpeed();

            if (currentSpeed == 0.0f)
            {
                return 0.0f;
            }

            return Mathf.Sign(currentSpeed);
        }

        public float GetNormalizedSpeed()
        {
            return Mathf.Clamp01(Mathf.Abs(GetCurrentSpeed()) /
                                                 ((currentInput.y >= 0.0f) ? forwardSpeed : reverseSpeed));
        }

        private void ApplyGravity()
        {
            carRB.AddForce(-Vector3.up * mass * gravity);
        }

        private void HandleForces(WheelInfo data)
        {
            bool didHitGround = Physics.Raycast(data.suspensionOrigin.position,
                                               -data.suspensionOrigin.up,
                                                out hit,
                                                data.radius,
                                                detectGroundLayerMask.value);

            //Debug.Log("Hitting ground: " + didHitGround);

            if (!didHitGround)
            {
                return;
            }

            groundChecksCount++;

            //Suspension Forces
            Vector3 springDir = data.suspensionOrigin.up;

            Vector3 tireWorldVelocity = carRB.GetPointVelocity(data.suspensionOrigin.position);

            float offset = data.suspensionRestDistance - hit.distance;

            float velocity = Vector3.Dot(springDir, tireWorldVelocity);

            float forceAmount = (offset * springStrength) - (velocity * damperStrength);

            Vector3 springForce = springDir * forceAmount;

            //Steering Forces
            Vector3 steeringDir = data.suspensionOrigin.right; //(input.x == 0.0f || !data.allowSteering) ? data.suspension.right :
                                                         //                      input.x * data.suspension.right;

            float steeringVelocity = Vector3.Dot(steeringDir, tireWorldVelocity);

            float desiredVelocityChange = -steeringVelocity * data.tireGripFactor;

            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

            Vector3 steeringForce = steeringDir * data.mass * desiredAcceleration;


            //Acceleration Forces
            Vector3 accelDir = data.suspensionOrigin.forward;

            float carSpeed = GetCurrentSpeed();

            //Debug.Log("Car Speed: " + carSpeed);

            float normalizedSpeed = GetNormalizedSpeed();

            //Debug.Log("Normalized speed: " + normalizedSpeed);

            float availaibleTorque = (currentInput.y != 0.0f) ? powerCurve.Evaluate(normalizedSpeed) * maxTorque * currentInput.y : 0.0f;
            //Debug.Log("Availaible torque: " + availaibleTorque);

            Vector3 accelerationForce = (normalizedSpeed < 1.0f) ? accelDir * availaibleTorque : Vector3.zero;

            //Debug.Log("Acceleration Force: " + accelerationForce);

            //carRB.AddForceAtPosition(springForce + steeringForce + accelerationForce, data.suspension.position);
            carRB.AddForceAtPosition(springForce, data.suspensionOrigin.position);
            carRB.AddForceAtPosition(steeringForce, data.suspensionOrigin.position);
            carRB.AddForceAtPosition(accelerationForce, data.suspensionOrigin.position);
        }

        private void HandleThrottle()
        {
            if(throttlePressed && !shouldThrottle && GetNormalizedSpeed() >= throttleMinSpeedPercent)
            {
                shouldThrottle = true;
            }

            if(shouldThrottle)
            {
                if(currentThrottleTime <= throttleTime)
                {
                    carRB.AddForce(transform.forward * throttleForce);
                    currentThrottleTime += Time.deltaTime;
                }
                else
                {
                    currentThrottleTime = 0.0f;
                    shouldThrottle = false;
                }
            }
        }

        private void HandleDampOnTouchingGround()
        {
            if (groundChecksCount == 0)
            {
                dampingCoroutine = null;
            }
            else if (groundChecksCount == 4 && dampingCoroutine == null)
            {
                dampingCoroutine = StartCoroutine(Damp());
            }
        }


        private void HandleSteering()
        {
            if (wheelFL != null && wheelFL.suspensionOrigin != null)
            {
                float leftWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + Mathf.Sign(input.x) * (rearTrack / 2.0f))) * currentInput.x;
                leftWheelAngle *= wheelFL.steerCurve.Evaluate(GetNormalizedSpeed());

                Vector3 leftWheelEulerAngles = wheelFL.suspensionOrigin.localEulerAngles;
                leftWheelEulerAngles.y = leftWheelAngle;

                wheelFL.suspensionOrigin.localEulerAngles = leftWheelEulerAngles;
            }

            if (wheelFR != null && wheelFR.suspensionOrigin != null)
            {
                float rightWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - Mathf.Sign(input.x) * (rearTrack / 2.0f))) * currentInput.x;
                rightWheelAngle *= wheelFR.steerCurve.Evaluate(GetNormalizedSpeed());

                Vector3 rightWheelEulerAngles = wheelFR.suspensionOrigin.localEulerAngles;
                rightWheelEulerAngles.y = rightWheelAngle;

                wheelFR.suspensionOrigin.localEulerAngles = rightWheelEulerAngles;
            }
        }

        private void HandleAntiRollbar()
        {
            if (!antiRollbarEnabled)
            {
                return;
            }


            RaycastHit leftWheelHit;
            RaycastHit rightWheelHit;


            float travelLeft = 1.0f;
            float travelRight = 1.0f;

            if (wheelRL != null && wheelRR != null && wheelRL.suspensionOrigin != null && wheelRR.suspensionOrigin != null)
            {

                bool wheelRLCheck = Physics.Raycast(wheelRL.suspensionOrigin.position,
                                                          -wheelRL.suspensionOrigin.up,
                                                           out leftWheelHit,
                                                           wheelRL.radius,
                                                           detectGroundLayerMask.value);

                bool wheelRRCheck = Physics.Raycast(wheelRR.suspensionOrigin.position,
                                                          -wheelRR.suspensionOrigin.up,
                                                           out rightWheelHit,
                                                           wheelRR.radius,
                                                           detectGroundLayerMask.value);


                travelLeft = 1.0f;
                travelRight = 1.0f;

                if (wheelRLCheck)
                {
                    Debug.DrawLine(wheelRL.suspensionOrigin.position,
                                   leftWheelHit.point, Color.white);

                    travelLeft = (wheelRL.suspensionOrigin.InverseTransformPoint(leftWheelHit.point).y - wheelRL.radius) / wheelRL.suspensionRestDistance;
                }

                if (wheelRRCheck)
                {
                    Debug.DrawLine(wheelRR.suspensionOrigin.position,
                                   rightWheelHit.point, Color.white);

                    travelRight = (wheelRR.suspensionOrigin.InverseTransformPoint(rightWheelHit.point).y - wheelRR.radius) / wheelRR.suspensionRestDistance;
                }


                float antiRollbarForceAmount = (travelLeft - travelRight) * stiffness * rollAngle;



                carRB.AddForceAtPosition(wheelRL.suspensionOrigin.up * -antiRollbarForceAmount,
                                          wheelRL.suspensionOrigin.position);

                carRB.AddForceAtPosition(wheelRR.suspensionOrigin.up * antiRollbarForceAmount,
                                          wheelRR.suspensionOrigin.position);
            }

        }

        private void HandleWheelGraphics(WheelInfo data)
        {
            if (data == null || data.suspensionOrigin == null)
            {
                return;
            }

            Quaternion wheelRotation = data.suspensionOrigin.localRotation;

            data.RotationX += GetAcceleratingDirection() *
                                  maxFakeWheelSpeed *
                                  fakeWheelSpeedCurve.Evaluate(GetNormalizedSpeed()) *
                                  Time.deltaTime;

            wheelRotation *= Quaternion.AngleAxis(data.RotationX,
                                                  Vector3.right);

            data.wheelGraphic.localRotation = wheelRotation;

            bool didHitGround = Physics.Raycast(data.suspensionOrigin.position,
                                               -data.suspensionOrigin.up,
                                                out hit,
                                                data.radius,
                                                detectGroundLayerMask.value);


            Vector3 wheelPosition = data.suspensionOrigin.position;
            if (didHitGround)
            {
                wheelPosition = hit.point + data.suspensionOrigin.up * data.radius;
            }

            data.wheelGraphic.position = wheelPosition;
        }

        private void HandleOrientation()
        {
            if (GetNormalizedSpeed() > 0.05f || IsGrounded)
            {
                return;
            }

            if (resetOrientationCoroutine == null)
            {
                resetOrientationCoroutine = StartCoroutine(ResetOrientation());
            }
        }

        private IEnumerator ResetOrientation()
        {
            yield return new WaitForSeconds(resetOrientationTime);

            carRB.isKinematic = true;

            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = 0.0f;
            eulerAngles.z = 0.0f;

            transform.position += Vector3.up * resetPositionOffsetY;
            transform.eulerAngles = eulerAngles;

            carRB.isKinematic = false;

            yield return new WaitUntil(() => !IsGrounded);
            resetOrientationCoroutine = null;
        }

        private void Setup()
        {
            carRB.isKinematic = false;
            carRB.mass = mass;
            carRB.useGravity = false;
            carRB.constraints = RigidbodyConstraints.None;
            carRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            carRB.interpolation = RigidbodyInterpolation.Interpolate;

            wheelBase = Vector3.Distance(wheelFL.suspensionOrigin.position, wheelFR.suspensionOrigin.position);
            rearTrack = Vector3.Distance(wheelFL.suspensionOrigin.position, wheelRL.suspensionOrigin.position);
        }

        private IEnumerator Damp()
        {
            carRB.linearDamping = linearDamp;
            carRB.angularDamping = angularDamp;
            yield return new WaitForSeconds(dampingTime);
            carRB.linearDamping = 0.0f;
            carRB.angularDamping = 0.05f;
        }

        private void Awake()
        {
            carRB = GetComponent<Rigidbody>();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Setup();
        }

        private void Update()
        {
            if (carRB.isKinematic)
            {
                return;
            }

            if (!interpolatedInput)
            {
                currentInput.x = input.x;
                currentInput.y = input.y;
                return;
            }


            currentInput.x = Mathf.SmoothDamp(currentInput.x, input.x, ref tempVelocityX, steeringInputTime);
            currentInput.y = Mathf.SmoothDamp(currentInput.y, input.y, ref tempVelocityY, accelerateInputTime);

            if ((currentInput.x > 0.0f && currentInput.x <= MIN_STEERING_INPUT) &&
                (currentInput.x < 0.0f && currentInput.x >= -MIN_STEERING_INPUT))
            {
                currentInput.x = 0.0f;
            }
            else if (currentInput.x > 0.0f && currentInput.x >= MAX_STEERING_INPUT)
            {
                currentInput.x = 1.0f;
            }
            else if (currentInput.x < 0.0f && currentInput.x <= -MAX_STEERING_INPUT)
            {
                currentInput.x = -1.0f;
            }

            if ((currentInput.y > 0.0f && currentInput.y <= MIN_ACCELERATING_INPUT) &&
                (currentInput.y < 0.0f && currentInput.y >= -MIN_ACCELERATING_INPUT))
            {
                currentInput.y = 0.0f;
            }
            else if (currentInput.y > 0.0f && currentInput.y >= MAX_ACCELERATING_INPUT)
            {
                currentInput.y = 1.0f;
            }
            else if (currentInput.y < 0.0f && currentInput.y <= -MAX_ACCELERATING_INPUT)
            {
                currentInput.y = -1.0f;
            }
        }
        private void FixedUpdate()
        {
            if (carRB.isKinematic)
            {
                return;
            }

            groundChecksCount = 0;


            HandleForces(wheelFL);
            HandleForces(wheelFR);
            HandleForces(wheelRL);
            HandleForces(wheelRR);

            HandleThrottle();

            ApplyGravity();

            HandleDampOnTouchingGround();
            HandleOrientation();

            HandleSteering();

            HandleWheelGraphics(wheelFL);
            HandleWheelGraphics(wheelFR);
            HandleWheelGraphics(wheelRL);
            HandleWheelGraphics(wheelRR);

            HandleAntiRollbar();
        }

        private void OnValidate()
        {
            if (wheelFL == null || wheelFL.suspensionOrigin == null || 
               wheelFR == null || wheelFR.suspensionOrigin == null ||
               wheelRL == null || wheelRL.suspensionOrigin == null)
            {
                return;
            }
            wheelBase = Vector3.Distance(wheelFL.suspensionOrigin.position, wheelFR.suspensionOrigin.position);
            rearTrack = Vector3.Distance(wheelFL.suspensionOrigin.position, wheelRL.suspensionOrigin.position);

            if (turnRadius < rearTrack / 2.0f)
            {
                turnRadius = rearTrack / 2.0f;
            }
        }

        private void DrawWheelDetails(WheelInfo data)
        {
            if (data == null)
            {
                return;
            }

#if UNITY_EDITOR

            if (data.suspensionOrigin != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(data.suspensionOrigin.position,
                                data.suspensionOrigin.position + Vector3.up * data.suspensionRestDistance);
            }

            if (data.wheelGraphic != null)
            {
                Handles.color = Color.green;
                Handles.DrawWireDisc(data.wheelGraphic.position, data.wheelGraphic.right, data.radius);
            }

#endif
        }
        private void OnDrawGizmosSelected()
        {
            DrawWheelDetails(wheelFL);
            DrawWheelDetails(wheelFR);
            DrawWheelDetails(wheelRL);
            DrawWheelDetails(wheelRR);

            if ((wheelFL != null && wheelFR != null && wheelRL != null) &&
                (wheelFL.suspensionOrigin != null && wheelFR.suspensionOrigin != null && wheelRL.suspensionOrigin != null))
            {
                Gizmos.color = Color.magenta;

                //For displaying wheel base
                Gizmos.DrawLine(wheelFL.suspensionOrigin.position, wheelRL.suspensionOrigin.position);

                //For displaying rear track
                Gizmos.DrawLine(wheelRL.suspensionOrigin.position, wheelRR.suspensionOrigin.position);
            }

            //For displaying turn radius.
            if (wheelRL != null && wheelRL.suspensionOrigin != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(wheelRL.suspensionOrigin.position,
                                wheelRL.suspensionOrigin.position + wheelRL.suspensionOrigin.right * turnRadius);
            }
        }
    }
}
