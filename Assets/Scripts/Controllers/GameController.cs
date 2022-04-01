using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using CreatorKitCode;
using CreatorKitCodeInternal;
using Data;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviourPunCallbacks,IPunObservable
{
    [SerializeField] private UISystem _uiSystem;
    [SerializeField] private TimeController _timeController;
    [SerializeField] private CharactersLocalData _charactersLocalData;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private Transform _escapeUI;
    private CharacterControl _mainCharacterController;
    private MatchStatistics _matchStatistics;
    private List<CharacterControl> _playerList;
    private int countOfPlayers;
    private PhotonLogin.GameType _gameType;
    public event Action EndOfGame;

    public enum PlayerState
    {
        Win,
        Loose,
        Waiting
    }

    public enum GameEndState
    {
        PlayerDead,
        EnemiesDead,
        AllPlayersDead,
        COOPDead
    }

    private void Awake()
    {
        _matchStatistics = new MatchStatistics();
        _matchStatistics.TeamEnemyKill = new List<int>();
        _matchStatistics.TeamEnemyKill.Add(0);
        _matchStatistics.TeamEnemyKill.Add(0);
        _matchStatistics.WinTeamColor = TeamColor.None;
        _charactersLocalData.BattleResult.GainedExp = 0;
        _gameType = (PhotonLogin.GameType) PhotonNetwork.CurrentRoom.CustomProperties["GameType"];
        _uiSystem.StartGameUI.SetStartText(_gameType);
    }

    private void Start()
    {
        
        var allCharacterControllers = FindObjectsOfType<MonoBehaviour>().OfType<CharacterControl>().ToList();
        var c = 0;
        foreach (var controller in allCharacterControllers)
        {
            _matchStatistics.KillEnemy = new Dictionary<int,int>();
            _matchStatistics.KillPlayers = new Dictionary<int,int>();
            _matchStatistics.Exp = new Dictionary<int,int>();
            if (controller.photonView.IsMine)
            {
                _matchStatistics.playerNum = controller.photonView.ViewID;
                _matchStatistics.KillEnemy.Add(_matchStatistics.playerNum,0);
                _matchStatistics.KillPlayers .Add(_matchStatistics.playerNum,0);
                _matchStatistics.Exp.Add(_matchStatistics.playerNum,0);
                _mainCharacterController = controller;
                _mainCharacterController.photonView.RPC("SetTeamOrOffIt", RpcTarget.All,
                    (int)
                    PhotonNetwork.CurrentRoom.CustomProperties["GameType"]);
            }

            c++;
        }

        _playerList = FindObjectsOfType<MonoBehaviour>().OfType<CharacterControl>().ToList();
        countOfPlayers = _playerList.Count;
        foreach (var player in _playerList)
        {
            if (!player.photonView.IsMine)
            {
                player.Data.DeathRattle += SetInfoAboutDeadPlayers;
            }
        }

        _mainCharacterController.Data.DeathRattle += SetMainCharacterDead;
        _mainCharacterController.GetSomeExp += GetSomeExp;
        _enemySpawner.CountOfEnemyChanged += CheckTheEnemyCount;
        _matchStatistics.WinOrLoose = PlayerState.Waiting;
    }

    private bool flag = true;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _escapeUI.gameObject.SetActive(!_escapeUI.gameObject.activeSelf);
        }

        if (_matchStatistics.WinOrLoose==PlayerState.Win||
            _matchStatistics.WinOrLoose==PlayerState.Loose)
        {
            _timeController.StopTimer();
        }

        if (countOfLinks == countOfPlayers && flag)
        {
            flag = false;
            Calculate();
        }
    }

    private void GetSomeExp(int exp, CharacterData characterData)
    {
        bool isNotEnemy = characterData.GetComponent<CharacterControl>();
        if (isNotEnemy)
            _matchStatistics.KillPlayers[_matchStatistics.playerNum]++;
        else
            _matchStatistics.KillEnemy[_matchStatistics.playerNum]++;
        _charactersLocalData.BattleResult.GainedExp += exp;
    }

    private void SetMainCharacterDead(int obj, CharacterData characterData)
    {
        var enemy = _enemySpawner.GetEnemiesCharData();
        foreach (var character in enemy)
        {
            character.GetComponent<SimpleEnemyController>().photonView.RPC("DeleteSomeCharacter", RpcTarget.All,
                _mainCharacterController.photonView.ViewID);
        }

        StartCoroutine(MakeTheEndOfGame(GameEndState.PlayerDead));
    }

    private void CheckTheEnemyCount(int enemies)
    {
        //no time
        if (_mainCharacterController.IsThisCharacterAttractable == 0)
        {
            _matchStatistics.TeamEnemyKill[0]++;
            this.photonView.RPC("SetTeamCount",RpcTarget.All,_matchStatistics.TeamEnemyKill[0],0);
        }

        if (_mainCharacterController.IsThisCharacterAttractable == 1)
        {
            _matchStatistics.TeamEnemyKill[1]++;
            this.photonView.RPC("SetTeamCount",RpcTarget.All,_matchStatistics.TeamEnemyKill[1],1);
            
        }

        if (enemies == 0)
        {
            StartCoroutine(MakeTheEndOfGame(GameEndState.EnemiesDead));
        }
    }

    [PunRPC]
    public void SetTeamCount(int count,int team)
    {
        _matchStatistics.TeamEnemyKill[team] = count;
    }

    private void SetInfoAboutDeadPlayers(int exp, CharacterData data)
    {
        _playerList.Remove(data.GetComponent<CharacterControl>());
        if (_playerList.Count == 1)
            StartCoroutine(MakeTheEndOfGame(GameEndState.AllPlayersDead));
        /*List<int> tuple = new List<int>() {0, 0, 0};
        foreach (var player in _playerList)
        {
            switch (player.IsThisCharacterAttractable)
            {
                case 0:
                    tuple[0]++;
                    break;
                case 1:
                    tuple[1]++;
                    break;
                case -1:
                    tuple[2]++;
                    break;
            }
        }

        if (tuple[2] > 1)
            return;
        else
        {
            if (tuple[0] == 0)
                StartCoroutine(MakeTheEndOfGame(GameEndState.COOPDead, 1));
            if (tuple[1] == 0)
                StartCoroutine(MakeTheEndOfGame(GameEndState.COOPDead, 0));
        }*/
    }

    public static int getMax(Dictionary<int,int> inputArray, out int index){ 
        
        var indexs = inputArray.Keys.ToList();
        int maxValue = inputArray[indexs[0]];
        index = 0;
        for(int i=1;i < inputArray.Count;i++){ if(inputArray[indexs[i]] > maxValue){ 
                maxValue = inputArray[indexs[i]];
                index = indexs[i];
            } 
        } 
        return maxValue; 
    }

    private int countOfLinks = 0;
    private GameEndState _gameEndState;
    
    [PunRPC]
    public void LinkWithMaster(int index, int enemyKill,int playerKill, int exp, int gameend)
    {
        Debug.Log("hmm");
        if (!_matchStatistics.KillEnemy.ContainsKey(index))
            _matchStatistics.KillEnemy.Add(index,enemyKill);
        else
            _matchStatistics.KillEnemy[index] = enemyKill;
        if (!_matchStatistics.KillPlayers.ContainsKey(index))
            _matchStatistics.KillPlayers.Add(index,playerKill);
        else
            _matchStatistics.KillPlayers[index] = playerKill;
        if (!_matchStatistics.Exp.ContainsKey(index))
            _matchStatistics.Exp.Add(index, exp);
        else
            _matchStatistics.Exp[index] = exp;
        countOfLinks++;
        _gameEndState = (GameEndState)gameend;
    }
    private void Calculate()
    {
        switch (_gameEndState)
        {
            case GameEndState.EnemiesDead:
                if (_gameType == PhotonLogin.GameType.TwoTeams)
                {
                    if (_matchStatistics.TeamEnemyKill[0]>_matchStatistics.TeamEnemyKill[1])
                    {
                        gameObject.GetPhotonView().RPC("EndGameWinTeam", RpcTarget.All, 0);
                    }
                    else
                    {
                        gameObject.GetPhotonView().RPC("EndGameWinTeam", RpcTarget.All, 1);
                    }
                }

                if (_gameType == PhotonLogin.GameType.HateAll)
                {
                    getMax(_matchStatistics.KillEnemy, out var index);
                    gameObject.GetPhotonView().RPC("EndGameLoose", RpcTarget.All,index);
                }
        
                if (_gameType == PhotonLogin.GameType.COOP)
                {
                    gameObject.GetPhotonView().RPC("COOPWin", RpcTarget.All);
                }
                break;
        }
        
    }

    [PunRPC]
    public void EndGameLoose(int index)
    {
        if (_matchStatistics.playerNum == index)
        {
            _matchStatistics.WinOrLoose = PlayerState.Win;
            AddScoreOfUser();
        }
        else
            _matchStatistics.WinOrLoose = PlayerState.Loose;
        ShowUIWithInfo();
    }
    
    [PunRPC]
    public void COOPWin()
    {
        _matchStatistics.WinOrLoose = PlayerState.Win;
        AddScoreOfUser();
        ShowUIWithInfo();
    }

    [PunRPC]
    public void EndGameWinTeam(int color)
    {
        if (_mainCharacterController.IsThisCharacterAttractable == color)
        {
            _matchStatistics.WinOrLoose = PlayerState.Win;
            AddScoreOfUser();
        }
        else
        {
            _matchStatistics.WinOrLoose = PlayerState.Loose;
        }
        _matchStatistics.WinTeamColor = (TeamColor) color;
        ShowUIWithInfo();
    }
    
    
    private IEnumerator MakeTheEndOfGame(GameEndState gameEndState)
    {
        photonView.RPC("LinkWithMaster",RpcTarget.MasterClient,
            _matchStatistics.playerNum,
            _matchStatistics.KillEnemy[_matchStatistics.playerNum],
            _matchStatistics.KillPlayers[_matchStatistics.playerNum],
            _matchStatistics.Exp[_matchStatistics.playerNum],
            (int)gameEndState);
        yield return new WaitForSeconds(2);
        switch (gameEndState)
        {
            case GameEndState.PlayerDead:
                if (_gameType != PhotonLogin.GameType.TwoTeams)
                {
                    _matchStatistics.WinOrLoose = PlayerState.Loose;
                }

                ShowUIWithInfo();
                break;
        }
        /*  case GameEndState.EnemiesDead:
              if (_gameType != PhotonLogin.GameType.TwoTeams)
              {
                  if (_gameType == PhotonLogin.GameType.COOP)
                  {
                     // AddScoreOfUser();
                      //_matchStatistics.WinOrLoose = PlayerState.Win;
                      gameObject.GetPhotonView().RPC("COOPWin", RpcTarget.MasterClient);
                  }
                  else
                  {
                    // AddScoreOfUser();
                  //getMax(_matchStatistics.KillEnemy, out var index);
                  //if (_matchStatistics.playerNum == index)
                  //{
                   //_matchStatistics.WinOrLoose = PlayerState.Win;
                  gameObject.GetPhotonView().RPC("EndGameLoose", RpcTarget.MasterClient);
                  /*}
                  else
                  {
                   _matchStatistics.WinOrLoose = PlayerState.Loose;
                  }#1#
                  }
                  ShowUIWithInfo();

              }
              else
              {
                if (_matchStatistics.TeamEnemyKill[0]>_matchStatistics.TeamEnemyKill[1])
                {
                    gameObject.GetPhotonView().RPC("EndGameWinTeam", RpcTarget.MasterClient, 0);
                }
                else
                {
                    gameObject.GetPhotonView().RPC("EndGameWinTeam", RpcTarget.MasterClient, 1);
                }
                                
              }
              break;
          case GameEndState.AllPlayersDead:
              /*AddScoreOfUser();
              _matchStatistics.WinOrLoose = PlayerState.Win;
              gameObject.GetPhotonView().RPC("EndGameLoose", RpcTarget.MasterClient);
              ShowUIWithInfo();#1#
              gameObject.GetPhotonView().RPC("EndGameLoose", RpcTarget.MasterClient);
              break;
      }*/
    }

  
    /*
    private IEnumerator MakeTheEndOfGame(GameEndState gameEndState, int command)
    {
        photonView.RPC("LinkWithOthers",RpcTarget.MasterClient,
            _matchStatistics.playerNum,
            _matchStatistics.KillEnemy[_matchStatistics.playerNum],
            _matchStatistics.KillPlayers[_matchStatistics.playerNum],
            _matchStatistics.Exp[_matchStatistics.playerNum]);
        yield return new WaitForSeconds(1);
        _matchStatistics.WinTeamColor = (TeamColor) command;
        gameObject.GetPhotonView().RPC("EndGameWinTeam", RpcTarget.All, command);
        /*if (_mainCharacterController.IsThisCharacterAttractable == command)
        {
            _matchStatistics.WinOrLoose = PlayerState.Win;
            AddScoreOfUser();
        }#1#
        //ShowUIWithInfo();
    }
    */

 
    private void ShowUIWithInfo()
    {
        _uiSystem.EndGameUI.SetEndText(_matchStatistics, _gameType);
        _uiSystem.EndGameUI.ShowEndUI();
    }

    private void AddScoreOfUser()
    {
        _matchStatistics.Exp[_matchStatistics.playerNum] = _charactersLocalData.BattleResult.GainedExp;
        UpdateCharacterAfterGame();
    }

    public void OnDestroy()
    {/*
        foreach (var player in _playerList)
        {
            if (!player.photonView.IsMine)
            {
                player.Data.DeathRattle -= SetInfoAboutDeadPlayers;
            }
        }
        */

        _mainCharacterController.Data.DeathRattle -= SetMainCharacterDead;
        _mainCharacterController.GetSomeExp -= GetSomeExp;
        _enemySpawner.CountOfEnemyChanged -= CheckTheEnemyCount;
    }

    private void UpdateCharacterAfterGame()
    {
        var level = _charactersLocalData.CharacterStatistics["Level"];
        var exp = _charactersLocalData.CharacterStatistics["Exp"];
        int prog;
        if (level == 0)
            prog = 0;
        else
            prog = 450 * (level);
        prog += exp;
        prog += _charactersLocalData.BattleResult.GainedExp;
        var newLevel = prog / 450;
        var newExp = prog % 450;
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest()
        {
            CharacterId = _charactersLocalData.CurrentCharacter,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"Level", newLevel},
                {"Exp", newExp}
            }
        }, result => {  }, Debug.LogError);
    }

    public void Resume()
    {
        _escapeUI.gameObject.SetActive(!_escapeUI.gameObject.activeSelf);
    }

    public void LeaveRoomAndLoadMain()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
       
    }

    public void LeaveGame()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        Application.Quit();
    }

    

    public override void OnLeftRoom()
    {
        SceneManager.LoadSceneAsync("MainProfile");
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}