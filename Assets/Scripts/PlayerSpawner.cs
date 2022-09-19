using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public event Action<List<Player>> PlayersSpawned = delegate { };

    [SerializeField]
    private Player _playerPrefab;

    [Header("Spawning")]
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private float minimumDistance = 1f;

    private List<Player> _spawnedPlayers = new List<Player>();

    private MarchingCubes _marchingCubes;

    private List<Vector3> _spawnedPlayerPositions = new List<Vector3>();

    private Vector2 _spawnBounds;

    private int _playerAmount = 0;
    private float _yRayLevel = 2;

    //public List<Player> SpawnedPlayers { get { return _spawnedPlayers; } }

    public void SpawnPlayers(int amount)
    {

        _playerAmount = amount;
        _marchingCubes = FindObjectOfType<MarchingCubes>();

        _spawnBounds = _marchingCubes.Bounds;

        _marchingCubes.OnMeshGenerated += InitializePlayers;
    }

    private void OnDisable()
    {
        _marchingCubes.OnMeshGenerated -= InitializePlayers;
    }

    private void InitializePlayers()
    {
        for (int i = 0; i < _playerAmount; i++)
        {
            _spawnedPlayers.Add(SpawnPlayer(i));
        }

        PlayersSpawned(_spawnedPlayers);
        FindObjectOfType<CameraHandler>().InitializeCameras(_spawnedPlayers);
    }

    private Player SpawnPlayer(int index)
    {
        Vector3 pos = GenerateRandomPosition();

        Player player = Instantiate(_playerPrefab, pos, Quaternion.identity);
        player.SetIndex(index);
        return player;
    }

    private Vector3 GenerateRandomPosition()
    {
        bool valid = false;
        int evaq = 0;
        float x = 0;
        float y = 0;
        float z = 0;
        while (!valid && evaq++ < 50)
        {
            valid = true;
            x = UnityEngine.Random.Range(0, _spawnBounds.x);
            z = UnityEngine.Random.Range(0, _spawnBounds.y);

            bool rayCast = Physics.Raycast(new Vector3(x, _yRayLevel, z), Vector3.down, out RaycastHit hitInfo, 10, _layerMask);
            if (rayCast)
            {
                y = hitInfo.point.y;
            }
            else
            {
                valid = false;
            }

            Vector3 point = new Vector3(x, y, z);

            for (int i = 0; i < _spawnedPlayerPositions.Count; i++)
            {
                if (Vector3.Distance(point, _spawnedPlayerPositions[i]) < minimumDistance)
                {
                    valid = false;
                }
            }
        }

        if (evaq >= 50)
        {
            Debug.LogError("Had to evaquate");
        }

        Vector3 validPosition = new Vector3(x, y, z);
        _spawnedPlayerPositions.Add(validPosition);

        return validPosition;
    }
}