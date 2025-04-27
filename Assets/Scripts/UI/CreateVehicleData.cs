using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Game.UI
{
    [System.Serializable]
    public class CreateVehicleData
    {
        public Page panel;
        
        public Button leftVehicleTypeIndexBtn;
        public TMP_Text vehicleTypeField;
        public Button rightVehicleIndexBtn;
        public Button goBackToMainBtn;

        public Slider topSpeedSlider;
        public Slider accelerationSlider;
        public Slider turnRadiusSlider;
        public Slider suspensionStrengthSlider;
        public Slider damperStrengthSlider;
        public Slider tireTractionSlider;
    }
}
