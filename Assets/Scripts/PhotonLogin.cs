using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLogin : MonoBehaviourPunCallbacks
{
    private string _roomName;

    [SerializeField] private GameObject playerList;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private PlayersElement element;
    
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinRandomRoom();
    }

    public void UpdateRoomName(string roomName)
    {
        _roomName = roomName;
    }
    
    public void OnCreateRoomButtonClicked()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed {message}");
    }

    public override void OnJoinedRoom()
    {
        createRoomPanel.SetActive(false);
        playerList.SetActive(true);
        foreach (var p in PhotonNetwork.PlayerList)
        {
            var newElement = Instantiate(element, element.transform.parent);
            newElement.gameObject.SetActive(true);
            newElement.SetItem(p);
        }
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        createRoomPanel.SetActive(false);
        playerList.SetActive(true);
        foreach (var p in roomList)
        {
            var newElement = Instantiate(element, element.transform.parent);
            newElement.gameObject.SetActive(true);
            newElement.SetItem(p.Name);
        }
    }
    
    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("ExampleScene");
    }
}
