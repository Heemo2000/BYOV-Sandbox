using UnityEngine;
using Game.InteractManagement;
using Game.InputManagement;
using Game.CameraManagement;

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
        }

        public override InteractableType GetInteractableType()
        {
            return InteractableType.Vehicle;
        }

        public override void HandleInput(InputStore store)
        {
            //Handle four wheeler input here, acceleration, steering and throttle.
            wheeler.Input = new Vector2(store.InputX, store.InputY);
            followCamera.HandleInput(store.RotateX, store.RotateY, false);
        }

        private void Awake()
        {
            wheeler = GetComponent<FourWheeler>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
