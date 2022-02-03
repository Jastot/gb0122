using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyElement : MonoBehaviour
{
    [SerializeField] private Text itemName;
    [SerializeField] private Button _button;

    public event Action<string> NeedToJoinRoom;
    
    private void Start()
    {
        _button.onClick.AddListener(Join);
    }

    public void SetItem(string name)
    {
        itemName.text = name;
    }

    private void Join()
    {
        if (itemName.text!=""||itemName.text!=null)
        {
          NeedToJoinRoom.Invoke(itemName.text);  
        }
        
    }
}
