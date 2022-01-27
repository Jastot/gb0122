using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class CatalogManager : MonoBehaviour
{
    [SerializeField] private CatalogItemsElement _element;
    
    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();

    private void Start()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), result =>
        {
            Debug.Log("Get Catalog Items Success");
            HandleCatalog(result.Catalog);
        }, error =>
        {
            Debug.LogError($"Get Catalog Items Failed: {error}");
        });
    }

    private void HandleCatalog(List<CatalogItem> catalog)
    {
        foreach (var item in catalog)
        {
            _catalog.Add(item.ItemId, item);
            Debug.Log($"Item with ID {item.ItemId} was added to dictionary");
            var element = Instantiate(_element, _element.transform.parent);
            element.gameObject.SetActive(true);
            element.SetItem(item);
        }
    }
}
