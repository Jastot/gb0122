using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class PhotonLogin : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _mm;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private TMP_Text _showErrorOfRandomConnection;
    [SerializeField] private Transform _parentListRoom;
    [SerializeField] private CharactersLocalData _charactersLocalData;
    
    [Header("MatchMaking")] 
    [SerializeField] private Text _playersCounter;
    [SerializeField] private TMP_Text _yourTeamIs;
    [SerializeField] private Button _startGameButton;
    
    [Header("Creating room elements")] 
    [SerializeField] private TMP_InputField _inputFieldRoomName;
    [SerializeField] private TMP_Dropdown _dropdownMatchType;
    [SerializeField] private TMP_Dropdown _dropdownRoomPrivacy;
    [SerializeField] private TMP_Text _tmpTextPlayerMax;
    // [SerializeField] private Button _buttonMorePlayers;
    // [SerializeField] private Button _buttonLessPlayers;
    
    private ProfileManager _profileManager;
    private List<RoomInfo> _roomInfos;
    private GameObject _listElementPrefab;
    [Range(1,6)]
    private int PlayersInRoom = 1;
    private int _roomCounter;
    private string _roomName="";
    private string currentPlayer;
    public enum GameType
    {
        COOP,
        TwoTeams,
        HateAll
    }

    private void Awake()
    {
        _profileManager = GetComponent<ProfileManager>();
        _listElementPrefab = Resources.Load<GameObject>("listElementPrefab");
        PhotonNetwork.AutomaticallySyncScene = true;
        _tmpTextPlayerMax.text = "1";
    }

    private void Start()
    {
        _profileManager.OnUserNicknameUpdate += OnUserNicknameUpdate;
    }

    private void OnDestroy()
    {
        _profileManager.OnUserNicknameUpdate -= OnUserNicknameUpdate;
    }

    
    
    public void AddMorePlayers()
    {
        if (PlayersInRoom < 6)
        {
            PlayersInRoom++;
            _tmpTextPlayerMax.text = PlayersInRoom.ToString();
        }
    }

    public void LessPlayers()
    {
        if (PlayersInRoom >1)
        {
            PlayersInRoom--;
            _tmpTextPlayerMax.text = PlayersInRoom.ToString();
        }
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
        if (!PhotonNetwork.IsMasterClient)
            _startGameButton.gameObject.SetActive(false);
        else
            _startGameButton.gameObject.SetActive(true);
        
        
        _playersCounter.text = $"{PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        currentPlayer = PhotonNetwork.PlayerList.Last().UserId;
        if ((GameType)PhotonNetwork.CurrentRoom.CustomProperties["GameType"] == GameType.TwoTeams)
        {
            Hashtable hashtable = new Hashtable();
            if (PhotonNetwork.CurrentRoom.Players.Count % 2 == 0)
            {
                _yourTeamIs.text = "Red Team";
                _yourTeamIs.color = Color.red;
                hashtable.Add("Color", 0);
            }
            else
            {
                _yourTeamIs.text = "Blue Team";
                _yourTeamIs.color = Color.blue;
                hashtable.Add("Color", 1);
            }

            PhotonNetwork.PlayerList.Last().SetCustomProperties(hashtable);
        }

        _charactersLocalData.InRoomId = currentPlayer;
        /*var pl = (Dictionary<string,int>)PhotonNetwork.CurrentRoom.CustomProperties["PlayersObjs"];
        pl.Add(currentPlayer,TeamNum);
        PhotonNetwork.CurrentRoom.CustomProperties["PlayersObjs"] = pl;*/
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //TODO:sync
        _playersCounter.text = $"{PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _playersCounter.text = $"{PhotonNetwork.CurrentRoom.Players.Count} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        //var pl = (Dictionary<string,int>)PhotonNetwork.CurrentRoom.CustomProperties["PlayersObjs"];
        /*Debug.Log("not left "+pl.Count);
        foreach (var playerObj in pl)
        {
            if (playerObj.Key==currentPlayer)
            {
                pl.Remove(playerObj.Key);
                return;
            }
        }
        Debug.Log(pl.Count);
        PhotonNetwork.CurrentRoom.CustomProperties["PlayersObjs"] = pl;*/
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        //сохранить команды hashtable.Add("GameType",GameType.TwoTeams);
       
        PhotonNetwork.LoadLevel("ExampleScene");
    }

    
    public void OnNameOfRoomChanged(string text)
    {
        _roomName = _inputFieldRoomName.text;
    }
    
    public void OnCreateRoomButtonClicked()
    {
        var options = new RoomOptions {};
        Hashtable hashtable = new Hashtable();
        
        //GameType
        switch (_dropdownMatchType.value)
        {
            case 0:
                hashtable.Add("GameType",GameType.COOP);
                break;
            case 1:
                hashtable.Add("GameType",GameType.TwoTeams);
                break;
            case 2:
                hashtable.Add("GameType",GameType.HateAll);
                break;
        }
        //MaxPlayers
        options.MaxPlayers = Convert.ToByte(PlayersInRoom);
        //OpenOrPrivateRoom
        options.IsVisible = !Convert.ToBoolean(_dropdownRoomPrivacy.value);
        //
        /*Dictionary<string,int> obj = new Dictionary<string,int>();
        hashtable.Add("PlayersObjs",obj);
        */
        hashtable.Add("Timer",(double)0);
        options.CustomRoomProperties = hashtable;
        //options.PublishUserId = true;
        PhotonNetwork.CreateRoom(_roomName, options);
        
    }

    public void OnJoinRandomRoom()
    {
        if (_roomCounter==0)
        {
            _showErrorOfRandomConnection.text = "No rooms in public";
            return;
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
            _showErrorOfRandomConnection.text = "";
        }
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