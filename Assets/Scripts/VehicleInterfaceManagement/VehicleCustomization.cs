using System.Collections.Generic;
using UnityEngine;
using Game.InteractManagement;
using Game.InputManagement;
using Game.UI;
using Game.DataPersistence;
using Game.DriveManagement;
using Game.CameraManagement;
using Game.InversionOfControlManagement;


namespace Game.VehicleInterfaceManagement
{
    public class VehicleCustomization : Interactable
    {
        private const string VEHICLE_DATA_FILE_NAME = "dummy_data.json";

        [Header("Top Speed Settings: ")]
        [Min(20.0f)]
        [SerializeField] private float minTopSpeed = 50.0f;
        [Min(80.0f)]
        [SerializeField] private float maxTopSpeed = 200.0f;

        [Header("Acceleration Settings: ")]
        [Min(1000.0f)]
        [SerializeField] private float minAcceleration = 1000.0f;
        [Min(2000.0f)]
        [SerializeField] private float maxAcceleration = 4000.0f;

        [Header("Turn Radius Settings: ")]
        [Min(5.0f)]
        [SerializeField] private float minTurnRadius = 5.0f;
        [Min(10.0f)]
        [SerializeField] private float maxTurnRadius = 20.0f;

        [Header("Suspension Strength Settings: ")]
        [Min(4000.0f)]
        [SerializeField] private float minSpringStrength = 4000.0f;
        [Min(20000.0f)]
        [SerializeField] private float maxSpringStrength = 20000.0f;

        [Header("Damper Strength Settings: ")]
        [Min(2000.0f)]
        [SerializeField] private float minDamperStrength = 2000.0f;
        [Min(10000.0f)]
        [SerializeField] private float maxDamperStrength = 10000.0f;

        [Header("Tire Traction Settings: ")]
        [Min(0.1f)]
        [SerializeField] private float minTireTraction = 0.1f;
        [Min(1.0f)]
        [SerializeField] private float maxTireTraction = 1.0f;


        [Header("Other Settings: ")]
        [SerializeField] private VehicleCustomizationUI customizationUI;
        [SerializeField] private SpawnVehicleData[] spawnVehicleDatas;
        [SerializeField] private ShowVehicleData[] showVehicleDatas;
        [SerializeField] private Transform spawnVehicleOrigin;
        [SerializeField] private FirstPersonCamera firstPersonCamera;

        //private JsonDataService dataService;
        //private VehicleDatas datas;
        private int currentCreateVehicleTypeIndex = -1;

        //private List<FourWheeler> selectableVehicles;
        public override void Activate()
        {
            ServiceLocator.ForSceneOf(this).Get(out CameraManager manager);
            if (manager != null)
            {
                manager.MakeCameraImportant(firstPersonCamera);
            }
            customizationUI.Activate();
            var type = showVehicleDatas[currentCreateVehicleTypeIndex].type;
            customizationUI.CreateVehicleData.vehicleTypeField.text = type.ToString();
            ShowVehicle(showVehicleDatas[currentCreateVehicleTypeIndex].type);
            
        }

        public override void Deactivate()
        {
            ServiceLocator.ForSceneOf(this).Get(out CameraManager manager);
            if (manager != null)
            {
                manager.MakeInitialCameraImportant();
            }
            customizationUI.Deactivate();
        }

        public override InteractableType GetInteractableType()
        {
            return InteractableType.VehicleCustomizeInterface;
        }

        public override void HandleInput(InputStore store)
        {
            
        }

        private void Setup()
        {
            

            currentCreateVehicleTypeIndex = 0;

            //For create UI
            customizationUI.CreateVehicleData.leftVehicleTypeIndexBtn.onClick.AddListener(SelectLeftVehicleType);
            customizationUI.CreateVehicleData.rightVehicleIndexBtn.onClick.AddListener(SelectRightVehicleType);
            customizationUI.CreateVehicleData.createBtn.onClick.AddListener(CreateVehicle);

            customizationUI.CreateVehicleData.topSpeedSlider.minValue = minTopSpeed;
            customizationUI.CreateVehicleData.topSpeedSlider.maxValue = maxTopSpeed;

            customizationUI.CreateVehicleData.accelerationSlider.minValue = minAcceleration;
            customizationUI.CreateVehicleData.accelerationSlider.maxValue = maxAcceleration;

            customizationUI.CreateVehicleData.turnRadiusSlider.minValue = minTurnRadius;
            customizationUI.CreateVehicleData.turnRadiusSlider.maxValue = maxTurnRadius;

            customizationUI.CreateVehicleData.springStrengthSlider.minValue = minSpringStrength;
            customizationUI.CreateVehicleData.springStrengthSlider.maxValue = maxSpringStrength;

            customizationUI.CreateVehicleData.damperStrengthSlider.minValue = minDamperStrength;
            customizationUI.CreateVehicleData.damperStrengthSlider.maxValue = maxDamperStrength;

            customizationUI.CreateVehicleData.tireTractionSlider.minValue = minTireTraction;
            customizationUI.CreateVehicleData.tireTractionSlider.maxValue = maxTireTraction;

            //For camera stuff
            ServiceLocator.ForSceneOf(this).Get(out CameraManager manager);
            if (manager != null)
            {
                manager.RegisterCamera(firstPersonCamera);
            }
        }

        private void ShowVehicle(VehicleType type)
        {
            foreach(var data in showVehicleDatas)
            {
                if(data.type == type)
                {
                    data.mesh.gameObject.SetActive(true);
                }
                else
                {
                    data.mesh.gameObject.SetActive(false);
                }
            }
        }

        private void SelectLeftVehicleType()
        {
            currentCreateVehicleTypeIndex--;
            if(currentCreateVehicleTypeIndex < 0)
            {
                currentCreateVehicleTypeIndex = spawnVehicleDatas.Length - 1;
            }

            var type = showVehicleDatas[currentCreateVehicleTypeIndex].type;
            customizationUI.CreateVehicleData.vehicleTypeField.text = type.ToString();
            ShowVehicle(type);
        }

        private void SelectRightVehicleType()
        {
            currentCreateVehicleTypeIndex = (currentCreateVehicleTypeIndex + 1) % spawnVehicleDatas.Length;
            var type = showVehicleDatas[currentCreateVehicleTypeIndex].type;
            customizationUI.CreateVehicleData.vehicleTypeField.text = type.ToString();
            ShowVehicle(showVehicleDatas[currentCreateVehicleTypeIndex].type);
        }

        private void CreateVehicle()
        {
            var vehicle = Instantiate(spawnVehicleDatas[currentCreateVehicleTypeIndex].prefab, 
                                      spawnVehicleOrigin.position, 
                                      spawnVehicleOrigin.rotation);


            vehicle.SetTopSpeed(customizationUI.CreateVehicleData.topSpeedSlider.value);
            vehicle.MaxTorque = customizationUI.CreateVehicleData.accelerationSlider.value;
            vehicle.TurnRadius = customizationUI.CreateVehicleData.turnRadiusSlider.value;
            vehicle.SpringStrength = customizationUI.CreateVehicleData.springStrengthSlider.value;
            vehicle.DamperStrength = customizationUI.CreateVehicleData.damperStrengthSlider.value;
            vehicle.SetTireTraction(customizationUI.CreateVehicleData.tireTractionSlider.value);
        }


        private void Awake()
        {
            //dataService = new JsonDataService();
            //selectableVehicles = new List<FourWheeler>();

            /*if(!dataService.IsFileExists(VEHICLE_DATA_FILE_NAME))
            {
                datas = new VehicleDatas();
                datas.infos.Add(new VehicleData() {type = VehicleType.Car });
                dataService.SaveData<VehicleDatas>(VEHICLE_DATA_FILE_NAME, datas, true);
            }
            else
            {
                datas = dataService.LoadData<VehicleDatas>(VEHICLE_DATA_FILE_NAME, true);
            }*/
            
        }

        private void Start()
        {
            Setup();
        }
    }
}
