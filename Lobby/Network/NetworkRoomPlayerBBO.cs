using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayerBBO : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject _lobbyUI;
    [SerializeField] TMP_Text[] _playerNameTexts = new TMP_Text[6];
    [SerializeField] TMP_Text[] _playerReadyTexts = new TMP_Text[6];
    [SerializeField] Button _startButton;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Waiting for player";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    bool _isLeader;
    public bool IsLeader
    {
        set
        {
            _isLeader = value;
            _startButton.gameObject.SetActive(value);
        }
    }

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

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        _lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        _Room.RoomPlayers.Add(this);
        UpdateDisplay();
    }
    public override void OnStopClient()
    {
        _Room.RoomPlayers.Remove(this);
        UpdateDisplay();
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach(var player in _Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for(int i = 0; i < _playerNameTexts.Length; i++)
        {
            _playerNameTexts[i].text = "Waiting for player";
            _playerReadyTexts[i].text = string.Empty;
        }
        for(int i = 0; i < _Room.RoomPlayers.Count; i++)
        {
            _playerNameTexts[i].text = _Room.RoomPlayers[i].DisplayName;
            _playerReadyTexts[i].text = _Room.RoomPlayers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not\nReady</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!_isLeader)
            return;
        _startButton.interactable = readyToStart;
    }

    [Command]
    void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;
        _Room.NotifyPlayersOfReadyState();
    }
    [Command]
    public void CmdStartGame()
    {
        if (_Room.RoomPlayers[0].connectionToClient != connectionToClient)
            return;
        _Room.StartGame();
    }
}
