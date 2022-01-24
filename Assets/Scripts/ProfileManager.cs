using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private Text _id;

    [SerializeField] private Image _loading;
    // Start is called before the first frame update
    void Start()
    {
        _loading.color = Color.yellow;
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), success =>
        {
            _id.text = $"Welcome back, Player 'LOST NAME' \n " +
                       $"With ID {success.AccountInfo.PlayFabId} \n " +
                       $"Created: {success.AccountInfo.Created}";
            _loading.color = Color.green;
        }, errorCallback =>
        {
            _loading.color = Color.red;
        });
    }

    public void LogOutAndForget()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Bootstrap");
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
