using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreatorKitCodeInternal;
using Data;
using UnityEngine;

public class InventoryStartManager : MonoBehaviour
{
    [SerializeField] private LocalInventoryGiveData _data;
    
    void Start()
    {
        foreach (var characterControl in FindObjectsOfType<MonoBehaviour>().OfType<CharacterControl>().ToList())
        {
            if (characterControl.photonView.IsMine)
            {
                foreach (var dataItem in _data.WhatToGive)
                {
                    characterControl.Data.Inventory.AddItem(dataItem);
                }
                break;
            }
        }
    }
}
