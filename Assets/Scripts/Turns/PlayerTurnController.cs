using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurnController : MonoBehaviour
{
    [Header("Yes I know this shouldn't be here")]
    [SerializeField]
    private float _punchRadius = 1;

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private int _damage = 2;

    [SerializeField]
    private float _knockbackStrength = 10;

    [Header("I can't be bothered doing it properly")]
    [SerializeField]
    private Weapon _grenadePrefab;

    [SerializeField]
    private float _grenadeForwardForce;

    [SerializeField]
    private float _grenadeUpwardForce;

    private Player _player;
    private PlayerAnimatorController _playerAnimator;
    private PlayerInputHandler _playerInput;

    private bool attacked = false;

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInputHandler>();

        _playerAnimator = GetComponent<PlayerAnimatorController>();
        _player = GetComponent<Player>();
        _player.OnBecomeActive += _player_OnBecomeActive;
    }

    private void _player_OnBecomeActive()
    {
        attacked = false;
    }

    private void OnDisable()
    {
        _player.OnBecomeActive -= _player_OnBecomeActive;
    }

    private void Update()
    {
        var input = _playerInput.GetPlayerInput();

        if (!CanAttack())
        {
            return;
        }

        if (input.Fire > 0)
        {
            Punch();
        }

        if (input.SecondaryFire > 0)
        {
            ThrowGrenade();
        }
    }

    private void ThrowGrenade()
    {
        attacked = true;

        var grendade = Instantiate(_grenadePrefab, transform.position + transform.forward * 0.03f, Quaternion.identity);
        grendade.GetComponent<Rigidbody>().AddForce(transform.forward * _grenadeForwardForce + Vector3.up * _grenadeUpwardForce, ForceMode.Impulse);
    }

    private bool CanAttack()
    {
        return _player.IsActive && !attacked;
    }

    private async void Punch()
    {
        attacked = true;

        _playerAnimator.SetPunch();

        await Task.Delay(800);

        GenerateAttackCollider();
    }

    private void GenerateAttackCollider() // This shouldn't be here
    {
        if (this == null)
        {
            return;
        }

        var results = Physics.OverlapSphere(transform.position + transform.forward * 0.02f, _punchRadius, _layerMask);

        for (int i = 0; i < results.Length; i++)
        {
            if (results[i].gameObject == this.gameObject)
            {
                continue;
            }

            if (results[i].TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.TakeDamage(_damage);
                results[i].GetComponent<PlayerMovementController>().AddForce(transform.forward, _knockbackStrength);
            }
        }
    }
}
