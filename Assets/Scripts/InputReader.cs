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
        if(context.phase == InputActionPhase.Performed)
            JumpInputEvent?.Invoke();
        else
            JumpInputCancelledEvent?.Invoke();
    }

    void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.PlayerControls.SetCallbacks(this);
        playerInput.PlayerControls.Enable();
    }
}
