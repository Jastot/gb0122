using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLogin : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _mm;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private Text _playersCounter;
    [SerializeField] private Transform _parentListRoom;
    
    private ProfileManager _profileManager;
    private int _roomCounter;
    private List<RoomInfo> _roomInfos;
    private GameObject _listElementPrefab;
    
    private void Awake()
    {
        _profileManager = GetComponent<ProfileManager>();
        _listElementPrefab = Resources.Load<GameObject>("listElementPrefab");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _profileManager.OnUserNicknameUpdate += OnUserNicknameUpdate;
    }

    private void OnDestroy()
    {
        _profileManager.OnUserNicknameUpdate -= OnUserNicknameUpdate;
    }

    private void OnUserNicknameUpdate()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = _profileManager.Nickname;
            PhotonNetwork.ConnectUsingSettings();
        }
        _profileManager.OnUserNicknameUpdate -= OnUserNicknameUpdate;
    }
    
    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        _startButton.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomInfos = roomList;
        _roomCounter = roomList.Count;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed {message}");
    }

    public override void OnJoinedRoom()
    {
        _menu.SetActive(false);
        _mm.SetActive(true);
        _playersCounter.text = $"{PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    
    public override void OnLeftRoom()
    {
        _menu.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _playersCounter.text = $"{PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _playersCounter.text = $"{PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("ExampleScene");
    }

    public void OnCreateRoomButtonClicked()
    {
        var options = new RoomOptions {MaxPlayers = 2};
        PhotonNetwork.CreateRoom("", options);
    }

    public void OnJoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        RefreshListOfRooms();
    }

    public void OnJoinRoomById(string id)
    {
        PhotonNetwork.JoinRoom(id);
        RefreshListOfRooms();
    }

    public void GenerateListOfRooms()
    {
        foreach (var roomInfo in _roomInfos)
        {
           var go = Instantiate(_listElementPrefab, _parentListRoom).GetComponent<RoomListElementUI>();
           go._text.text = roomInfo.Name;
           go._joinButton.onClick.AddListener(()=>OnJoinRoomById(roomInfo.Name));
        }
    }

    public void RefreshListOfRooms()
    {
        foreach (var room in _parentListRoom.transform.GetComponentsInChildren<RoomListElementUI>())
        {
            room._joinButton.onClick.RemoveAllListeners();
            Destroy(room.gameObject);
        }
    }
    
    public void OnBackButtonClicked()
    {
        _mm.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
}