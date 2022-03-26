using PlayFab;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PlayerInfo", menuName = "ScriptableObjects/UsefulPlayerInfoForShop", order = 0)]
    public class UsefulPlayerInfoForShop : ScriptableObject
    {
        public PlayFabAuthenticationContext AuthenticationContext;
    }
}