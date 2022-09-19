using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnHandler : MonoBehaviour
{
    public event Action<Player> OnNewTurn = delegate { };

    private PlayerSpawner _playerSpawner;
    private CameraHandler _cameraHandler;
    private InputActions _inputActions;
    private InputAction _enter;

    private int _activeIndex = 0;

    private List<Player> _players = new List<Player>();

    public int ActiveIndex { get { return _activeIndex; } }

    private void Awake()
    {
        _cameraHandler = FindObjectOfType<CameraHandler>();
        _playerSpawner = FindObjectOfType<PlayerSpawner>();
        _playerSpawner.PlayersSpawned += _playerSpawner_PlayersSpawned;
    }

    private void _playerSpawner_PlayersSpawned(List<Player> players)
    {
        _players = players;
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].OnBecomeDead += TurnHandler_OnBecomeDead;
        }
    }


    private void TurnHandler_OnBecomeDead(Player player)
    {
        player.OnBecomeDead -= TurnHandler_OnBecomeDead;

        _players.Remove(player);

        if (_players.Count <= 1)
        {
            FindObjectOfType<WinHandler>(true).Win();
        }
    }

    private void OnEnable()
    {
        _inputActions = new InputActions();
        _enter = _inputActions.Player.Enter;

        _enter.Enable();
        _enter.performed += Enter_performed;
    }

    private void OnDisable()
    {
        _enter.Disable();

        _playerSpawner.PlayersSpawned -= _playerSpawner_PlayersSpawned;

        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].OnBecomeDead -= TurnHandler_OnBecomeDead;
        }
    }

    private void Enter_performed(InputAction.CallbackContext obj)
    {
        NextTurn();
    }

    public void FirstTurn()
    {
        _activeIndex = 0;
        _cameraHandler.FocusOnIndex(_activeIndex);

        OnNewTurn(_players[_activeIndex]);
    }

    public void NextTurn()
    {
        if (++_activeIndex >= _players.Count)
        {
            _activeIndex = 0;
        }

        _cameraHandler.FocusOnIndex(_activeIndex);

        OnNewTurn(_players[_activeIndex]);
    }
}
