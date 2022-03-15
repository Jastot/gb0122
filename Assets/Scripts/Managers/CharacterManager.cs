using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    private const string CharactersStoreId = "characters_store";
    private const string VirtualCurrencyKey = "GC";
    //[SerializeField] private GameController _gameController;
    [SerializeField] private GameObject _whatToShow;
    [SerializeField] private List<GameObject> _whatToHide;
    [SerializeField] private TMP_Text infoCharacter;
    [SerializeField] private List<GameObject> CharactersButtons;
    [SerializeField] private CharactersLocalData _charactersLocalData;
    [SerializeField] private Transform _parent;
    [SerializeField] private InputField _inputFieldText;
    private GameObject _characterPrefab;

    private void Start()
    {
        _characterPrefab = Resources.Load<GameObject>("ChooseCharacterButton");
        CharactersButtons = new List<GameObject>();
        UpdateCharacters();
        /*if (_charactersLocalData.BattleResult.AggregatedDamage > 0)
        {
            UpdateCharacterAfterBattle();
        }*/
        //_gameController.EndOfGame += UpdateCharacterAfterGame;
    }

    private void OnDestroy()
    {
       //_gameController.EndOfGame -= UpdateCharacterAfterGame;
    }

    public void onValueChanged()
    {
        SetCurrentCharacter(_inputFieldText.text);
    }
    
    public void OnCreatButtonClicked()
    {
        if (string.IsNullOrEmpty(_inputFieldText.text))
        {
            Debug.LogError("Input field should not be empty");
            return;
        }
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
        {
            StoreId = CharactersStoreId
        }, result =>
        {
            HandleStoreResult(result.Store);
        }, Debug.LogError);
    }

    private void HandleStoreResult(List<StoreItem> items)
    {
        foreach (var item in items)
        {
            Debug.Log(item.ItemId);
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
            {
                ItemId = item.ItemId,
                Price = (int)item.VirtualCurrencyPrices[VirtualCurrencyKey],
                VirtualCurrency = VirtualCurrencyKey
            }, result =>
            {
                Debug.Log($"Item {result.Items[0].ItemId} was purchased");
                TransormItemIntoCharacter(result.Items[0].ItemId);
            }, Debug.LogError);
        }
    }

    private void TransormItemIntoCharacter(string itemId)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            ItemId = itemId,
            CharacterName = _inputFieldText.text
        }, result =>
        {
            UpdateCharacterStatistics(result.CharacterId);
        }, Debug.LogError);
    }

    private void UpdateCharacterStatistics(string characterId)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterId,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"Level", 1},
                {"Exp", 0},
                {"HealthPoints", 100}
            }
        }, result =>
        {
            //createPanel.SetActive(false);
            UpdateCharacters();
        }, Debug.LogError);
    }
    private ListUsersCharactersResult _characterResult;
    public void UpdateCharacters()
    {
        RefreshList();
        GetCharacters();
        if (_characterResult == null)
        {
            return;
        }
        foreach (var character in _characterResult.Characters)
        {
            CharactersButtons.Add(Instantiate(_characterPrefab, _parent));
            var texts = CharactersButtons.Last().GetComponentsInChildren<Text>();
            texts[0].text = character.CharacterName;
            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest()
                    {CharacterId = character.CharacterId},
                result =>
                {
                    texts[1].text = result.CharacterStatistics["Level"].ToString();
                },Debug.LogError);
            var but = CharactersButtons.Last().GetComponent<Button>();
            but.onClick.AddListener(()=> ChooseCharacter(character.CharacterId));
            but.onClick.AddListener(()=> ShowUI());
        }
    }

    private void ChooseCharacter(string id)
    {
        GetCurrentCharacterAndPutInfoBox(id);
    }

    public void AbortCharacter()
    {
        infoCharacter.text = "";
    }
    
    private void ShowUI()
    {
        _whatToShow.SetActive(true);
        foreach (var gameObject in _whatToHide)
        {
            gameObject.SetActive(false);
        }
    }
    
    private void RefreshList()
    {
        foreach (var VARIABLE in CharactersButtons)
        {
            Destroy(VARIABLE);
        }
        
        CharactersButtons.Clear();
    }

    private void GetCharacters()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            result =>
            {
                _characterResult = result;
            }, Debug.LogError );
    }
    
    public void SetCurrentCharacter(string input)
    {
        
        foreach (var character in _characterResult.Characters)
        {
            if (character.CharacterName == input)//_inputNameOfCharacter.text)
            {
                _charactersLocalData.CurrentCharacter = character.CharacterId;
            }
        }
    }
    
    //массовая функция
    public void GetCurrentCharacterAndPutInfoBox(string id)
    {
        _charactersLocalData.CurrentCharacter = id;
        _charactersLocalData.CharacterStatistics.Clear();
        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest()
                    {CharacterId = _charactersLocalData.CurrentCharacter},
                result =>
                {
                    foreach (var characterStatistic in result.CharacterStatistics  )
                    {
                        infoCharacter.text += characterStatistic.Key + ": " + characterStatistic.Value + "\n";
                        _charactersLocalData.CharacterStatistics.Add(characterStatistic.Key,characterStatistic.Value);
                    }
                    PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
                        result => 
                        {
                            foreach (var characterStatistic in result.VirtualCurrency  )
                            {
                                infoCharacter.text += characterStatistic.Key + ": " + characterStatistic.Value + "\n";
                            }
                        },error => Debug.Log("GetUserInventory"));
                },error => Debug.Log("GetCharacterStatistics"));
    }
}