using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> ownedItems;
    [SerializeField] private Transform itemCellHolder;
    [SerializeField] private InventoryItem randomItem;
    [SerializeField] private int gold;
    [SerializeField] private int inventorySize;

    private List<InventoryCell> itemCells;

    public static InventoryController Instance { get; private set; }
    public InventoryData OwnedItem => new InventoryData(ownedItems);
    public int Gold => gold;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ownedItems = new List<InventoryItem>();
        itemCells = itemCellHolder.GetComponentsInChildren<InventoryCell>().ToList();
        for (var index = 0; index < itemCells.Count; index++)
        {
            itemCells[index].SetId(index);
        }

        UpdateDisplay();
        EventManager.OnItemAdd += OnItemAdd;
        EventManager.OnItemAddAsLoot += OnItemAddAsLoot;
        EventManager.OnItemRemove += OnItemRemove;
        EventManager.OnItemBought += OnItemBought;
    }


    public void UpdateDisplay()
    {
        for (var i = 0; i < itemCells.Count; i++)
        {
            if (i < ownedItems.Count)
            {
                itemCells[i].UpdateDisplay(ownedItems[i], InventoryCellType.Bag);
            }
            else
            {
                itemCells[i].SetNoItemState();
            }
        }
    }

    private void OnItemAdd(InventoryItem addedItem)
    {
        var inventoryItem = ownedItems.FirstOrDefault(ownedItem => ownedItem.name == addedItem.name);

        while (addedItem.count > 0 && ownedItems.Count < inventorySize)
        {
            if (inventoryItem == null || inventoryItem.count == inventoryItem.stackSize)
            {
                inventoryItem = Instantiate(addedItem, transform);
                inventoryItem.count = 0;
                ownedItems.Add(inventoryItem);
            }

            var canAccept = Mathf.Min(inventoryItem.stackSize - inventoryItem.count, addedItem.count);
            inventoryItem.count += canAccept;
            addedItem.count -= canAccept;
        }

        UpdateDisplay();
    }


    private void OnItemAddAsLoot(InventoryItem item, LootItemPanel panel)
    {
        OnItemAdd(item);
        Destroy(panel.gameObject);
    }

    private void OnItemBought(InventoryItem boughtItem)
    {
        if (boughtItem.buyPrice > Gold) return;

        var prevCount = boughtItem.count;
        boughtItem.count = 1;

        OnItemAdd(boughtItem);

        var boughtItemCount = 1 - boughtItem.count;
        boughtItem.count = prevCount - boughtItemCount;
        if (boughtItemCount > 0)
        {
            AddGold(-boughtItem.buyPrice * boughtItemCount);
            ShopManager.Instance.UpdateDisplay();
        }
    }

    private void OnItemRemove(InventoryItem removedItem)
    {
        removedItem.count -= 1;

        if (removedItem.count <= 0)
        {
            ownedItems.Remove(removedItem);
        }

        UpdateDisplay();
    }

    [ContextMenu("Add random item")]
    public void AddRandomItem()
    {
        var inventoryItem = Instantiate(randomItem);
        EventManager.OnItemAdd?.Invoke(inventoryItem);
    }

    [ContextMenu("Remove random item")]
    public void RemoveRandomItem()
    {
        EventManager.OnItemRemove?.Invoke(randomItem);
    }

    public void ItemCellClicked(InventoryItem clickedItem, InventoryCellType cellType)
    {
        //TODO change this

        switch (cellType)
        {
            case InventoryCellType.Shop:
                OnItemBought(clickedItem);
                break;
            default:
                print("Using item");
                clickedItem.Use();
                EventManager.OnItemRemove?.Invoke(clickedItem);
                break;
        }
    }

    public void LoadData(InventoryData inventoryData)
    {
        this.ownedItems = inventoryData.ownedItems;
        UpdateDisplay();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        EventManager.OnPlayerCoreUpdate?.Invoke(gold);
    }
}


[Serializable]
public class InventoryData
{
    public List<InventoryItem> ownedItems;

    public InventoryData(List<InventoryItem> ownedItems)
    {
        this.ownedItems = ownedItems;
    }
}