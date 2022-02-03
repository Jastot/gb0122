using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private CatalogItemsElement _element;
    
    private void Start()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            HandleInventory(result.Inventory);
        }, Debug.LogError);
    }

    private void HandleInventory(List<ItemInstance> items)
    {
        foreach (var item in items)
        {
            var element = Instantiate(_element, _element.transform.parent);
            element.gameObject.SetActive(true);
            element.SetItem(item);
        }
    }
}
