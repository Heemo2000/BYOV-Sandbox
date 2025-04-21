using UnityEngine;
using Game.InputManagement;
using Game.PlayerManagement;
using Game.CameraManagement;

namespace Game.InteractManagement
{
    public class InteractManager : MonoBehaviour
    {
        [SerializeField] private FirstPersonCamera lookCamera;
        [Min(0.01f)]
        [SerializeField] private float checkRadius = 0.05f;
        
        [Min(0.1f)]
        [SerializeField] private float checkDistance = 4.0f;
        
        [SerializeField] private Vector3 checkOffset = Vector3.zero;
        
        [SerializeField] private LayerMask checkLayerMask;
        
        private Interactable currentInteractable;
        private Interactable playerMovement;

        public void HandleInput(InputStore store)
        {
            if(store.InteractPressed)
            {
                int currentID = currentInteractable.GetInstanceID();
                int playerID = playerMovement.GetInstanceID();
                if (currentID == playerID && 
                   IsInteractableFound(out Interactable interactable))
                {
                    currentInteractable.Deactivate();
                    currentInteractable.enabled = false;

                    interactable.enabled = true;
                    interactable.Activate();
                    currentInteractable = interactable;
                }
                else if(currentID != playerID)
                {
                    currentInteractable.Deactivate();
                    playerMovement.enabled = true;
                    playerMovement.Activate();

                    currentInteractable = playerMovement;
                }
            }

            currentInteractable.HandleInput(store);
        }

        private bool IsInteractableFound(out Interactable interactable)
        {
            Vector3 checkPosition = lookCamera.transform.position + checkOffset;

            if(Physics.SphereCast(checkPosition, 
                                  checkRadius, 
                                  lookCamera.transform.forward, 
                                  out RaycastHit hit, 
                                  checkDistance, 
                                  checkLayerMask.value))
            {
                if(hit.transform.TryGetComponent<Interactable>(out interactable))
                {
                    return true;
                }
            }

            interactable = null;
            return false;
        }

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            currentInteractable = playerMovement;
            currentInteractable.enabled = true;
            currentInteractable.Activate();
        }

        private void OnDrawGizmosSelected()
        {
            if(lookCamera == null)
            {
                return;
            }
            Vector3 origin = lookCamera.transform.position + checkOffset;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, checkRadius);

            Vector3 end = origin + lookCamera.transform.forward * checkDistance;

            Gizmos.DrawLine(origin, end);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(end, checkRadius);
        }
    }
}
