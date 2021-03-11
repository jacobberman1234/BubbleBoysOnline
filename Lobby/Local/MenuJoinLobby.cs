using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuJoinLobby : MonoBehaviour
{
    [SerializeField] NetworkManagerBBO _networkManager;

    [Header("UI")]
    [SerializeField] GameObject _landingPagePanel;
    [SerializeField] TMP_InputField _ipInputField;
    [SerializeField] Button _joinButton;

    void OnEnable()
    {
        NetworkManagerBBO.OnClientConnected += HandleClientConnected;
        NetworkManagerBBO.OnClientDisconnected += HandleClientDisconnected;
        _ipInputField.text = "localhost";
    }
    void OnDisable()
    {
        NetworkManagerBBO.OnClientConnected -= HandleClientConnected;
        NetworkManagerBBO.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = _ipInputField.text;
        _networkManager.networkAddress = ipAddress;
        _networkManager.StartClient();
    }

    void HandleClientConnected()
    {
        _joinButton.interactable = true;
        gameObject.SetActive(false);
        _landingPagePanel.SetActive(false);
    }
    void HandleClientDisconnected() => _joinButton.interactable = true;
}
