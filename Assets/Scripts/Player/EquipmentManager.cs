using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private List<EquippableItem> equippedItems;

    public static EquipmentManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        equippedItems = new List<EquippableItem>();
    }

    public int CountEquippedType(EquippableItem item)
    {
        return equippedItems
            .FindAll(equippedItem => item.equipmentGroup == equippedItem.equipmentGroup)
            .ConvertAll(i => i.slotCount)
            .Sum();
    }

    public void AddEquipment(EquippableItem equippableItem)
    {
        equippedItems.Add(equippableItem);
        GetSlotRef(equippableItem).UpdateDisplay(equippableItem, InventoryCellType.Equipment);
    }
    
    public void RemoveEquipment(EquippableItem equippableItem)
    {
        equippedItems.Remove(equippableItem);
        GetSlotRef(equippableItem).SetNoItemState();
        EventManager.OnItemAdd?.Invoke(equippableItem);
    }
    
    public void ApplyAllEquipments()
    {
        foreach (var item in equippedItems)
        {
            item.ApplyEffect(false);
        }
    }

    private InventoryCell GetSlotRef(EquippableItem equippableItem)
    {
        var rl = CountEquippedType(equippableItem) % 2;
        switch (equippableItem.equipmentType)
        {
            case EquipmentType.TwoHanded:
                return InventoryController.Instance.EquipmentSlotCells.mainHand;
            case EquipmentType.Helmet:
                return InventoryController.Instance.EquipmentSlotCells.helmet;
            case EquipmentType.Chestplate:
                return InventoryController.Instance.EquipmentSlotCells.chestplate;
            case EquipmentType.Leggings:
                return InventoryController.Instance.EquipmentSlotCells.leggings;
            case EquipmentType.Boots:
                return InventoryController.Instance.EquipmentSlotCells.boots;
            case EquipmentType.SingleHand:
                return rl == 1
                    ? InventoryController.Instance.EquipmentSlotCells.mainHand
                    : InventoryController.Instance.EquipmentSlotCells.offHand;
            case EquipmentType.Ring:
                return rl == 1
                    ? InventoryController.Instance.EquipmentSlotCells.leftRing
                    : InventoryController.Instance.EquipmentSlotCells.rightRing;
            case EquipmentType.Bracelet:
                return rl == 1
                    ? InventoryController.Instance.EquipmentSlotCells.leftBracelet
                    : InventoryController.Instance.EquipmentSlotCells.rightBracelet;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}