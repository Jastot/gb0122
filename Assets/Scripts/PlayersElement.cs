using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayersElement : MonoBehaviour
{
    [SerializeField] private Text itemName;
    
    public void SetItem(Player item)
    {
        itemName.text = item.UserId;
    }
}
