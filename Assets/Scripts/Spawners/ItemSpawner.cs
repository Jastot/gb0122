using System;
using System.Collections.Generic;
using Elements;
using Factories;
using Photon.Pun;
using UnityEngine;


namespace Spawners
{
    public class ItemSpawner: MonoBehaviour
    {
        public List<ItemToSpawnElement> ItemToSpawnElements;
        private ItemFactory _itemFactory;
        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _itemFactory = new ItemFactory();
                foreach (var itemsType in ItemToSpawnElements)
                {
                    for (int i = 0; i < itemsType.ItemsCount; i++)
                        _itemFactory.SpawnItem(itemsType.ItemsTypes,itemsType.WherePlaceIt[i]);
                }
            }
        }
    }
}