using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayersMoneyManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private CatalogManager _catalogManager;
        private Dictionary<string, int> _balance;
        private void Start()
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
                result =>
                {
                    _balance = result.VirtualCurrency;
                    ShowBalance(result.VirtualCurrency);
                },
                error =>
                {
                    Debug.LogError($"GetUserDataRequest Failed: {error}");
                }
                );
            _catalogManager._buttonWasPressed += BuySomeStaff;
        }

        private void OnDestroy()
        {
            _catalogManager._buttonWasPressed -= BuySomeStaff;
        }

        private void ShowBalance(Dictionary<string,int> balance)
        {
            foreach (var currency in balance)
            {
                _textMeshProUGUI.text += $"{currency.Key}: {currency.Value}\n";
            }
        }

        private void BuySomeStaff(CatalogItem catalogItem)
        {
            uint price;
            if (catalogItem.VirtualCurrencyPrices.ContainsKey("GC"))
            {
                price = catalogItem.VirtualCurrencyPrices["GC"];
                if (_balance["GC"] >= price)
                {
                    Debug.Log("SOLD");
                }
                else
                {
                    Debug.Log("You need GC");
                }
            }
            if (catalogItem.VirtualCurrencyPrices.ContainsKey("FD"))
            {
                price = catalogItem.VirtualCurrencyPrices["FD"];
                if (_balance["FD"] >= price)
                {
                    Debug.Log("SOLD");
                }
                else
                {
                    Debug.Log("You need FD");
                }
            }
        }
    }
    
}