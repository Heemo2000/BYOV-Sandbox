using Game.InputManagement;
using UnityEngine;

namespace Game.InteractManagement
{
    public abstract class Interactable : MonoBehaviour
    {

        public abstract InteractableType GetInteractableType();
        public abstract void Activate();
        public abstract void HandleInput(InputStore store);
        public abstract void Deactivate();
    }
}
