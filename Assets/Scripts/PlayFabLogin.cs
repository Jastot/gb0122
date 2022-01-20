using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "5EE75";
            Debug.Log("Title ID was installed");
        }
        
    }

    public void LogInPlayFab()
    {
        var request = new LoginWithCustomIDRequest {CustomId = "lesson3", CreateAccount = true};
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        _textMeshProUGUI.color = Color.green;
        _textMeshProUGUI.text = "PlayFab Success";
    }

    private void OnLoginFailure(PlayFabError error)
    {
        _textMeshProUGUI.color = Color.red;
        _textMeshProUGUI.text = $"Fail: {error}";
    }
}
