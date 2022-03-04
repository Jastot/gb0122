using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProfileSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameObject baseUIComp;
    private List<BaseUIElemntWithBackButton> UI;
    private Stack<int> _indexOfLastPoss;
    
    private void Start()
    {
        _indexOfLastPoss = new Stack<int>();
        UI = baseUIComp.GetComponents<BaseUIElemntWithBackButton>().ToList();
        foreach (var baseUIElementWithBackButton in UI)
        {
            baseUIElementWithBackButton.BackButton.onClick.AddListener(
                ()=>GoBack(baseUIElementWithBackButton.index));
            if (baseUIElementWithBackButton.ActivatorButton != null)
                baseUIElementWithBackButton.ActivatorButton.onClick.AddListener(
                ()=>AddInStack(baseUIElementWithBackButton.index));
        }
    }

    private void GoBack(int index)
    {
        if (index != _indexOfLastPoss.Peek())
        {
            Debug.Log(index+ " "+ _indexOfLastPoss.Peek()); 
            return;
        }
        foreach (var gameObject in UI[_indexOfLastPoss.Peek()].WhatToShow)
        {
            gameObject.SetActive(true);
        }
        foreach (var gameObject in UI[_indexOfLastPoss.Peek()].WhatToUnShow)
        {
            gameObject.SetActive(false);
        }
        PeekFromStack();
    }

    private void AddInStack(int index)
    {
        if (!_indexOfLastPoss.Contains(index))
            _indexOfLastPoss.Push(index);
    }

    public void ClearStack()
    {
        _indexOfLastPoss.Clear();
    }

    private void PeekFromStack()
    {
        _indexOfLastPoss.Pop();
    }
    
    private void OnDestroy()
    {
        foreach (var baseUIElementWithBackButton in UI)
        {
            baseUIElementWithBackButton.BackButton.onClick.RemoveAllListeners();
            if(baseUIElementWithBackButton.ActivatorButton!=null)
                baseUIElementWithBackButton.ActivatorButton.onClick.RemoveAllListeners();
        }
    }
}
