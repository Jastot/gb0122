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
    [SerializeField] private Transform _parentOfPlayersCharacters;
    [SerializeField] private Transform _escapeUI;
    private CharacterControl _mainCharacterController;
    private MatchStatistics _matchStatistics;
    private List<CharacterControl> _playerList;
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
        _gameType = (PhotonLogin.GameType) PhotonNetwork.CurrentRoom.CustomProperties["GameType"];
        _uiSystem.StartGameUI.SetStartText(_gameType);
    }

    private void Start()
    {
        var allCharacterControllers = _parentOfPlayersCharacters.GetComponentsInChildren<CharacterControl>();
        foreach (var controller in allCharacterControllers)
        {
            if (controller.photonView.IsMine)
            {
                _mainCharacterController = controller;
                _mainCharacterController.photonView.RPC("SetTeamOrOffIt", RpcTarget.All,
                    (int)
                    PhotonNetwork.CurrentRoom.CustomProperties["GameType"]);
            }
        }

        _playerList = FindObjectsOfType<MonoBehaviour>().OfType<CharacterControl>().ToList();
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
    }

    private void GetSomeExp(int exp, CharacterData characterData)
    {
        bool isNotEnemy = characterData.GetComponent<CharacterControl>();
        if (isNotEnemy)
            _matchStatistics.KillPlayers++;
        else
            _matchStatistics.KillEnemy++;
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
        if (enemies == 0)
        {
            StartCoroutine(MakeTheEndOfGame(GameEndState.EnemiesDead));
        }
    }

    private void SetInfoAboutDeadPlayers(int exp, CharacterData data)
    {
        _playerList.Remove(data.GetComponent<CharacterControl>());
        if (_playerList.Count == 1)
            StartCoroutine(MakeTheEndOfGame(GameEndState.AllPlayersDead));
        List<int> tuple = new List<int>() {0, 0, 0};
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
        }
    }

    private IEnumerator MakeTheEndOfGame(GameEndState gameEndState)
    {
        yield return new WaitForSeconds(1);
        switch (gameEndState)
        {
            case GameEndState.PlayerDead:
                if (_gameType != PhotonLogin.GameType.TwoTeams)
                {
                    _matchStatistics.WinOrLoose = PlayerState.Loose;
                }

                ShowUIWithInfo();
                break;
            case GameEndState.EnemiesDead:
                if (_gameType != PhotonLogin.GameType.TwoTeams)
                {
                    if (_gameType == PhotonLogin.GameType.COOP)
                    {
                        AddScoreOfUser();
                        _matchStatistics.WinOrLoose = PlayerState.Win;
                        gameObject.GetPhotonView().RPC("COOPWin", RpcTarget.Others);
                    }
                    else
                    {
                        AddScoreOfUser();
                        if (_matchStatistics.KillEnemy >=
                            (_enemySpawner.StartCountOfEnemies / 2))
                        {
                            _matchStatistics.WinOrLoose = PlayerState.Win;
                            gameObject.GetPhotonView().RPC("EndGameLoose", RpcTarget.Others);
                        }
                        else
                        {
                            _matchStatistics.WinOrLoose = PlayerState.Loose;
                        }
                    }
                }
                ShowUIWithInfo();
                break;
            case GameEndState.AllPlayersDead:
                AddScoreOfUser();
                _matchStatistics.WinOrLoose = PlayerState.Win;
                gameObject.GetPhotonView().RPC("EndGameLoose", RpcTarget.Others);
                ShowUIWithInfo();
                break;
        }
    }



    private IEnumerator MakeTheEndOfGame(GameEndState gameEndState, int command)
    {
        yield return new WaitForSeconds(1);
        _matchStatistics.WinTeamColor = (TeamColor) command;
        //gameObject.GetPhotonView().RPC("EndGameWinTeam", RpcTarget.Others, command);
        if (_mainCharacterController.IsThisCharacterAttractable == command)
        {
            _matchStatistics.WinOrLoose = PlayerState.Win;
            AddScoreOfUser();
        }
        ShowUIWithInfo();
    }

    private void ShowUIWithInfo()
    {
        _uiSystem.EndGameUI.SetEndText(_matchStatistics, _gameType);
        _uiSystem.EndGameUI.ShowEndUI();
    }

    private void AddScoreOfUser()
    {
        _matchStatistics.Exp = _charactersLocalData.BattleResult.GainedExp;
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
        }, result => { _charactersLocalData.BattleResult.GainedExp = 0; }, Debug.LogError);
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

    [PunRPC]
    public void EndGameLoose()
    {
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

    public override void OnLeftRoom()
    {
        SceneManager.LoadSceneAsync("MainProfile");
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}