using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu]
public class InputReader : ScriptableObject, PlayerInput.IPlayerControlsActions
{
    PlayerInput playerInput;

    public event Action<Vector2> MoveInputEvent;
    
    public event Action JumpInputEvent;
    public event Action JumpInputCancelledEvent;
    public event Action<bool> DescendInputEvent;

    public event Action<bool> LeftMouseInputEvent;
    public event Action<bool> RightMouseInputEvent;
    
    void OnEnable()
    {
        playerInput ??= new PlayerInput();
        playerInput.PlayerControls.SetCallbacks(this);
        playerInput.PlayerControls.Enable();
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
        if(context.performed)
            JumpInputEvent?.Invoke();
        else
            JumpInputCancelledEvent?.Invoke();
    }

    public void OnLeftMouse(InputAction.CallbackContext context)
    {
        LeftMouseInputEvent?.Invoke(context.performed);
    }

    public void OnRightMouse(InputAction.CallbackContext context)
    {
        RightMouseInputEvent?.Invoke(context.performed);
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
}
