using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class VehicleCustomizationUI : MonoBehaviour
    {
        [Header("Options Select Settings: ")]
        [SerializeField]
        private Page optionsSelectPanel;

        [Header("Select Vehicle Settings: ")]
        [SerializeField]
        private SelectVehicleData selectVehicleData;

        [Header("Create Vehicle Settings: ")]
        [SerializeField]
        private CreateVehicleData createVehicleData;

        [Header("UI Settings: ")]
        [SerializeField]
        private UIManager uiManager;

        public CreateVehicleData CreateVehicleData { get => createVehicleData; }

        public void Activate()
        {
            uiManager.PushPage(optionsSelectPanel);
        }

        public void Deactivate()
        {
            while(!uiManager.IsPageOnTopOfStack(optionsSelectPanel))
            {
                uiManager.PopPage(false);
            }

            uiManager.PopPage(false);
        }

        public void ShowSelectVehicleData(float topSpeedPercent,
                                          float accelerationPercent,
                                          float turnRadiusPercent,
                                          float suspensionStrengthPercent,
                                          float damperStrengthPercent,
                                          float tireTractionPercent)
        {
            selectVehicleData.topSpeedSlider.fillAmount = topSpeedPercent;
            selectVehicleData.accelerationSlider.fillAmount = accelerationPercent;
            selectVehicleData.turnRadiusSlider.fillAmount = turnRadiusPercent;
            selectVehicleData.suspensionStrengthSlider.fillAmount = suspensionStrengthPercent;
            selectVehicleData.damperStrengthSlider.fillAmount = damperStrengthPercent;
            selectVehicleData.tireTractionSlider.fillAmount = tireTractionPercent;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }
    }
}
