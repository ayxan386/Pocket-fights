using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private List<EquippableItem> equippedItems;
    [SerializeField] private List<EquippableItem> itemPrefabs;
    [field: SerializeField] public Transform LeftHandAttachmentPoint { get; set; }
    [field: SerializeField] public Transform RightHandAttachmentPoint { get; set; }

    public static EquipmentManager Instance { get; private set; }

    public InventoryData SaveData => new(equippedItems);

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
        var inventoryCell = GetSlotRef(equippableItem);
        equippableItem.slotRef = inventoryCell;
        equippableItem.isEquipped = true;
        equippableItem.referencePoint = equippableItem.isRight ? RightHandAttachmentPoint : LeftHandAttachmentPoint;
        equippableItem.PlaceInReferencePoint();
        equippedItems.Add(equippableItem);
        inventoryCell.UpdateDisplay(equippableItem, InventoryCellType.Equipment);
    }

    public void RemoveEquipment(EquippableItem equippableItem)
    {
        equippedItems.Remove(equippableItem);
        equippableItem.slotRef.SetNoItemState();
        equippableItem.slotRef = null;
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
        var equipmentSlotCells = InventoryController.Instance.EquipmentSlotCells;
        switch (equippableItem.equipmentType)
        {
            case EquipmentType.TwoHanded:
                return equipmentSlotCells.mainHand;
            case EquipmentType.Helmet:
                return equipmentSlotCells.helmet;
            case EquipmentType.Chestplate:
                return equipmentSlotCells.chestplate;
            case EquipmentType.Leggings:
                return equipmentSlotCells.leggings;
            case EquipmentType.Boots:
                return equipmentSlotCells.boots;
            case EquipmentType.SingleHand:
                var leftHand = equipmentSlotCells.mainHand.storedItem == null;
                equippableItem.isRight = !leftHand;
                return leftHand
                    ? equipmentSlotCells.mainHand
                    : equipmentSlotCells.offHand;
            case EquipmentType.Ring:
                var leftHandFinger = equipmentSlotCells.leftRing.storedItem == null;
                equippableItem.isRight = !leftHandFinger;
                return leftHandFinger
                    ? equipmentSlotCells.leftRing
                    : equipmentSlotCells.rightRing;
            case EquipmentType.Bracelet:
                var leftArm = equipmentSlotCells.leftBracelet.storedItem == null;
                equippableItem.isRight = !leftArm;
                return leftArm
                    ? equipmentSlotCells.leftBracelet
                    : equipmentSlotCells.rightBracelet;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void LoadData(InventoryData inventoryData)
    {
        foreach (var savedItem in inventoryData.ownedItems)
        {
            var inventoryItemPrefab = itemPrefabs.Find(prefab => prefab.name == savedItem.saveName);
            if (inventoryItemPrefab == null || !savedItem.equipped) continue;
            var inventoryItem = Instantiate(inventoryItemPrefab, transform);
            inventoryItem.count = savedItem.count;
            AddEquipment(inventoryItem);
        }
    }
}