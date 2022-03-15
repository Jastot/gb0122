using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    [Serializable]
    public class ItemToSpawnElement
    {
        public string ItemsTypes;
        public int ItemsCount;
        public List<Transform> WherePlaceIt;
    }
}