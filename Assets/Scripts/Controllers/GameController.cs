using System;
using System.Collections;
using System.Collections.Generic;
using CreatorKitCodeInternal;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //тут нужно заинитить стартовое UI и отслеживание конца игры
    [SerializeField] private UISystem _uiSystem;
    [SerializeField] private BattleResult result;
    void Start()
    {
     
    }

    
    void Update()
    {
        
    }

    public void OnDestroy()
    {
        throw new NotImplementedException();
    }
}
