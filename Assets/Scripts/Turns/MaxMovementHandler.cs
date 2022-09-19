using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxMovementHandler : MonoBehaviour
{
    [SerializeField]
    private float _maxDistance = 1;

    private TurnHandler _turnHandler;
    private Player _activePlayer;
    private PlayerMovementController _activeMovementController;

    private Vector3 _startPosition;

    private float _distance = 0;

    public float MaxDistance { get => _maxDistance; }
    public float Distance { get => _distance; }

    private void Awake()
    {
        _turnHandler = FindObjectOfType<TurnHandler>();
        _turnHandler.OnNewTurn += TurnHandler_OnNewTurn;
    }

    private void OnDisable()
    {
        _turnHandler.OnNewTurn -= TurnHandler_OnNewTurn;
    }

    private void TurnHandler_OnNewTurn(Player activePlayer)
    {
        if (_activePlayer != null) // Reset the old one
        {
            _activeMovementController.MaxMovementReached = false;
        }

        _activePlayer = activePlayer;
        _activeMovementController = _activePlayer.GetComponent<PlayerMovementController>();
        _startPosition = _activePlayer.transform.position;
    }

    private void Update()
    {
        if (_activePlayer == null)
        {
            return;
        }

        _distance = Vector3.Distance(_startPosition, _activePlayer.transform.position);
        _activeMovementController.MaxMovementReached = _distance > _maxDistance;
    }
}
