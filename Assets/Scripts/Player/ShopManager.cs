using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Transform itemCellHolder;
    [SerializeField] private TextMeshProUGUI shopName;
    public static ShopManager Instance { get; private set; }

    private Dictionary<string, int> boughtItemCounts;
    private List<InventoryItem> storedItems;
    private ShopOpeningAction shopSource;
    private List<InventoryCell> itemCells;

    public bool IsShopOpen { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        itemCells = itemCellHolder.GetComponentsInChildren<InventoryCell>().ToList();
        for (var index = 0; index < itemCells.Count; index++)
        {
            itemCells[index].SetId(index);
        }

        EventManager.OnShopToggled += OnShopToggled;
        EventManager.OnPauseMenuToggled += OnPauseMenuToggled;
    }

    private void OnDestroy()
    {
        EventManager.OnShopToggled -= OnShopToggled;
        EventManager.OnPauseMenuToggled -= OnPauseMenuToggled;
    }

    public void SetShopState(ShopOpeningAction stateSource)
    {
        shopSource = stateSource;
        storedItems = stateSource.StoredItems;
        boughtItemCounts = stateSource.BougthItems;
        shopName.text = stateSource.ShopName;
    }

    private void OnPauseMenuToggled(bool isPaused)
    {
        if (!isPaused) EventManager.OnShopToggled?.Invoke(false);
    }

    private void OnShopToggled(bool isShopOpen)
    {
        IsShopOpen = isShopOpen;
        InventoryController.Instance.UpdateDisplay();
        if (!isShopOpen) return;

        shopSource.CheckBoughtItemCounts();
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        for (var i = 0; i < itemCells.Count; i++)
        {
            if (i < storedItems.Count && storedItems[i].count > 0)
            {
                itemCells[i].UpdateDisplay(storedItems[i], InventoryCellType.Shop);
            }
            else
            {
                itemCells[i].SetNoItemState();
            }
        }
    }

    public void ItemSold(InventoryItem soldItem, int count)
    {
        boughtItemCounts.TryAdd(soldItem.name, 0);
        boughtItemCounts[soldItem.name] += count;

        shopSource.CheckBoughtItemCounts();
    }
}