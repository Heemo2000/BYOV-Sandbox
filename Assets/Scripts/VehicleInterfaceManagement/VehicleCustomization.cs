using UnityEngine;
using Game.InteractManagement;
using Game.InputManagement;
using Game.UI;
using Game.DataPersistence;
using Game.DriveManagement;

namespace Game.VehicleInterfaceManagement
{
    public class VehicleCustomization : Interactable
    {
        private const string VEHICLE_DATA_FILE_NAME = "dummy_data.json";

        [SerializeField] private VehicleCustomizationUI customizationUI;
        private JsonDataService dataService;
        private VehicleDatas datas;

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
            
            if(!dataService.IsFileExists(VEHICLE_DATA_FILE_NAME))
            {
                datas = new VehicleDatas();
                datas.infos.Add(new VehicleData() { name = "Tuktuk" });
                dataService.SaveData<VehicleDatas>(VEHICLE_DATA_FILE_NAME, datas, true);
            }
            else
            {
                datas = dataService.LoadData<VehicleDatas>(VEHICLE_DATA_FILE_NAME, true);
            }
            
        }
    }
}
