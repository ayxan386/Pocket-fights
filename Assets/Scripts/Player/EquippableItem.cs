using System;
using System.Collections.Generic;
using UnityEngine;

public class EquippableItem : InventoryItem
{
    public EquipmentType equipmentType;
    public EquipmentGroup equipmentGroup;
    public int slotCount;
    public List<EquipmentStatEffect> statEffects;
    public bool isEquipped;
    public GameObject inWorldObject;
    public Vector3[] offset;
    public Vector3[] rotation;
    public Vector3[] scale;
    public Transform referencePoint;
    public bool isRight;
    public InventoryCell slotRef;

    public bool TryEquip()
    {
        if (isEquipped) return false;

        var countEquippedType = EquipmentManager.Instance.CountEquippedType(this);

        if (2 - slotCount - countEquippedType >= 0)
        {
            Equip();
            return true;
        }

        return false;
    }

    private void Equip()
    {
        EquipmentManager.Instance.AddEquipment(this);
        PlaceInReferencePoint();
        ApplyEffect();
    }

    [ContextMenu("Place in reference point")]
    public void PlaceInReferencePoint()
    {
        if (!isEquipped || equipmentType != EquipmentType.SingleHand) return;

        inWorldObject.SetActive(true);
        var objectTransform = inWorldObject.transform;
        objectTransform.SetParent(referencePoint);

        var index = isRight ? 1 : 0;
        objectTransform.localScale = scale[index];
        objectTransform.localEulerAngles = rotation[index];
        objectTransform.localPosition = offset[index];
    }

    public void ApplyEffect(bool shouldUpdate = true)
    {
        var stats = PlayerInputController.Instance.Stats;

        foreach (var statEffect in statEffects)
        {
            var totalChange = statEffect.TotalChange(stats.GetStatValue(statEffect.statValue).baseValue);
            stats.BoostStatValue(statEffect.statValue, totalChange, shouldUpdate);
        }
    }

    public void ReverseEffect(bool shouldUpdate = true)
    {
        var stats = PlayerInputController.Instance.Stats;

        foreach (var statEffect in statEffects)
        {
            var totalChange = statEffect.TotalChange(stats.GetStatValue(statEffect.statValue).baseValue);
            stats.BoostStatValue(statEffect.statValue, -totalChange, shouldUpdate);
        }
    }

    public void TryUnEquip()
    {
        isEquipped = false;
        referencePoint = null;
        inWorldObject.SetActive(false);
        displayInInventory = true;
        ReverseEffect();
        EquipmentManager.Instance.RemoveEquipment(this);
    }
}

public enum EquipmentType
{
    SingleHand,
    TwoHanded,
    Helmet,
    Chestplate,
    Leggings,
    Boots,
    Ring,
    Bracelet
}

public enum EquipmentGroup
{
    Hand,
    Armor,
    Accessory
}

[Serializable]
public class EquipmentStatEffect
{
    public StatValue statValue;
    public int constantImprovement;
    public float fractionalImprovement;

    public int TotalChange(float baseStat)
    {
        return (int)(constantImprovement + baseStat * fractionalImprovement);
    }
}