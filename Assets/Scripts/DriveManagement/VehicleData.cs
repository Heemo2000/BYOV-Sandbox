
namespace Game.DriveManagement
{
    [System.Serializable]
    public class VehicleData
    {
        public string name = "Test";
        public VehicleType type = VehicleType.Car;
        public float topSpeed = 50.0f;
        public float acceleration = 1000.0f;
        public float turnRadius = 5.0f;
        public float suspensionStrength = 4000.0f;
        public float damperStrength = 2000.0f;
        public float tireTraction = 1.0f;
    }
}
