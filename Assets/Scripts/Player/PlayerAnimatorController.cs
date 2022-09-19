using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour 
{
    private PlayerMovementController _movementController;
    private Animator _animator;

    private void Start()
    {
        _movementController = GetComponentInChildren<PlayerMovementController>();
        _movementController.OnJump += MovementController_OnJump;

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float speed = _movementController.Speed;
        if (speed < 0.02f)
        {
            speed = 0;
        }
        _animator.SetFloat("Speed", speed);
    }

    private void OnDisable()
    {
        _movementController.OnJump -= MovementController_OnJump;
    }

    private void MovementController_OnJump()
    {
        _animator.SetTrigger("Jump");
    }

    public void SetPunch()
    {
        _animator.SetTrigger("Punch");
    }
}