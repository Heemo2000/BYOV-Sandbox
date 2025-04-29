using UnityEngine;
using Game.InteractManagement;
using Game.InputManagement;
using Game.CameraManagement;
using Game.InversionOfControlManagement;

namespace Game.DriveManagement
{
    public class FourWheelerController : Interactable
    {
        [SerializeField] private BaseCamera followCamera;

        private FourWheeler wheeler;
        public override void Activate()
        {
            //Activate the camera and make four wheeler move.
            followCamera.Activate();
        }

        public override void Deactivate()
        {
            //Deactive the camera, switch the player's camera and make four wheeler stop.
            followCamera.Deactivate();
            wheeler.Input = Vector2.zero;
            wheeler.ThrottlePressed = false;
        }

        public override InteractableType GetInteractableType()
        {
            return InteractableType.Vehicle;
        }

        public override void HandleInput(InputStore store)
        {
            //Handle four wheeler input here, acceleration, steering and throttle.
            wheeler.Input = new Vector2(store.InputX, store.InputY);
            wheeler.ThrottlePressed = store.ThrottlePressed;
            followCamera.HandleInput(store.RotateX, store.RotateY, false);
        }

        private void Awake()
        {
            wheeler = GetComponent<FourWheeler>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.ForSceneOf(this).Get(out CameraManager manager);
            if(manager != null)
            {
                manager.RegisterCamera(followCamera);
            }
        }
    }
}
