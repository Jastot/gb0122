using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PhotonLogin : MonoBehaviourPunCallbacks
{
    private string _roomName;

    [SerializeField] private GameObject playerList;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private PlayersElement element;
    [SerializeField] private LobbyElement _lobbyElement;
    [SerializeField] private GameObject _roomList;
    [SerializeField] private TMP_InputField _inputField;
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

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

    public void UpdateRoomName(string roomName)
    {
        _roomName = roomName;
    }

    public void OnCreateRoomButtonClicked()
    {
        RoomOptions roomOptions = new RoomOptions(){ IsVisible = true, IsOpen = true};
        PhotonNetwork.CreateRoom(_roomName,roomOptions);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed {message}");
    }

    public override void OnJoinedRoom()
    {
        createRoomPanel.SetActive(false);
        _roomList.SetActive(false);
        playerList.SetActive(true);
        foreach (var p in PhotonNetwork.PlayerList)
        {
            var newElement = Instantiate(element, element.transform.parent);
            newElement.gameObject.SetActive(true);
            newElement.SetItem(p);
        }
        
    }

    public void CloseOrOpenRoom(bool state)
    {
        PhotonNetwork.CurrentRoom.IsOpen = state;
    }
    
    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("ExampleScene");
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        cachedRoomList.Clear();
        Debug.Log("OnRoomListUpdate");
        ShowRooms(roomList);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        cachedRoomList.Clear();
    }
    
    public void ShowRooms(List<RoomInfo> roomList)
    {
        for(int i=0; i<roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
        foreach (var room in cachedRoomList.Values)
        {
            var newElement = Instantiate(_lobbyElement, _lobbyElement.transform.parent);
            newElement.gameObject.SetActive(true);
            newElement.SetItem(room.Name);
            newElement.NeedToJoinRoom += JoinRoomByName;
        }
    }

    private string _friendsRoomName;
    
    public void SetSearchName()
    {
        _friendsRoomName = _inputField.text;
    }
    
    private void JoinRoomByName(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    public void JoinRoomByName()
    {
        PhotonNetwork.JoinRoom(_friendsRoomName);
    }
    
    public void CopyNameCurrentOfRoom()
    {
        GUIUtility.systemCopyBuffer = PhotonNetwork.CurrentRoom.Name;
    }
}
