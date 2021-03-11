using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGamePlayerBBO : NetworkBehaviour
{
    [SyncVar]
    string _displayName = "Waiting for player";

    NetworkManagerBBO _room;
    NetworkManagerBBO _Room
    {
        get
        {
            if (_room != null)
                return _room;
            return _room = NetworkManager.singleton as NetworkManagerBBO;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        _Room.GamePlayers.Add(this);
    }
    public override void OnStopClient()
    {
        _Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        _displayName = displayName;
    }

}
