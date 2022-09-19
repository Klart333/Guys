using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerCamera _cameraPrefab;

    private List<PlayerCamera> playerCams = new List<PlayerCamera>();

    public void InitializeCameras(List<Player> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            var cam = Instantiate(_cameraPrefab, players[i].transform);
            cam.Initialize(players[i].transform, i == 0 ? 1 : 0);

            playerCams.Add(cam);
        }

        FindObjectOfType<TurnHandler>().FirstTurn();
    }

    public void FocusOnIndex(int index)
    {
        for (int i = 0; i < playerCams.Count; i++)
        {
            playerCams[i].SetPriority(i == index ? 1 : 0);
        }
    }
}
