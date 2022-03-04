using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUIElemntWithBackButton : MonoBehaviour
{
    public int index;
    public Button BackButton;
    public Button ActivatorButton;
    public List<GameObject> WhatToShow;
    public List<GameObject> WhatToUnShow;
}
