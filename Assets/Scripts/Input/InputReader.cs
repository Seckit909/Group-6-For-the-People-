using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace P106.Main.Input
{
    [CreateAssetMenu(menuName = "P106/Input/Input Reader")]
    public sealed class InputReader : ScriptableObject, PlayerInput.IPlayerControlsActions
    {
        [Header("Mouse Drag Settings:")]
        [SerializeField, Min(0f)] float mouseDragSmoothSpeed = .1f;

        [SerializeField, Min(1f)] float maxDragSmoothSpeed = 1000f;
        [SerializeField, Range(1f, 3f)] float deltaTimeMultiplier = 2f;

        PlayerInput playerInput;
        static Camera mainCam;

        public static event Action<Vector2, float, float> MouseDragEvent;
        public event Action<Vector2> MoveInputEvent;

        public event Action JumpInputEvent;
        public event Action JumpInputCancelledEvent;
        public event Action<bool> DescendInputEvent;

        public static event Action<bool> LeftMouseInputEvent;
        public event Action<bool> RightMouseInputEvent;

        public static bool IsHoldingLeftMouseButton { get; private set; }


        void OnEnable()
        {
            playerInput ??= new PlayerInput();
            playerInput.PlayerControls.SetCallbacks(this);
            playerInput.PlayerControls.Enable();
            mainCam = Camera.main;
        }

        void OnDisable()
        {
            playerInput.PlayerControls.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    MoveInputEvent?.Invoke(context.ReadValue<Vector2>());
                    break;
                case InputActionPhase.Canceled:
                    MoveInputEvent?.Invoke(Vector2.zero);
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                JumpInputEvent?.Invoke();
            else
                JumpInputCancelledEvent?.Invoke();
        }

        public void OnLeftMouse(InputAction.CallbackContext context)
        {
            IsHoldingLeftMouseButton = context.performed;
            LeftMouseInputEvent?.Invoke(IsHoldingLeftMouseButton);
        }

        public void OnRightMouse(InputAction.CallbackContext context)
        {
            RightMouseInputEvent?.Invoke(context.performed);
        }

        public void OnMouseDrag(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            var delta = context.ReadValue<Vector2>();
            MouseDragEvent?.Invoke(delta, mouseDragSmoothSpeed, maxDragSmoothSpeed);
        }

        public void OnDescend(InputAction.CallbackContext context)
        {
            DescendInputEvent?.Invoke(context.performed);
        }

        // UI EVENTS
        public event Action<bool> DebugUIEvent;
        public event Action<bool> PauseMenuEvent;

        public void OnDebugUI(InputAction.CallbackContext context)
        {
            DebugUIEvent?.Invoke(context.performed);
        }

        public void OnPauseMenuUI(InputAction.CallbackContext context)
        {
            PauseMenuEvent?.Invoke(context.performed);
        }

        public static Vector2 GetMousePosition() => Mouse.current.position.ReadValue();
        public static Vector3 GetMousePosition3D() => new(GetMousePosition().x, GetMousePosition().y, 0f);
        public static Vector2 GetScreenToWorldMousePosition() => mainCam.ScreenToWorldPoint(GetMousePosition3D());

        public static int GetMouseKeyValue()
        {
            if (Mouse.current.leftButton.isPressed) return 0;
            if (Mouse.current.rightButton.isPressed) return 1;
            if (Mouse.current.middleButton.isPressed) return 2;
            return -1;
        }
    }
}