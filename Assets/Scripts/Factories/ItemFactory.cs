using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Factories
{
    public class ItemFactory
    {
        public void SpawnItem(string whatToSpawn,Transform whereToPlace)
        {
            var go = PhotonNetwork.InstantiateRoomObject(whatToSpawn,whereToPlace.position ,
                whereToPlace.rotation  ,0);
        }
    }
}