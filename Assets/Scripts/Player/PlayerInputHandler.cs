using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private InputActions _inputActions;
    private InputAction _move;
    private InputAction _fire;
    private InputAction _secFire;
    private InputAction _jump;

    private PlayerInput currentInputs;

    private void OnEnable()
    {
        _inputActions = new InputActions();
        _move = _inputActions.Player.Move;
        _move.Enable();

        _fire = _inputActions.Player.Fire;
        _fire.Enable();

        _secFire = _inputActions.Player.SecondaryFire;
        _secFire.Enable();

        _jump = _inputActions.Player.Jump;
        _jump.Enable();
    }

    private void Update()
    {
        Vector2 movementInput = _move.ReadValue<Vector2>();
        float fireInput = _fire.ReadValue<float>();
        float secFireInput = _secFire.ReadValue<float>();
        bool jump = _jump.ReadValue<float>() > 0;

        currentInputs.Movement = movementInput;
        currentInputs.Fire = fireInput;
        currentInputs.SecondaryFire = secFireInput;
        currentInputs.Jump = jump;
    }

    public void OnDisable()
    {
        _move.Disable();
        _fire.Disable();
        _jump.Disable();
    }

    public PlayerInput GetPlayerInput()
    {
        return currentInputs;
    }
}

public struct PlayerInput
{
    public Vector2 Movement;
    public float Fire;
    public float SecondaryFire;
    public bool Jump;
}