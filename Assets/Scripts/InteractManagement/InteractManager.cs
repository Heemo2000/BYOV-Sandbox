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
        private RaycastHit[] hits;

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
                    currentInteractable.transform.parent = interactable.EnterLocation.transform;
                    currentInteractable.transform.localPosition = Vector3.zero;
                    
                    interactable.enabled = true;
                    interactable.Activate();
                    currentInteractable = interactable;
                }
                else if(currentID != playerID)
                {
                    currentInteractable.Deactivate();
                    playerMovement.transform.parent = null;
                    playerMovement.enabled = true;
                    playerMovement.Activate();

                    currentInteractable = playerMovement;
                }
            }

            currentInteractable.HandleInput(store);
        }

        private Vector3 GetActualLookOffset()
        {
            if(lookCamera == null)
            {
                return Vector3.zero;
            }

            return lookCamera.transform.right * checkOffset.x +
                   lookCamera.transform.up * checkOffset.y +
                   lookCamera.transform.forward * checkOffset.z;
        }

        private bool IsInteractableFound(out Interactable interactable)
        {
            Vector3 checkPosition = lookCamera.transform.position + GetActualLookOffset();

            int count = Physics.SphereCastNonAlloc(checkPosition,
                                  checkRadius,
                                  lookCamera.transform.forward,
                                  hits,
                                  checkDistance,
                                  checkLayerMask.value);

            Debug.Log("Hits Count: " + count);

            if (count > 0)
            {
                Debug.Log("Hits 0: " + hits[0].transform.name);
                if (hits[0].transform.TryGetComponent<Interactable>(out interactable))
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
            hits = new RaycastHit[1];
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
            Vector3 origin = lookCamera.transform.position + GetActualLookOffset();

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, checkRadius);

            Vector3 end = origin + lookCamera.transform.forward * checkDistance;

            Gizmos.DrawLine(origin, end);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(end, checkRadius);
        }
    }
}
