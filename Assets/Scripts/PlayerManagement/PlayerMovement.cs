using UnityEngine;
using Game.InteractManagement;
using Game.InputManagement;
using Game.CameraManagement;
using UnityEngine.Events;

namespace Game.PlayerManagement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : Interactable
    {
        [Header("Ground and Gravity Settings: ")]
        [Min(1.0f)]
        [SerializeField] private float gravity = 10.0f;

        [Header("Movement Settings: ")]
        [Min(1.0f)]
        [SerializeField] private float moveSpeed = 10.0f;

        [Min(0.0f)]
        [SerializeField] private float pickupMoveSpeedTime = 0.2f;

        [Min(0.1f)]
        [SerializeField] private float rotateSpeed = 50.0f;

        [Min(0.0f)]
        [SerializeField] private float pickupRotateSpeedTime = 0.2f;

        [Header("Camera Settings: ")]
        [SerializeField] private FirstPersonCamera firstPersonCamera;

        [Header("Activation And Deactivation Settings: ")]
        public UnityEvent OnMovementActivate;

        public UnityEvent OnMovementDeactivate;

        private CharacterController controller;
        
        private float velocityY = 0.0f;
        private float currentRotationX = 0.0f;
        private float tempRotationX = 0.0f;
        
        private Vector2 currentInput = Vector2.zero;
        private float tempInputX = 0.0f;
        private float tempInputY = 0.0f;

        public override InteractableType GetInteractableType()
        {
            return InteractableType.Human;
        }

        public override void Activate()
        {
            controller.enabled = true;
            firstPersonCamera.Activate();
            OnMovementActivate?.Invoke();
        }

        public override void HandleInput(InputStore store)
        {
            HandleGravity();
            HandleMove(store.InputX, store.InputY, store.RotateX);
            firstPersonCamera.HandleInput(0.0f, store.RotateY, store.InputY != 0.0f);
        }

        private void HandleGravity()
        {
            if (controller.isGrounded)
            {
                velocityY = 0.0f;
            }
            else
            {
                float oldVelocityY = velocityY;
                float newVelocityY = velocityY - gravity * Time.fixedDeltaTime;
                velocityY = (oldVelocityY + newVelocityY) * 0.5f;
            }
        }

        public override void Deactivate()
        {
            controller.enabled = false;
            firstPersonCamera.Deactivate();
            OnMovementDeactivate?.Invoke();
        }

        private void HandleMove(float inputX, float inputY, float rotateX)
        {
            currentInput.x = Mathf.SmoothDamp(currentInput.x, inputX, ref tempInputX, pickupMoveSpeedTime);
            currentInput.y = Mathf.SmoothDamp(currentInput.y, inputY, ref tempInputY, pickupMoveSpeedTime);

            if(controller.enabled)
            {
                Vector3 movementXZ = (transform.right * inputX + transform.forward * inputY).normalized * moveSpeed;
                Vector3 movememntY = Vector3.up * velocityY;

                controller.Move((movementXZ + movememntY) * Time.fixedDeltaTime);
            }

            currentRotationX = Mathf.SmoothDamp(currentRotationX, currentRotationX + rotateX * rotateSpeed * Time.fixedDeltaTime,
                                                ref tempRotationX, pickupRotateSpeedTime);

            controller.transform.rotation = Quaternion.AngleAxis(currentRotationX, Vector3.up);
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }
    }
}
