using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharactersLocalData", order = 1)]
    public class CharactersLocalData: ScriptableObject
    {
        public string CurrentCharacter;
        public BattleResult BattleResult;
    }
}