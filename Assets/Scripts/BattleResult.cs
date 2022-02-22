using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BattleResult", order = 1)]
public class BattleResult : ScriptableObject
{
    public int AggregatedDamage;
}
