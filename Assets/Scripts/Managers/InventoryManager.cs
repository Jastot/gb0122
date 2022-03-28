using System.Collections.Generic;
using CreatorKitCode;
using Data;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private CatalogItemsElement _element;
    [SerializeField] private Transform _parent;
    [SerializeField] private LocalInventoryGiveData _data;
    [SerializeField] private UsefulPlayerInfoForShop _shop;
    private Dictionary<CatalogItemsElement,ItemInstance> _inventoryItems;
    
    private void Start()
    {
        _inventoryItems = new Dictionary<CatalogItemsElement,ItemInstance>();
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            HandleInventory(result.Inventory);
        }, Debug.LogError);
    }

    private void HandleInventory(List<ItemInstance> items)
    {
        foreach (var item in items)
        {
            var element = Instantiate(_element, _parent);
            element.gameObject.SetActive(true);
            element.SetItem(item);
            _inventoryItems.Add(element,item);
        }
    }

    public void GetItemsToPlay()
    {
        foreach (var catalogItemsElement in _inventoryItems)
        {
            if (catalogItemsElement.Key.gameObject.GetComponentInChildren<Toggle>(true).isOn)
            {
                
                PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest()
                    {
                        ItemInstanceId = catalogItemsElement.Value.ItemInstanceId,
                        AuthenticationContext = _shop.AuthenticationContext,
                        ConsumeCount = catalogItemsElement.Key._count
                    }, 
                    result =>
                {
                    Debug.Log("RemainingUses: "+result.RemainingUses);
                    for (int i = 0; i < catalogItemsElement.Key._count; i++)
                        _data.WhatToGive.Add(Resources.Load<Item>("ItemDatabase/"+catalogItemsElement.Key.GetInventoryItem().ItemId));
                }, error =>
                {
                    Debug.Log("nope");
                });
            }
        }
    }
}