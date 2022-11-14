using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu]
public class InputReader : ScriptableObject, PlayerInput.IPlayerControlsActions
{
    PlayerInput playerInput;

    public event Action<Vector2> OnMovement;
    public void OnMove(InputAction.CallbackContext context)
    {
        
        switch (context.phase) 
        {
            case InputActionPhase.Performed:
                OnMovement?.Invoke(context.ReadValue<Vector2>());
                break;
            case InputActionPhase.Canceled:
                OnMovement?.Invoke(Vector2.zero);
                break;

        }
    }



    void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.PlayerControls.SetCallbacks(this);
        playerInput.PlayerControls.Enable();
    }
}
