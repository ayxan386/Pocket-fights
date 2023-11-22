using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> storedItems;
    [SerializeField] private Transform itemCellHolder;
    [SerializeField] private ShopItemsData shopItemsData;

    [Header("Refresh logic")] [SerializeField]
    private int refreshCounter;

    [SerializeField] private TextMeshProUGUI refreshCounterText;

    private List<InventoryCell> itemCells;
    private int currentCounter;

    public static ShopManager Instance { get; private set; }
    private Dictionary<string, int> boughtItemCounts;
    public bool IsShopOpen { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshShop();
        itemCells = itemCellHolder.GetComponentsInChildren<InventoryCell>().ToList();
        for (var index = 0; index < itemCells.Count; index++)
        {
            itemCells[index].SetId(index);
        }

        EventManager.OnShopToggled += OnShopToggled;
        EventManager.OnPauseMenuToggled += OnPauseMenuToggled;
        EventManager.OnMobDeath += OnMobDeath;
        currentCounter = refreshCounter;
    }

    private void RefreshShop()
    {
        storedItems = shopItemsData.GenerateShopData(transform);
        boughtItemCounts = new Dictionary<string, int>();
        currentCounter = refreshCounter;
        CheckBoughtItemCounts();
    }

    private void OnMobDeath(MobController obj)
    {
        UpdateRefreshCounter(-1);
    }

    private void UpdateRefreshCounter(int diff)
    {
        currentCounter += diff;
        if (currentCounter <= 0)
        {
            RefreshShop();
        }

        UpdateDisplay();
    }

    private void OnPauseMenuToggled(bool isPaused)
    {
        if (!isPaused) EventManager.OnShopToggled?.Invoke(false);
    }

    private void OnShopToggled(bool isShopOpen)
    {
        IsShopOpen = isShopOpen;
        if (!isShopOpen) return;

        CheckBoughtItemCounts();
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

        refreshCounterText.text = "Kill: " + currentCounter;
    }

    public void ItemSold(InventoryItem soldItem, int count)
    {
        boughtItemCounts.TryAdd(soldItem.name, 0);
        boughtItemCounts[soldItem.name] += count;

        CheckBoughtItemCounts();
    }

    private void CheckBoughtItemCounts()
    {
        foreach (var inventoryItem in InventoryController.Instance.OwnedItems)
        {
            boughtItemCounts.TryGetValue(inventoryItem.name, out var count);
            inventoryItem.sellPrice = (int)Mathf.Floor(inventoryItem.maxSellPrice *
                                                       Mathf.Pow(inventoryItem.priceDropRate,
                                                           count / inventoryItem.stackSize));
        }
    }

    public int GetPrice(InventoryItem inventoryItem)
    {
        boughtItemCounts.TryGetValue(inventoryItem.name, out var count);
        return (int)Mathf.Floor(inventoryItem.maxSellPrice *
                                Mathf.Pow(inventoryItem.priceDropRate,
                                    count / inventoryItem.stackSize));
    }
}