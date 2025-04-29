using UnityEngine;
using Game.InteractManagement;
using Game.InputManagement;
using UnityEngine.InputSystem;
using System;
using System.Collections;

namespace Game.PlayerManagement
{
    public class Player : MonoBehaviour
    {
        private InteractManager interactManager;
        private InputStore inputStore;
        private GameControls gameControls;
        private Vector2 moveInput;
        private Vector2 rotateInput;
        private Coroutine interactCoroutine;
        private Coroutine throttleCoroutine;

        private void OnInteractPressed(InputAction.CallbackContext context)
        {
            if(interactCoroutine == null)
            {
                interactCoroutine = StartCoroutine(MakeInteractEnabled());
            }
        }

        private void OnThrottleStarted(InputAction.CallbackContext context)
        {
            if(throttleCoroutine == null)
            {
                throttleCoroutine = StartCoroutine(MakeThrottleEnabled());
            }
        }

        private IEnumerator MakeInteractEnabled()
        {
            inputStore.InteractPressed = true;
            yield return new WaitForSeconds(0.005f);
            inputStore.InteractPressed = false;

            interactCoroutine = null;
        }

        private IEnumerator MakeThrottleEnabled()
        {
            inputStore.ThrottlePressed = true;
            yield return new WaitForSeconds(0.01f);
            inputStore.ThrottlePressed = false;

            throttleCoroutine = null;
        }

        private void Awake()
        {
            inputStore = new InputStore();
            gameControls = new GameControls();
            interactManager = GetComponent<InteractManager>();
        }

        void Start()
        {
            gameControls.Enable();
            gameControls.MainActionMap.Interact.started += OnInteractPressed;
            gameControls.MainActionMap.Throttle.started += OnThrottleStarted;
        }

        

        private void Update()
        {
            moveInput = gameControls.MainActionMap.Movement.ReadValue<Vector2>();
            rotateInput = gameControls.MainActionMap.LookAround.ReadValue<Vector2>();

            inputStore.InputX = moveInput.x;
            inputStore.InputY = moveInput.y;
            inputStore.RotateX = rotateInput.x;
            inputStore.RotateY = rotateInput.y;
        }
        private void FixedUpdate()
        {
            interactManager.HandleInput(inputStore);
        }

        private void OnDestroy()
        {
            gameControls.Disable();
            gameControls.MainActionMap.Interact.started -= OnInteractPressed;
            gameControls.MainActionMap.Throttle.started -= OnThrottleStarted;
        }
    }
}
