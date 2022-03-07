using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Spawners
{
    public class ShopSpawner: MonoBehaviour
    {
        [SerializeField] private List<EnemiesSpawnerPoint> _spawnerPoint;
        [SerializeField] private string prefab;
        private void Start()
        {
            int index = _spawnerPoint.Count;
            for (int i = 0; i < index; i++)
            {
                SpawnGameShop(index);
            }
        }

        private void SpawnGameShop(int index)
        {
            var go = PhotonNetwork.InstantiateRoomObject(prefab,_spawnerPoint[index].transform.position ,
                _spawnerPoint[index].transform.rotation  ,0);
        }
    }
}