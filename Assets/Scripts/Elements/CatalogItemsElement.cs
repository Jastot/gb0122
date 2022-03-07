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

    public void SetItem(CatalogItem item)
    {
        itemName.text = item.DisplayName;
        if (item.VirtualCurrencyPrices.ContainsKey("GD"))
        {
            price.text = item.VirtualCurrencyPrices["GD"].ToString();
        }
    }
    
    public void SetItem(StoreItem item)
    {
        itemName.text = item.ItemId;
        if (item.VirtualCurrencyPrices.ContainsKey("GD"))
        {
            _price = item.VirtualCurrencyPrices["GD"];
            price.text = item.VirtualCurrencyPrices["GD"].ToString();
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
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            ItemId = "sw_cnt_key",
            Price = 10,
            VirtualCurrency = "GD"
        }, result => { }, Debug.LogError);
    }
}
