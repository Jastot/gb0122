using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSceneUIManager : MonoBehaviour
{
    [SerializeField] private List<BaseUIElemntWithBackButton> UI;
    private Queue<int> _indexOfLastPoss;
    
    private void Start()
    {
        foreach (var baseUIElemntWithBackButton in UI)
        {
            foreach (var button in baseUIElemntWithBackButton.BackButton)
            {
                button.onClick.AddListener(GoBack);
            }
        }
    }

    private void GoBack()
    {
        
        
    }
    
    

    private void AddInQueue()
    {
        _indexOfLastPoss.Enqueue(0);
    }

    private int PeekFromQueue()
    {
       return _indexOfLastPoss.Dequeue();
    }
    
    private void OnDestroy()
    {
        foreach (var baseUIElemntWithBackButton in UI)
        {
            foreach (var button in baseUIElemntWithBackButton.BackButton)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }
}
