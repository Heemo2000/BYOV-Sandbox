using UnityEngine;
using Unity.Cinemachine;

namespace Game.CameraManagement
{
    public class FirstPersonCamera : BaseCamera
    {
        [SerializeField] private Vector2 traverseSpeed = Vector2.zero;
        [SerializeField] private Vector2 lookInDegreeMin = Vector2.zero;
        [SerializeField] private Vector2 lookInDegreeMax = Vector2.zero;
        [SerializeField] private Vector2 pickupTraverseSpeedTime = Vector2.zero;
        
        [Min(45.0f)]
        [SerializeField] private float normalFOV = 45.0f;
        
        [Min(1.0f)]
        [SerializeField] private float maxFOVMultiplier = 1.25f;
        
        [Min(0.1f)]
        [SerializeField] private float pickupChangeFOVTime = 0.5f;
        
        private Vector2 targetLookInDegree = Vector2.zero;
        private Vector2 lookInDegree = Vector2.zero;
        private Vector2 tempLookInDegree = Vector2.zero;
        private float currentFOV = 0.0f;
        private float targetFOV = 0.0f;
        private float tempFOV = 0.0f;

        public override void Activate()
        {
            //Logic here to activate the camera using some manager.
            base.Activate();
        }

        public override void Deactivate()
        {
            //Logic here to deactivate the camera using some manager.
            base.Deactivate();
        }

        public override void HandleInput(float inputX, float inputY, bool activateMaxFOV)
        {
            targetLookInDegree.x = Mathf.Clamp(targetLookInDegree.x - inputY * traverseSpeed.y,
                                               lookInDegreeMin.x,
                                               lookInDegreeMax.x);

            targetLookInDegree.y = Mathf.Clamp(targetLookInDegree.y + inputX * traverseSpeed.x,
                                               lookInDegreeMin.y,
                                               lookInDegreeMax.y);

            lookInDegree.x = Mathf.SmoothDamp(lookInDegree.x,
                                              targetLookInDegree.x,
                                              ref tempLookInDegree.x,
                                              pickupTraverseSpeedTime.x);

            lookInDegree.y = Mathf.SmoothDamp(lookInDegree.y,
                                              targetLookInDegree.y,
                                              ref tempLookInDegree.y,
                                              pickupTraverseSpeedTime.y);

            virtualCamera.transform.localRotation = Quaternion.Euler(lookInDegree.x,
                                                                lookInDegree.y, 
                                                                0.0f);


            targetFOV = (activateMaxFOV) ? normalFOV * maxFOVMultiplier : normalFOV;
            currentFOV = Mathf.SmoothDamp(currentFOV, targetFOV, ref tempFOV, pickupChangeFOVTime);
            virtualCamera.Lens.FieldOfView = currentFOV;
        }
    }
}
