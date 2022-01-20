using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLogin : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private Button _button;
    private bool _buttonCondition;
    private TextMeshProUGUI _buttonText;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _buttonCondition = true;
        _textMeshProUGUI.color = Color.magenta;
        _buttonText = _button.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ButtonShifter()
    {
        if (_buttonCondition)
            Connect();
        else
            Disconnect();
    }
    
    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        SetConditionAndShowInfo();
    }

    private void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
        SetConditionAndShowInfo();
    }

    private void SetConditionAndShowInfo()
    {
        if (_buttonCondition)
        {
            _textMeshProUGUI.text = "Connect";
            _buttonText.text = "Log out";
            _buttonCondition = false;
        }
        else
        {
            _textMeshProUGUI.text = "Disconnect";
            _buttonText.text = "Log in";
            _buttonCondition = true;
        }
    }
    
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Photon Success");
    }
}
