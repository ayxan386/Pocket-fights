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
        EventManager.OnItemRemove += OnItemRemove;
    }


    public void UpdateDisplay()
    {
        for (var i = 0; i < itemCells.Count; i++)
        {
            if (i < ownedItems.Count)
            {
                itemCells[i].UpdateDisplay(ownedItems[i], i);
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

        if (inventoryItem != null)
        {
            inventoryItem.count += addedItem.count;
        }
        else
        {
            ownedItems.Add(addedItem);
        }

        UpdateDisplay();
    }


    private void OnItemRemove(InventoryItem removedItem)
    {
        var removedCount = ownedItems.RemoveAll(ownedItem => ownedItem.name == removedItem.name);

        if (removedCount <= 0)
        {
            print("Item not found");
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
    }

    [ContextMenu("Remove random item")]
    public void RemoveRandomItem()
    {
        EventManager.OnItemRemove?.Invoke(randomItem);
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