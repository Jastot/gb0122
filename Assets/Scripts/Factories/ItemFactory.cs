using System.Collections.Generic;
using CreatorKitCode;
using Photon.Pun;
using UnityEngine;

namespace Factories
{
    public class ItemFactory
    {
        public GameObject SpawnItem(string whatToSpawn,Transform whereToPlace)
        {
            return GameObject.Instantiate(Resources.Load(whatToSpawn) as GameObject,whereToPlace.position ,
                whereToPlace.rotation);
        }
    }
}