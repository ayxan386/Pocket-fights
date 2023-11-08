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
        for (var index = 0; index < itemCells.Count; index++)
        {
            itemCells[index].SetId(index);
        }

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
        print("Removed event received");
        var inventoryItem = ownedItems.FirstOrDefault(ownedItem => ownedItem.name == removedItem.name);

        if (inventoryItem != null)
        {
            inventoryItem.count -= 1;

            if (inventoryItem.count <= 0)
            {
                ownedItems.Remove(inventoryItem);
            }
        }
        else
        {
            print("Item not found");
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

    public void ItemCellClicked(InventoryItem clickedItem)
    {
        //TODO change this
        EventManager.OnItemRemove?.Invoke(clickedItem);
    }
}