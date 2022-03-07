using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using View;

namespace Controllers
{
    public class GameShopController: MonoBehaviour
    {
        [SerializeField] private CatalogItemsElement _element;
        [SerializeField] private Transform UIParent;
        [SerializeField] private TimeController _timeController;
        [SerializeField] private EnemySpawner _enemySpawner;
        private List<ShopView> _shopViews;
        private List<CatalogItemsElement> _shopItemList;
        private float IsCountOfEnemyLowCoefficient = 1f;
        
        private void Start()
        {
            _shopItemList = new List<CatalogItemsElement>();
            _shopViews = GetComponentsInChildren<ShopView>().ToList();
            foreach (var shop in _shopViews)
            {
                shop.ActivateStoreEvent += ActivateStore;
                shop.DeactivateStoreEvent += DeactivateStore;
            }

            _enemySpawner.CountOfEnemyChanged += SetIsCountOfEnemyLowCoefficient;
            _timeController.GiveDateTime += AllShopsEconomicControl;
        }

        private void OnDestroy()
        {
            foreach (var shop in _shopViews)
            {
                shop.ActivateStoreEvent -= ActivateStore;
                shop.DeactivateStoreEvent -= DeactivateStore;
            }
            _enemySpawner.CountOfEnemyChanged -= SetIsCountOfEnemyLowCoefficient;
            _timeController.GiveDateTime -= AllShopsEconomicControl;
        }

        //"items_store"
        private void ActivateStore(string StoreID, ShopView shopView)
        {
            PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
            {
                StoreId = StoreID
            }, result =>
            {
                HandleStore(result.Store);
                AddMoreGreedForItemInShop(shopView);
                UIParent.gameObject.SetActive(true);
            }, Debug.LogError);
        }

        public void DeactivateStore()
        {
            _shopItemList.Clear();
            UIParent.gameObject.SetActive(false);
        }

        private void ClearUI()
        {
            
        }
        
        private void HandleStore(List<StoreItem> store)
        {
            foreach (var item in store)
            {
                var element = Instantiate(_element, UIParent.transform);
                element.gameObject.SetActive(true);
                element.SetItem(item);
                _shopItemList.Add(element);
            }
        }

        private void AllShopsEconomicControl(int economicCoefficient)
        {
            foreach (var shop in _shopViews)
            {
                shop.SetEconomicCoefficient(economicCoefficient);
            }
        }
        

        private void SetIsCountOfEnemyLowCoefficient(int count)
        {
            if (count  / _enemySpawner.StartCountOfEnemies <= _enemySpawner.StartCountOfEnemies*3/4) 
            {
                if (count  / _enemySpawner.StartCountOfEnemies <= _enemySpawner.StartCountOfEnemies*2/4) 
                {
                    if (count  / _enemySpawner.StartCountOfEnemies <= _enemySpawner.StartCountOfEnemies/4)
                    {
                        IsCountOfEnemyLowCoefficient = 2.4f;
                        return;
                    }
                    IsCountOfEnemyLowCoefficient = 2f;
                    return;
                }
                IsCountOfEnemyLowCoefficient = 1.4f;
                return;
            }
            else
            {
                IsCountOfEnemyLowCoefficient = 1f;
            }
        }
        
        private void AddMoreGreedForItemInShop(ShopView shopView)
        {
            foreach (var catalog in _shopItemList)
            {
                catalog.ConvertPrice(IsCountOfEnemyLowCoefficient);
                catalog.ConvertPrice(shopView.GetIsCustomerDyingCoefficient());
            }
        }
    }
}