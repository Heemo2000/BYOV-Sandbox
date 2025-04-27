using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [System.Serializable]
    public class SelectVehicleData
    {
        public Page panel;
        public Button clickButton;
        public Button leftCarIndexBtn;
        public Button rightCarIndexBtn;
        public Button goBackToMainBtn;

        public Image topSpeedSlider;
        public Image accelerationSlider;
        public Image turnRadiusSlider;
        public Image suspensionStrengthSlider;
        public Image damperStrengthSlider;
        public Image tireTractionSlider;
    }
}
