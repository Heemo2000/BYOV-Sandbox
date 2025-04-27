using Game.InputManagement;
using UnityEngine;

namespace Game.InteractManagement
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private Transform enterLocation;
        public Transform EnterLocation { get => enterLocation; }

        public abstract InteractableType GetInteractableType();
        public abstract void Activate();
        public abstract void HandleInput(InputStore store);
        public abstract void Deactivate();
    }
}
