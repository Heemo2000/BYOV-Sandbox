using UnityEngine;
using Game.InteractManagement;
using Game.InputManagement;
using Game.UI;
using Game.DataPersistence;


namespace Game.VehicleInterfaceManagement
{
    public class VehicleCustomization : Interactable
    {
        [SerializeField] private VehicleCustomizationUI customizationUI;
        private JsonDataService dataService;


        public override void Activate()
        {
            customizationUI.Activate();
        }

        public override void Deactivate()
        {
            customizationUI.Deactivate();
        }

        public override InteractableType GetInteractableType()
        {
            return InteractableType.VehicleCustomizeInterface;
        }

        public override void HandleInput(InputStore store)
        {
            
        }

        private void Awake()
        {
            dataService = new JsonDataService();
        }
    }
}
