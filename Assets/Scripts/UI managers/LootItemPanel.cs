using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootItemPanel : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI countText;

    public void UpdateDisplay(InventoryItem item)
    {
        var itemIconColor = itemIcon.color;
        itemIconColor.a = 1;
        itemIcon.color = itemIconColor;
        itemIcon.sprite = item.icon;
        countText.text = "x" + item.count;
        countText.alpha = 1;
        descText.text = $"{item.name}";
    }
}