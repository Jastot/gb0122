using System;
using System.Collections.Generic;
using Controllers;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatalogItemsElement : MonoBehaviour
{
    [SerializeField] private Text itemName;
    [SerializeField] private Text price;
    
    [Header("For inventory")]
    [SerializeField] private TMP_Text _text;
    
    private float _price;
    private StoreItem _item;
    private ItemInstance _itemInventory;
    private int? _countOfUse;
    [NonSerialized]
    public int _count = 1;
    public event Action<Tuple<int,float>> BuyItem;
    public event Action<StoreItem> GiveAnItem;
    
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
        _itemInventory = item;
        _countOfUse = item.RemainingUses;
        _text.text = _count.ToString();
    }

    public ItemInstance GetInventoryItem()
    {
        return _itemInventory;
    }
    
    public void ConvertPrice(float coefficient)
    {
        _price *= coefficient;
        price.text = _price.ToString();
    }

    public void MakePurchaseAndPutInInventory()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
        {
            ItemId = _item.ItemId,
            Price = Convert.ToInt32(_item.VirtualCurrencyPrices["GC"]),
            VirtualCurrency = "GC"
        }, result =>
        {
            Tuple<int, float> returnInfo = new Tuple<int, float>(
                Convert.ToInt32(_item.VirtualCurrencyPrices["GC"]),
                _price);
            BuyItem?.Invoke(returnInfo);
            GiveAnItem?.Invoke(_item);
        }, error =>
        {
            
        });
    }

    public void AddMoreToTake()
    {
        if (_count < _countOfUse)
        {
            _count++;
            _text.text = _count.ToString();
        }
    }

    public void LessToTake()
    {
        if (_count > 1)
        {
            _count--;
            _text.text = _count.ToString();
        }
    }

    public void MakePurchase()
    { 
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
