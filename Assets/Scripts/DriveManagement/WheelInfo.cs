using UnityEngine;

namespace Game.DriveManagement
{
    [System.Serializable]
    public class WheelInfo
    {
        public Transform suspensionOrigin;
        public Transform wheelGraphic;

        [Min(0.1f)]
        public float suspensionRestDistance = 0.1f;
        
        [Min(0.1f)]
        public float radius = 0.5f;
        
        [Range(0.0f, 1.0f)]
        public float tireGripFactor = 1.0f;
        
        public AnimationCurve steerCurve;
        
        [Min(0.1f)]
        public float mass = 10.0f;

        private float rotationX = 0.0f;

        public float RotationX { get => rotationX; set => rotationX = value; }
    }
}
