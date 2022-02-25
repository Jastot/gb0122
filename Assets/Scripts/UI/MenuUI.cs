using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _playersProfile;
    private const string choosedProfile = "playfab-choosed-character";
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey(choosedProfile))
        {
            _playersProfile.SetActive(false);
        }
        else
        {
            _playersProfile.SetActive(true);
        }
    }

    public void ClearBaseCharacter()
    {
        PlayerPrefs.DeleteKey(choosedProfile);
    }
    
    public void SetBaseCharacter(string charactersName)
    {
        PlayerPrefs.SetString(choosedProfile, charactersName);
    }
}
