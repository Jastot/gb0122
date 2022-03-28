using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "LocalInventoryGiveData", menuName = "ScriptableObjects/LocalInventoryGiveData", order = 0)]
    public class LocalInventoryGiveData : ScriptableObject
    {
        public List<Item> WhatToGive;
    }
}