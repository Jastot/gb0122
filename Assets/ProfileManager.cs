using PlayFab;
using PlayFab.ClientModels;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private Text welcomeLabel;
    [SerializeField] private Text createdLabel;
    [SerializeField] private Text errorLabel;

    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), success =>
        {
            welcomeLabel.text = $"Welcome back, {success.AccountInfo.Username}";
            createdLabel.text = $"Profile was created at {success.AccountInfo.Created.ToString(CultureInfo.CurrentCulture)}";
        }, error =>
        {
            errorLabel.text = error.GenerateErrorReport();
        });
    }
    
    public void ClearCredentials()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Bootstrap");
    }
    
    public void PlayBattle()
    {
        SceneManager.LoadScene("ExampleScene");
    }
}
