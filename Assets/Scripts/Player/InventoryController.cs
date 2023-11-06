using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> ownedItems;
    [SerializeField] private Transform itemCellHolder;
    [SerializeField] private InventoryItem randomItem;

    private List<InventoryCell> itemCells;
    
    public static InventoryController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ownedItems = new List<InventoryItem>();
        itemCells = itemCellHolder.GetComponentsInChildren<InventoryCell>().ToList();
        UpdateDisplay();
        EventManager.OnItemAdd += OnItemAdd;
    }

    public void UpdateDisplay()
    {
        for (var i = 0; i < itemCells.Count; i++)
        {
            if (i < ownedItems.Count)
            {
                itemCells[i].UpdateDisplay(ownedItems[i]);
            }
            else
            {
                itemCells[i].SetNoItemState();
            }
        }
    }

    private void OnItemAdd(InventoryItem addedItem)
    {
        print("Event received: " + addedItem.name);
        InventoryItem inventoryItem = null;
        foreach (var ownedItem in ownedItems)
        {
            if (ownedItem.name == addedItem.name)
            {
                inventoryItem = ownedItem;
                break;
            }
        }

        if (inventoryItem != null)
        {
            print("Inventory item found: " + inventoryItem.name);
            inventoryItem.count += addedItem.count;
        }
        else
        {
            ownedItems.Add(addedItem);
        }

        UpdateDisplay();
    }


    [ContextMenu("Add random item")]
    public void AddRandomItem()
    {
        var inventoryItem = new InventoryItem
        {
            name = randomItem.name,
            icon = randomItem.icon,
            count = randomItem.count,
            description = randomItem.description
        };
        EventManager.OnItemAdd?.Invoke(inventoryItem);
        print("Event sent");
    }
}

[Serializable]
public class InventoryItem
{
    public Sprite icon;
    public string name;
    public int count;
    public string description;
}