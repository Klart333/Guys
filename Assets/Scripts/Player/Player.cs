using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action OnBecomeActive = delegate { };
    public event Action<Player> OnBecomeDead = delegate { };

    private TurnHandler turnHandler;

    private int _index;
    public bool IsActive { get { return _index == turnHandler.ActiveIndex; } }

    private void Awake()
    {
        turnHandler = FindObjectOfType<TurnHandler>();

        turnHandler.OnNewTurn += TurnHandler_OnNewTurn;
    }

    private void OnDisable()
    {
        turnHandler.OnNewTurn -= TurnHandler_OnNewTurn;
    }

    private void TurnHandler_OnNewTurn(Player player)
    {
        if (IsActive)
        {
            OnBecomeActive();
        }
    }

    public void SetIndex(int index)
    {
        _index = index;
    }

    public void Die()
    {
        OnBecomeDead(this);
    }
}
