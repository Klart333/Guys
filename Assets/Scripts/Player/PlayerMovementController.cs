using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour 
{
    public static readonly Vector3 Gravity = new Vector3(0, -0.981f, 0);

    public event Action OnJump = delegate { }; 

    [Header("Movement")]
    [SerializeField]
    private float _speed = 1;

    [SerializeField]
    private float _turnSpeed = 1;

    [Header("Jump")]
    [SerializeField]
    private float _jumpForce = 10;

    [SerializeField]
    private float _jumpForwardForce = 1;

    [SerializeField]
    private float _floatiness = 1;

    [Header("Grounding")]
    [SerializeField]
    private Transform[] _groundCheckPositions;

    [SerializeField]
    private float _groundDistanceThreshold;

    [SerializeField]
    private LayerMask _layerMask;

    private PlayerInputHandler _playerInputHandler;
    private CharacterController _characterController;
    private Player _player;

    private Vector3 _combinedMovementVector;
    private PlayerInput currentInput;

    private float _airTimer = 0;
    private bool _lastJumpInput = false;
    private bool _jumping = false;

    public bool MaxMovementReached { get; set; }

    public float Speed
    {
        get
        {
            if (!AllowedMovement())
            {
                return 0;
            }

            return MathF.Abs(currentInput.Movement.y);
        }
    }

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerInputHandler = FindObjectOfType<PlayerInputHandler>();
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        currentInput = _playerInputHandler.GetPlayerInput();

        _combinedMovementVector = new Vector3();

        if (AllowedMovement())
        {
            _combinedMovementVector += HandleMovement();
        }

        if (_player.IsActive)
        {
            UpdateRotation(currentInput.Movement.x);
        }

        if (Grounded())
        {
            if (AllowedMovement())
            {
                HandleJump();
            }

            _airTimer = 0;
        }
        else if (!_jumping)
        {
            _airTimer += Time.deltaTime * (1.0f / _floatiness);
            _combinedMovementVector += HandleGravity();
        }

        _characterController.Move(_combinedMovementVector);

        _lastJumpInput = currentInput.Jump;
    }

    private bool AllowedMovement()
    {
        return _player.IsActive && !MaxMovementReached; 
    }

    private void HandleJump()
    {
        if (currentInput.Jump && !_lastJumpInput)
        {
            OnJump();
            Jumping();
        }
    }

    private async void Jumping()
    {
        _jumping = true;
        float t = 0;
        Vector3 jumpForce = Vector3.up * _jumpForce + _characterController.transform.forward * _jumpForwardForce;

        while (t <= 1f)
        {
            t += Time.deltaTime * (1.0f / _floatiness);

            Vector3 jumpVector = jumpForce * (1 - t) * Time.deltaTime;
            _characterController.Move(jumpVector);

            await Task.Yield();
        }
        _jumping = false;
    }

    private Vector3 HandleGravity()
    {
        Vector3 gravityForce = Gravity * Time.deltaTime * _airTimer;

        return gravityForce;
    }

    private Vector3 HandleMovement()
    {
        if (currentInput.Movement.magnitude < 0.1f)
        {
            return new Vector3();
        }

        Vector3 movement = currentInput.Movement.y * transform.forward;
        movement *= _speed * Time.deltaTime;

        return movement;
    }

    private void UpdateRotation(float turn)
    {
        Vector3 forward = Quaternion.Euler(0, turn * _turnSpeed, 0) * _characterController.transform.forward;

        Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);
        Quaternion targetRotation = Quaternion.Slerp(_characterController.transform.rotation, rot , 0.5f);

        _characterController.transform.rotation = targetRotation;
    }

    private bool Grounded()
    {
        bool grounded = false;
        for (int i = 0; i < _groundCheckPositions.Length; i++)
        {
            grounded = Physics.Raycast(_groundCheckPositions[i].position, Vector3.down, _groundDistanceThreshold, _layerMask);
            if (grounded)
            {
                return grounded;
            }
        }

        return grounded;
    }

    public async void AddForce(Vector3 dir, float strength)
    {
        float t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime * (1.0f / _floatiness);

            Vector3 forceVector = dir * strength * (1 - t) * Time.deltaTime;
            if (_characterController == null)
            {
                return;
            }
            _characterController.Move(forceVector);

            await Task.Yield();
        }
    }
}
