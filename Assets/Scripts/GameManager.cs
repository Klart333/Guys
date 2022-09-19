using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManager : Singleton<GameManager>
{
    private int _playerAmount;
    public int PlayerAmount { get { return _playerAmount; } }

    protected override void Awake()
    {
        base.Awake();

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene fromScene, Scene toScene)
    {
        if (toScene.buildIndex == 1 && _playerAmount != 0)
        {
            FindObjectOfType<PlayerSpawner>().SpawnPlayers(PlayerAmount);
        }
    }

    public void SetPlayerAmount(int amount)
    {
        _playerAmount = amount;
    }
}