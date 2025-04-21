using UnityEngine;
using Unity.Cinemachine;

namespace Game.CameraManagement
{
    public class ThirdPersonCamera : BaseCamera
    {
        [SerializeField] private Vector2 lookSpeed = Vector2.zero;
        [SerializeField] private Vector2 minLookInDegree = Vector2.zero;
        [SerializeField] private Vector2 maxLookInDegree = Vector2.zero;

        private CinemachineOrbitalFollow orbitalFollow;

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void HandleInput(float inputX, float inputY, bool activateMaxFOV)
        {
            float lookX = orbitalFollow.HorizontalAxis.Value + inputX * lookSpeed.x;
            float lookY = orbitalFollow.VerticalAxis.Value + inputY * lookSpeed.y;

            lookX = Mathf.Clamp(lookX, minLookInDegree.x, maxLookInDegree.x);
            lookY = Mathf.Clamp(lookY, minLookInDegree.y, maxLookInDegree.y);

            orbitalFollow.HorizontalAxis.Value = lookX;
        }

        protected override void Awake()
        {
            base.Awake();
            orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        }

        private void Start()
        {
            orbitalFollow.HorizontalAxis.Range.Set(minLookInDegree.x, maxLookInDegree.x);
            orbitalFollow.VerticalAxis.Range.Set(minLookInDegree.y, maxLookInDegree.y);

        }
    }
}
