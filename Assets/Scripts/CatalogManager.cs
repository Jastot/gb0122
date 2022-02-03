using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class CatalogManager : MonoBehaviour
{
    [SerializeField] private CatalogItemsElement _element;
    [SerializeField] private Transform parent;
    
    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();
    private int _clickCounter;
    public event Action<CatalogItem> _buttonWasPressed;
    
    private void Start()
    {
        _clickCounter = 0;
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), result =>
        {
            //Debug.Log("Get Catalog Items Success");
            HandleCatalog(result.Catalog);
        }, error =>
        {
            //Debug.LogError($"Get Catalog Items Failed: {error}");
        });
    }

    private void HandleCatalog(List<CatalogItem> catalog)
    {
        foreach (var item in catalog)
        {
            _catalog.Add(item.ItemId, item);
            //Debug.Log($"Item with ID {item.ItemId} was added to dictionary");
            var element = Instantiate(_element, parent);
            element.gameObject.SetActive(true);
            element.SetItem(item);
            //element.GetButton().onClick.AddListener(()=>ButtonWasPressed(item));
        }
    }

    private void ButtonWasPressed(CatalogItem catalogItem)
    {
        _buttonWasPressed.Invoke(catalogItem);
    }
    public void Filter()
    {
        var uiItems = FindObjectsOfType<CatalogItemsElement>(true);
        foreach (var item in uiItems)
        {
            item.Filter(_clickCounter);
        }
        if (_clickCounter==3)
            _clickCounter = 0;
        else
            _clickCounter++;
    }
}
