using System.Globalization;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private Text nickname;
    [SerializeField] private Text id;
    [SerializeField] private Text created;
    
    private string _inputText;

    private void Start()
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), success =>
        {
            nickname.text = $"Player Name: {success.PlayerProfile.DisplayName}";
            // id.text = $"Player ID: {success.AccountInfo.PlayFabId}";
            // created.text = $"Creation time: {success.AccountInfo.Created.ToString(CultureInfo.CurrentCulture)}";
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
        }, result => { }, Debug.LogError);
    }
    
    public void ClearCredentials()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Bootstrap");
    }
}
