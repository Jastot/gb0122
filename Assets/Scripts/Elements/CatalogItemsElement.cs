using System;
using Controllers;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class CatalogItemsElement : MonoBehaviour
{
    [SerializeField] private Text itemName;
    [SerializeField] private Text price;

    private float _price;
    private StoreItem _item;

    public event Action<Tuple<int,float>> BuyItem;
    
    public void SetItem(CatalogItem item)
    {
        itemName.text = item.DisplayName;
        if (item.VirtualCurrencyPrices.ContainsKey("GC"))
        {
            price.text = item.VirtualCurrencyPrices["GC"].ToString();
        }
    }
    
    public void SetItem(StoreItem item)
    {
        _item = item;
        itemName.text = item.ItemId;
        if (item.VirtualCurrencyPrices.ContainsKey("GC"))
        {
            _price = item.VirtualCurrencyPrices["GC"];
            price.text = item.VirtualCurrencyPrices["GC"].ToString();
        }
    }
    
    public void SetItem(ItemInstance item)
    {
        itemName.text = item.DisplayName;
    }
    
    public void ConvertPrice(float coefficient)
    {
        _price *= coefficient;
        price.text = _price.ToString();
    }
    
    public void MakePurchase()
    {
        //TODO: Заглушить псевдо-покупку в инвентаре
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
                ItemId = _item.ItemId,
                Price = Convert.ToInt32(_item.VirtualCurrencyPrices["GC"]),
                VirtualCurrency = "GC"
        }, 
            result => { 
                Tuple<int, float> returnInfo = new Tuple<int, float>(
                Convert.ToInt32(_item.VirtualCurrencyPrices["GC"]),
                _price);
                BuyItem?.Invoke(returnInfo);
            },
            Debug.LogError);
    }
}
