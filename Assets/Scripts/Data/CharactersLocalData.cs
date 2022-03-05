using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharactersLocalData", order = 1)]
    public class CharactersLocalData: ScriptableObject
    {
        public string CurrentCharacter;
        public Dictionary<string, int> CharacterStatistics = new Dictionary<string, int>();
        public BattleResult BattleResult;
    }
}