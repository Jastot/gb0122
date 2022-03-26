using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreatorKitCode;
using CreatorKitCodeInternal;
using Data;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //тут нужно заинитить стартовое UI и отслеживание конца игры
    [SerializeField] private UISystem _uiSystem;
    [SerializeField] private CharactersLocalData _charactersLocalData;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private Transform _parentOfPlayersCharacters;
    [SerializeField] private Transform _escapeUI;
    private CharacterControl _mainCharacterController;
    private MatchStatistics _matchStatistics;
    private bool IsMainCharacterDead = false;
    
    public event Action EndOfGame;
    
    public enum PlayerState
    {
        Win,
        Loose
    }
    
    private void Awake()
    {
        _matchStatistics = new MatchStatistics();
        _uiSystem.StartGameUI.SetStartText((PhotonLogin.GameType)
            PhotonNetwork.CurrentRoom.CustomProperties["GameType"]);
    }

    private void Start()
    {
        var allCharacterControllers = _parentOfPlayersCharacters.GetComponentsInChildren<CharacterControl>();
        foreach (var controller in allCharacterControllers)
        {
            if (controller.GetComponent<PhotonView>().IsMine)
            {
                _mainCharacterController = controller;
                _mainCharacterController.photonView.RPC("SetTeamOrOffIt",RpcTarget.All,
                    (int)
                    PhotonNetwork.CurrentRoom.CustomProperties["GameType"]);
            }
            
            
        }    
        
        _mainCharacterController.Data.DeathRattle += SetMainCharacterDead;
        _mainCharacterController.GetSomeExp += GetSomeExp;
        _enemySpawner.CountOfEnemyChanged += CheckTheOfTheGame;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _escapeUI.gameObject.SetActive(!_escapeUI.gameObject.activeSelf);
        }
    }

    private void GetSomeExp(int exp,CharacterData characterData)
    {
        bool isNotEnemy = characterData.GetComponent<CharacterControl>();
        if (isNotEnemy)
            _matchStatistics.KillPlayers++;
        else
            _matchStatistics.KillEnemy++;
        _charactersLocalData.BattleResult.GainedExp += exp;
    }

    private void SetMainCharacterDead(int obj,CharacterData characterData)
    {
        IsMainCharacterDead = true;
    }

    private void CheckTheOfTheGame(int enemies)
    {
        Debug.Log("Enemy c: "+enemies);
        //TODO: переработать условие
        if (enemies == 0)
        {
            if (CheckTheOfTheGame(IsMainCharacterDead))
            {
                return;
            }
            EndOfTheGame(false);
        }
    }
    private bool CheckTheOfTheGame(bool isDead)
    {
        if (isDead)
        {
            EndOfTheGame(isDead);
            return true;
        }
        return false;
    }

    private void EndOfTheGame(bool isDead)
    {
        //EndOfGame?.Invoke();
        if(!isDead) 
            AddScoreOfUser();
        _uiSystem.EndGameUI.SetEndText(_matchStatistics,(PhotonLogin.GameType)
            PhotonNetwork.CurrentRoom.CustomProperties["GameType"]);
        _uiSystem.EndGameUI.ShowEndUI();
    }

    private void AddScoreOfUser()
    {
        _matchStatistics.Exp = _charactersLocalData.BattleResult.GainedExp;
        UpdateCharacterAfterGame();
        //PhotonNetwork.CurrentRoom.CustomProperties.Add("","");
    }

    public void OnDestroy()
    {
        _mainCharacterController.GetSomeExp -= GetSomeExp;
        _enemySpawner.CountOfEnemyChanged -= CheckTheOfTheGame;
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
        //TODO: выяснить в чем проблема потери уровня
        Debug.Log("newLevel: "+newLevel);
        Debug.Log("newExp: "+newExp);
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
        
        SceneManager.LoadSceneAsync("MainProfile");
    }

    public void LeaveGame()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        
        Application.Quit();
    }
}
