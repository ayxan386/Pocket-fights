using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> ownedItems;
    [SerializeField] private Transform itemCellHolder;
    [SerializeField] private InventoryItem randomItem;
    [SerializeField] private int gold;
    [SerializeField] private int inventorySize;
    [SerializeField] private EquipmentSlotCells equipmentCells;
    [SerializeField] private List<InventoryItem> itemPrefabs;

    private List<InventoryCell> itemCells;

    public static InventoryController Instance { get; private set; }
    public InventoryData SaveData => new(ownedItems, Gold);
    public List<InventoryItem> OwnedItems => ownedItems;

    public EquipmentSlotCells EquipmentSlotCells => equipmentCells;
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

        CheckItemPrefabNames();

        UpdateDisplay();
        EventManager.OnItemAdd += OnItemAdd;
        EventManager.OnItemAddAsLoot += OnItemAddAsLoot;
        EventManager.OnItemRemove += OnItemRemove;
    }

    private void OnDestroy()
    {
        EventManager.OnItemAdd -= OnItemAdd;
        EventManager.OnItemAddAsLoot -= OnItemAddAsLoot;
        EventManager.OnItemRemove -= OnItemRemove;
    }

    public void UpdateDisplay()
    {
        if (itemCells == null) return;
        for (var i = 0; i < itemCells.Count; i++)
        {
            if (i < ownedItems.Count && ownedItems[i].displayInInventory)
            {
                itemCells[i].UpdateDisplay(ownedItems[i], InventoryCellType.Bag);
            }
            else
            {
                itemCells[i].SetNoItemState();
            }
        }
    }


    private void CheckItemPrefabNames()
    {
        print("Checking item prefab name consistency...");
        foreach (var itemPrefab in itemPrefabs)
        {
            if (itemPrefab.gameObject.name != itemPrefab.name)
            {
                Debug.LogError($"Item {itemPrefab.gameObject.name} names not matching");
            }
        }

        print("...Checking item prefab name consistency -> DONE");
    }

    private void OnItemAdd(InventoryItem addedItem)
    {
        if (addedItem.type == ItemType.Currency)
        {
            UseAllCurrencies(addedItem);
            return;
        }

        var inventoryItem = ownedItems.FirstOrDefault(ownedItem => ownedItem.name == addedItem.name
                                                                   && ownedItem.count < ownedItem.stackSize);

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

    private static void UseAllCurrencies(InventoryItem addedItem)
    {
        while (addedItem != null && addedItem.count > 0)
        {
            addedItem.Use();
            addedItem.count--;
        }
    }


    private void OnItemAddAsLoot(InventoryItem item, LootItemPanel panel)
    {
        OnItemAdd(item);
        UpdateLootSelection(panel);
        Destroy(panel.gameObject);
    }

    private static void UpdateLootSelection(LootItemPanel panel)
    {
        var loots = panel.transform.parent;
        for (int i = 0; i < loots.childCount; i++)
        {
            if (loots.GetChild(i) == panel.transform) continue;
            EventSystem.current.SetSelectedGameObject(loots.GetChild(i).gameObject);
            return;
        }

        var allButtons = loots.parent.GetComponentsInChildren<Button>();
        foreach (var button in allButtons)
        {
            if (button.transform == panel.transform) continue;
            button.Select();
            return;
        }
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

    private void OnItemSold(InventoryItem soldItem)
    {
        if (!soldItem.canBeSold) return;

        soldItem.count--;
        AddGold(soldItem.sellPrice);

        ShopManager.Instance.ItemSold(soldItem, 1);

        if (soldItem.count <= 0)
        {
            EventManager.OnItemRemove?.Invoke(soldItem, 1);
        }
        else
        {
            UpdateDisplay();
        }
    }

    private void OnItemRemove(InventoryItem removedItem, int count = 1)
    {
        removedItem.count -= count;

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
        EventManager.OnItemRemove?.Invoke(randomItem, randomItem.count);
    }

    public void ItemCellClicked(InventoryItem clickedItem, InventoryCellType cell)
    {
        //TODO change this

        switch (cell)
        {
            case InventoryCellType.Shop:
                OnItemBought(clickedItem);
                break;
            case InventoryCellType.Bag when ShopManager.Instance.IsShopOpen && clickedItem.canBeSold:
                OnItemSold(clickedItem);
                break;
            case InventoryCellType.Equipment:
                (clickedItem as EquippableItem)?.TryUnEquip();
                break;
            default:
                print("Using item");
                if (clickedItem is EquippableItem item)
                {
                    if (item.TryEquip())
                    {
                        ownedItems.Remove(item);
                        UpdateDisplay();
                    }
                }
                else
                {
                    clickedItem.Use();
                    EventManager.OnItemRemove?.Invoke(clickedItem , 1);
                }

                break;
        }
    }


    public void LoadData(InventoryData inventoryData)
    {
        ownedItems = new List<InventoryItem>();
        foreach (var savedItem in inventoryData.ownedItems)
        {
            var inventoryItemPrefab = itemPrefabs.Find(prefab => prefab.name == savedItem.saveName);
            if (inventoryItemPrefab == null) continue;
            var inventoryItem = Instantiate(inventoryItemPrefab, transform);
            inventoryItem.count = savedItem.count;
            OnItemAdd(inventoryItem);
        }

        gold = inventoryData.goldAmount;

        UpdateDisplay();
        EventManager.OnPlayerCoreUpdate?.Invoke(gold);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        EventManager.OnPlayerCoreUpdate?.Invoke(gold);
    }

    public bool CheckSkillFragments(Skill targetSkill)
    {
        foreach (var item in ownedItems)
        {
            if (IsSkillFragment(targetSkill, item))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSkillFragment(Skill targetSkill, InventoryItem item)
    {
        return item.type == ItemType.SkillFragment
               && item.extraData == targetSkill.id
               && item.count >= PlayerActionManager.Instance.SkillFragmentCountRequirement;
    }

    public void ConsumeSkillFragments(Skill targetSkill)
    {
        foreach (var item in ownedItems)
        {
            if (!IsSkillFragment(targetSkill, item)) continue;

            EventManager.OnItemRemove?.Invoke(item, PlayerActionManager.Instance.SkillFragmentCountRequirement);
            break;
        }
    }
}


[Serializable]
public class InventoryData
{
    public List<InventoryItemWrapper> ownedItems;
    public int goldAmount;

    public InventoryData(List<InventoryItem> ownedItems, int goldAmount)
    {
        this.ownedItems = ownedItems.ConvertAll(item => new InventoryItemWrapper(item.name, item.count));
        this.goldAmount = goldAmount;
    }

    public InventoryData(List<EquippableItem> equippedItems)
    {
        ownedItems =
            equippedItems.ConvertAll(item => new InventoryItemWrapper(item.name, item.count, item.isEquipped));
    }
}

[Serializable]
public class InventoryItemWrapper
{
    public string saveName;
    public int count;
    public bool equipped;

    public InventoryItemWrapper(string saveName, int count)
    {
        this.saveName = saveName;
        this.count = count;
        equipped = false;
    }

    public InventoryItemWrapper(string saveName, int count, bool equipped)
    {
        this.saveName = saveName;
        this.count = count;
        this.equipped = equipped;
    }
}

[Serializable]
public struct EquipmentSlotCells
{
    [Header("Hands")] public InventoryCell mainHand;
    public InventoryCell offHand;

    [Header("Armor")] public InventoryCell helmet;
    public InventoryCell chestplate;
    public InventoryCell leggings;
    public InventoryCell boots;

    [Header("Accessories")] public InventoryCell leftRing;
    public InventoryCell rightRing;
    public InventoryCell leftBracelet;
    public InventoryCell rightBracelet;
}