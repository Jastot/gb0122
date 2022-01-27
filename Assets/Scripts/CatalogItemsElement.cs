using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class CatalogItemsElement : MonoBehaviour
{
    [SerializeField] private Text itemName;
    [SerializeField] private Text price;
    [SerializeField] private Button _button;
    private int keyType=-1;

    public Button GetButton()
    {
        return _button;
    }
    public void SetItem(CatalogItem item)
    {
        itemName.text = item.DisplayName;
        if (item.VirtualCurrencyPrices.ContainsKey("GC"))
        {
            price.text = item.VirtualCurrencyPrices["GC"].ToString() + " GC";
            keyType = 1;
        }
        if (item.VirtualCurrencyPrices.ContainsKey("FD"))
        {
            price.text = item.VirtualCurrencyPrices["FD"].ToString() + " FD";
            keyType = 2;
        }
        if (item.VirtualCurrencyPrices.ContainsKey("RM"))
        {
            price.text = item.VirtualCurrencyPrices["RM"].ToString() + " $";
            keyType = 3;
        }
    }

    public void Filter(int key)
    {
        if (key!=keyType)
        {
            if (key == 0 && key!=-1)
                this.gameObject.SetActive(true);
            else
                this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }
}
