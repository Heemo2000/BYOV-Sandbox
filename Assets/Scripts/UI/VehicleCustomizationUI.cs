using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class VehicleCustomizationUI : MonoBehaviour
    {
        [Header("Create Vehicle Settings: ")]
        [SerializeField]
        private CreateVehicleData createVehicleData;

        [Header("UI Settings: ")]
        [SerializeField]
        private UIManager uiManager;

     
        public CreateVehicleData CreateVehicleData { get => createVehicleData; }
        
        public void Activate()
        {
            uiManager.PushPage(createVehicleData.panel);
        }

        public void Deactivate()
        {
            uiManager.PopPage(false);
        }
    }
}
