using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField] private Text loginErrorLabel;
    [SerializeField] private Text createAccountErrorLabel;
    
    private string _username;
    private string _mail;
    private string _pass;

    private const string PlayFabUsernameForAuthKey = "playfab-username-for-auth-key";
    private const string PlayFabPasswordForAuthKey = "playfab-password-for-auth-key";
    
    public void UpdateUsername(string username)
    {
        _username = username;
    }

    public void UpdateEmail(string mail)
    {
        _mail = mail;
    }

    public void UpdatePass(string pass)
    {
        _pass = pass;
    }

    public void CreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _username,
            Email = _mail,
            Password = _pass,
            RequireBothUsernameAndEmail = true
        }, _ =>
        {
            Debug.Log($"Create Account Success: {_.PlayFabId}");
            RememberCredentials(_username, _pass);
            SceneManager.LoadScene("MainProfile");
        }, OnFailure);
    }

    public void Login()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _username,
            Password = _pass
        }, result =>
        {
            Debug.Log($"Login Success: {result.PlayFabId}");
            RememberCredentials(_username, _pass);
            SceneManager.LoadScene("MainProfile");
        }, OnFailure);
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "5EE75";
            Debug.Log("Title ID was installed");
        }

        if (PlayerPrefs.HasKey(PlayFabUsernameForAuthKey) && PlayerPrefs.HasKey(PlayFabPasswordForAuthKey))
        {
            PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
            {
                Username = PlayerPrefs.GetString(PlayFabUsernameForAuthKey),
                Password = PlayerPrefs.GetString(PlayFabPasswordForAuthKey)
            }, result =>
            {
                Debug.Log($"Login Success: {result.PlayFabId}");
                SceneManager.LoadScene("MainProfile");
            }, OnFailure);
        }
    }

    private void OnFailure(PlayFabError error)
    {
        loginErrorLabel.text = error.GenerateErrorReport();
        createAccountErrorLabel.text = error.GenerateErrorReport();
    }

    private void RememberCredentials(string username, string pass)
    {
        PlayerPrefs.SetString(PlayFabUsernameForAuthKey, username);
        PlayerPrefs.SetString(PlayFabPasswordForAuthKey, pass);
    }
}
