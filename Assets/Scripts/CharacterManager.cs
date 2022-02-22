using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    private const string CharactersStoreId = "characters_store";
    private const string VirtualCurrencyKey = "GD";

    [SerializeField] private GameObject createPanel;
    [SerializeField] private GameObject plusPanel;
    
    [SerializeField] private GameObject plus1;
    [SerializeField] private GameObject character1;
    [SerializeField] private Text characterName1;
    [SerializeField] private Text characterLvl1;
    [SerializeField] private Text characterName2;
    [SerializeField] private Text characterLvl2;
    
    [SerializeField] private BattleResult _battleResult;
    
    private string _inputFieldText;
    
    private void Start()
    {
        UpdateCharacters();
        if (_battleResult.AggregatedDamage > 0)
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
            createPanel.SetActive(false);
            UpdateCharacters();
        }, Debug.LogError);
    }

    private void UpdateCharacters()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            result =>
            {
                for (int i = 0; i != 2 && i != result.Characters.Count; ++i)
                {
                    var characterName = result.Characters[i].CharacterName;
                    PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
                    {
                        CharacterId = result.Characters[i].CharacterId
                    }, res =>
                    {
                        plus1.SetActive(false);
                        character1.SetActive(true);
                        characterName1.text = characterName;
                        characterLvl1.text = res.CharacterStatistics["Level"].ToString();
                    }, Debug.LogError);
                }
                plusPanel.SetActive(true);
            }, Debug.LogError);
    }

    private void UpdateCharacterAfterBattle()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            result =>
            {
                for (int i = 0; i != 2 && i != result.Characters.Count; ++i)
                {
                    PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
                    {
                        CharacterId = result.Characters[i].CharacterId,
                        CharacterStatistics = new Dictionary<string, int>
                        {
                            {"Exp", _battleResult.AggregatedDamage}
                        }
                    }, result =>
                    {
                        _battleResult.AggregatedDamage = 0;
                    }, Debug.LogError);
                }
            }, Debug.LogError);
    }
}