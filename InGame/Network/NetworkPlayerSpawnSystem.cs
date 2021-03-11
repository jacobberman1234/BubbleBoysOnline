using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkPlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefab;

    static List<Transform> _spawnPoints = new List<Transform>();
    int _nextIndex = 0;

    public static void AddSpawnPoint(Transform spawnPoint)
    {
        _spawnPoints.Add(spawnPoint);
        _spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }
    public static void RemoveSpawnPoint(Transform spawnPoint) => _spawnPoints.Remove(spawnPoint);

    public override void OnStartServer() => NetworkManagerBBO.OnServerReadied += SpawnPlayer;
    [ServerCallback]
    void OnDestroy() => NetworkManagerBBO.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = _spawnPoints.ElementAtOrDefault(_nextIndex);
        if(spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {_nextIndex}");
            return;
        }
        GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoints[_nextIndex].position, _spawnPoints[_nextIndex].rotation);
        NetworkServer.Spawn(playerInstance, conn);
        _nextIndex++;
    }
}
