using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMain : MonoBehaviour
{
    [SerializeField] NetworkManagerBBO _networkManager;

    [Header("UI")]
    [SerializeField] GameObject _landingPagePanel;

    public void HostLobby()
    {
        _networkManager.StartHost();
        _landingPagePanel.SetActive(false);
    }
}
