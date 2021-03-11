using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_InputField _nameInputField;
    [SerializeField] Button _confirmButton;

    public static string DisplayName { get; private set; }
    const string PLAYER_PREFS_NAME_KEY = "PlayerName";

    void Start() => SetUpInputField();

    void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PLAYER_PREFS_NAME_KEY))
            return;
        string defaultName = PlayerPrefs.GetString(PLAYER_PREFS_NAME_KEY);
        _nameInputField.text = defaultName;
        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        _confirmButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        DisplayName = _nameInputField.text;
        PlayerPrefs.SetString(PLAYER_PREFS_NAME_KEY, DisplayName);
    }
}
