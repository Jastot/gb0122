using System;
using System.Globalization;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private Text nickname;
    
    private string _inputText;
    private string _userNickname;

    public string Nickname => _userNickname;

    public event Action OnUserNicknameUpdate;

    private void Start()
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), success =>
        {
            UpdateNickname(success.PlayerProfile.DisplayName);
        }, Debug.LogError);
    }

    public void UpdateInputFiled(string text)
    {
        _inputText = text;
    }

    public void SaveNicknameOnPlayFab()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = _inputText
        }, result =>
        {
            UpdateNickname(result.DisplayName);
        }, Debug.LogError);
    }

    private void UpdateNickname(string value)
    {
        _userNickname = value;
        nickname.text = $"Player Name: {_userNickname}";
        OnUserNicknameUpdate?.Invoke();
    }
    
    public void ClearCredentials()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Bootstrap");
    }
}
