using UnityEngine;

namespace Game.InputManagement
{
    public struct InputStore
    {
        private float inputX;
        private float inputY;
        private float rotateX;
        private float rotateY;
        private bool interactPressed;
        private bool throttlePressed;

        public float InputX { get => inputX; set => inputX = value; }
        public float InputY { get => inputY; set => inputY = value; }
        public float RotateX { get => rotateX; set => rotateX = value; }
        public float RotateY { get => rotateY; set => rotateY = value; }
        public bool InteractPressed { get => interactPressed; set => interactPressed = value; }
        public bool ThrottlePressed { get => throttlePressed; set => throttlePressed = value; }
        
    }
}
