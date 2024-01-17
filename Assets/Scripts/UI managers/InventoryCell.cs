using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject descGm;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image innerPart;
    [SerializeField] private Color[] innerColors;
    [SerializeField] private InventoryCellType type;
    public int id;
    public InventoryItem storedItem;
    private Coroutine clickCoro;

    private void Start()
    {
        if (storedItem == null && type != InventoryCellType.Equipment)
            SetNoItemState();
    }

    public void SetNoItemState()
    {
        storedItem = null;
        var itemIconColor = itemIcon.color;
        itemIconColor.a = 0;
        itemIcon.color = itemIconColor;
        descGm.SetActive(false);
        countText.alpha = 0;
        priceText.alpha = 0;
    }

    [ContextMenu("Set color")]
    public void SetColor()
    {
        if (name.Contains("("))
        {
            var substring = name.Substring("InventoryCell (".Length);
            var index = (int.Parse(substring.Remove(substring.Length - 1, 1)) % 9 / 3);
            innerPart.color = innerColors[index];
        }
    }

    public void UpdateDisplay(InventoryItem item, InventoryCellType inventoryCellType)
    {
        storedItem = item;
        type = inventoryCellType;

        var itemIconColor = itemIcon.color;
        itemIconColor.a = 1;
        itemIcon.color = itemIconColor;
        itemIcon.sprite = item.icon;

        countText.text = "x" + item.count;
        countText.alpha = 1;

        priceText.text = (type == InventoryCellType.Shop ? item.buyPrice : item.sellPrice) + "g";
        switch (type)
        {
            case InventoryCellType.Shop:
                priceText.alpha = 1;
                break;
            case InventoryCellType.Bag:
                if (ShopManager.Instance.IsShopOpen && item.canBeSold)
                    priceText.alpha = 1;
                else
                    priceText.alpha = 0;
                break;
            case InventoryCellType.Equipment:
            default:
                countText.alpha = 0;
                priceText.alpha = 0;
                break;
        }
    }

    public void SetId(int newId)
    {
        id = newId;
    }

    public void ClickStarted()
    {
        if (clickCoro == null)
        {
            SendClick();
            clickCoro = StartCoroutine(ClickRegular());
        }
    }

    public void ClickStopped()
    {
        if (clickCoro != null)
        {
            StopCoroutine(clickCoro);
            clickCoro = null;
        }
    }

    private IEnumerator ClickRegular()
    {
        while (storedItem != null)
        {
            yield return new WaitForSeconds(0.2f);
            SendClick();
        }

        clickCoro = null;
    }

    private void SendClick()
    {
        if (storedItem == null) return;
        InventoryController.Instance.ItemCellClicked(storedItem, type);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (storedItem != null) ItemDescriptionManager.InventoryInstance.DisplayItem(storedItem);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (storedItem != null) ItemDescriptionManager.InventoryInstance.DisplayItem(storedItem);
    }
}

public enum InventoryCellType
{
    Bag,
    Shop,
    Equipment
}