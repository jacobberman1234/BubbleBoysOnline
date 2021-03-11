using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerBBO : NetworkManager
{
    [SerializeField] int _minPlayers = 2;
    [Scene] [SerializeField] string _lobbyScene;

    [Header("Room")]
    [SerializeField] NetworkRoomPlayerBBO _roomPlayerPrefab;

    [Header("Game")]
    [SerializeField] NetworkGamePlayerBBO _gamePlayerPrefab;
    [SerializeField] GameObject _playerSpawnSystem;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    const string SPAWNABLE_PREFABS = "SpawnablePrefabs";
    const string GAME_SCENE = "Scene_GameMap";

    public List<NetworkRoomPlayerBBO> RoomPlayers { get; } = new List<NetworkRoomPlayerBBO>();
    public List<NetworkGamePlayerBBO> GamePlayers { get; } = new List<NetworkGamePlayerBBO>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>(SPAWNABLE_PREFABS).ToList();
    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>(SPAWNABLE_PREFABS);
        foreach (var prefab in spawnablePrefabs)
            ClientScene.RegisterPrefab(prefab);
    }
    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        if(SceneManager.GetActiveScene().path != _lobbyScene)
        {
            conn.Disconnect();
            return;
        }
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerBBO>();
            RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == _lobbyScene)
        {
            bool isLeader = RoomPlayers.Count == 0;
            NetworkRoomPlayerBBO roomPlayerInstance = Instantiate(_roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }
    
    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
            player.HandleReadyToStart(IsReadyToStart());
    }

    bool IsReadyToStart()
    {
        if (numPlayers < _minPlayers)
            return false;
        foreach(var player in RoomPlayers)
        {
            if (!player.IsReady)
                return false;
        }

        return true;
    }

    public void StartGame()
    {
        if(SceneManager.GetActiveScene().path == _lobbyScene)
        {
            if (!IsReadyToStart())
                return;
            ServerChangeScene(GAME_SCENE);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if(SceneManager.GetActiveScene().path == _lobbyScene && newSceneName == GAME_SCENE)
        {
            for(int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(_gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName == GAME_SCENE)
        {
            GameObject playerSpawnSystemInstance = Instantiate(_playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn);
    }


}
