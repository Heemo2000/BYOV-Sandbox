using UnityEngine;
using Unity.Cinemachine;
using Game.InversionOfControlManagement;

namespace Game.CameraManagement
{
    public abstract class BaseCamera : MonoBehaviour
    {
        protected CinemachineCamera virtualCamera;

        public void SetPriority(int priority)
        {
            virtualCamera.Priority.Value = priority;
        }

        public virtual void Activate()
        {
            CameraManager cameraManager = null;
            ServiceLocator.ForSceneOf(this).Get(out cameraManager);
            if(cameraManager != null)
            {
                cameraManager.MakeCameraImportant(this);
            }
        }

        public virtual void Deactivate()
        {
            CameraManager cameraManager = null;
            ServiceLocator.For(this).Get(out cameraManager);

            if (cameraManager != null)
            {
                cameraManager.MakeInitialCameraImportant();
            }
        }

        public abstract void HandleInput(float inputX, float inputY, bool activateMaxFOV);
        
        protected virtual void Awake()
        {
            virtualCamera = GetComponent<CinemachineCamera>();
        }
    }
}
