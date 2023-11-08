using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootItemPanel : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI countText;
    private InventoryItem storedItem;

    public void UpdateDisplay(InventoryItem item)
    {
        storedItem = item;
        var itemIconColor = itemIcon.color;
        itemIconColor.a = 1;
        itemIcon.color = itemIconColor;
        itemIcon.sprite = item.icon;
        countText.text = "x" + item.count;
        countText.alpha = 1;
        descText.text = $"{item.name}";
    }

    public void OnClick()
    {
        EventManager.OnItemAddAsLoot?.Invoke(storedItem, this);
    }
}