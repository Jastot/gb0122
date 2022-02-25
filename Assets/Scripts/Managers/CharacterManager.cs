using System.Collections.Generic;
using System.Linq;
using Data;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    private const string CharactersStoreId = "characters_store";
    private const string VirtualCurrencyKey = "GC";

    // [SerializeField] private GameObject createPanel;
    // [SerializeField] private GameObject plusPanel;
    //
    // [SerializeField] private GameObject plus1;
    // [SerializeField] private GameObject character1;
    // [SerializeField] private Text characterName1;
    // [SerializeField] private Text characterLvl1;
    // [SerializeField] private Text characterName2;
    // [SerializeField] private Text characterLvl2;
    //[SerializeField] private GameObject plusPanel;
    
    [SerializeField] private InputField _inputNameOfCharacter;
    [SerializeField] private TMP_Text infoCharacter;
    [SerializeField]
    private List<GameObject> CharactersButtons;
    
    //[SerializeField] private BattleResult _battleResult;
    [SerializeField] private CharactersLocalData _charactersLocalData;
    [SerializeField] private Transform _parent;
    
    private string _inputFieldText;
    private GameObject _characterPrefab;
    
    private void Start()
    {
        _characterPrefab = Resources.Load<GameObject>("ChooseCharacterButton");
        CharactersButtons = new List<GameObject>();
        UpdateCharacters();
        if (_charactersLocalData.BattleResult.AggregatedDamage > 0)
        {
            UpdateCharacterAfterBattle();
        }
    }

    public void OnNameChanged(string changedName)
    {
        _inputFieldText = changedName;
    }

    public void OnCreatButtonClicked()
    {
        if (string.IsNullOrEmpty(_inputFieldText))
        {
            Debug.LogError("Ïnput field should not be empty");
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
            CharacterName = _inputFieldText
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

    private void UpdateCharacters()
    {
        RefreshList();
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            result =>
            {
                foreach (var character in result.Characters)
                {
                    CharactersButtons.Add(Instantiate(_characterPrefab, _parent));
                    var texts = CharactersButtons.Last().GetComponents<Text>();
                    texts[0].text = character.CharacterName;
                    PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest()
                            {CharacterId = _charactersLocalData.CurrentCharacter},
                        result =>
                        {
                            texts[1].text = result.CharacterStatistics["Level"].ToString();
                        },Debug.LogError);
                    
                }
                
                // for (int i = 0; i != 2 && i != result.Characters.Count; ++i)
                // {
                //     var characterName = result.Characters[i].CharacterName;
                //     PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
                //     {
                //         CharacterId = result.Characters[i].CharacterId
                //     }, res =>
                //     {
                //         chosenCharacterName.text = characterName + "\n" + res.CharacterStatistics["Level"] + "\n";
                //     }, Debug.LogError);
                // }
                //plusPanel.SetActive(true);
            }, Debug.LogError);
    }

    private void RefreshList()
    {
        foreach (var VARIABLE in CharactersButtons)
        {
            Destroy(VARIABLE);
        }
        
        CharactersButtons.Clear();
    }
    
    private void UpdateCharacterAfterBattle()
    {
        // PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
        //     result =>
        //     {
        //         for (int i = 0; i != 2 && i != result.Characters.Count; ++i)
        //         {
        //             PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        //             {
        //                 CharacterId = result.Characters[i].CharacterId,
        //                 CharacterStatistics = new Dictionary<string, int>
        //                 {
        //                     {"Exp", _charactersLocalData.BattleResult.AggregatedDamage}
        //                 }
        //             }, result =>
        //             {
        //                 _charactersLocalData.BattleResult.AggregatedDamage = 0;
        //             }, Debug.LogError);
        //         }
        //     }, Debug.LogError);
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest()
        {
            CharacterId = _charactersLocalData.CurrentCharacter,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"Exp", _charactersLocalData.BattleResult.AggregatedDamage}
            }
        }, result =>
        {
            _charactersLocalData.BattleResult.AggregatedDamage = 0;
        }, Debug.LogError);
    }

    public void SetCurrentCharacter()
    {
        _charactersLocalData.CurrentCharacter = _inputNameOfCharacter.text;
    }
    
    //массовая функция
    public void GetCurrentCharacterAndPutInfoBox()
    { 
        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest()
                    {CharacterId = _charactersLocalData.CurrentCharacter},
                result => 
                {
                    foreach (var characterStatistic in result.CharacterStatistics  )
                    {
                        infoCharacter.text += characterStatistic.Key + ": " + characterStatistic.Value + "\n";
                    }
                },Debug.LogError);
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result => 
        {
            foreach (var characterStatistic in result.VirtualCurrency  )
            {
                infoCharacter.text += characterStatistic.Key + ": " + characterStatistic.Value + "\n";
            }
        },Debug.LogError);
    }
}