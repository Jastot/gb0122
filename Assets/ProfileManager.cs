using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private Text _id;
    // Start is called before the first frame update
    void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), success =>
        {
            _id.text = $"Welcome back, Player {success.AccountInfo.PlayFabId}";
        }, errorCallback =>
        {
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
