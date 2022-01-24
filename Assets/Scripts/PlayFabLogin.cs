using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField] private Image _loading;
    private string _username;
    private string _mail;
    private string _pass;

    private const string AuthKey = "player-unique-id";
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
        }, result =>
        {
            Debug.Log("Success");
        }, errorCallback => {
            Debug.LogError($"Error: {errorCallback}");
        });
    }

    public void Login()
    {
        
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _username,
            Password = _pass
        }, result =>
        {
            
            Debug.Log($"Success: {_username}");
        }, errorCallback => {
            Debug.LogError($"Error: {errorCallback}");
        });
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "5EE75";
            Debug.Log("Title ID was installed");
        }

        var needCreation = !PlayerPrefs.HasKey(AuthKey);
        Debug.Log($"needCreation = {needCreation}");
        var id = PlayerPrefs.GetString(AuthKey, Guid.NewGuid().ToString());
        Debug.Log($"id = {id}");
        var request = new LoginWithCustomIDRequest {CustomId = id, CreateAccount = needCreation};
        _loading.color = Color.red;
        PlayFabClientAPI.LoginWithCustomID(request, reuslt =>
        {
            _loading.color = Color.yellow;
            PlayerPrefs.SetString(AuthKey, id);
            SceneManager.LoadScene("MainProfile");
        }, OnLoginFailure);
    }
    
    private void OnLoginFailure(PlayFabError error)
    {
        _loading.color = Color.magenta;
        Debug.LogError($"Fail: {error}");
    }
}
