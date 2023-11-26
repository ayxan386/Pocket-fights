using System;
using System.Collections.Generic;

public class EquippableItem : InventoryItem
{
    public EquipmentType equipmentType;
    public EquipmentGroup equipmentGroup;
    public int slotCount;
    public List<EquipmentStatEffect> statEffects;
    public bool isEquipped;

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

        ApplyEffect();
    }

    public void ApplyEffect(bool shouldUpdate = true)
    {
        var stats = PlayerInputController.Instance.Stats;
        foreach (var statEffect in statEffects)
        {
            stats.BoostStatValue(statEffect.statValue,
                statEffect.TotalChange(stats.GetStatValue(statEffect.statValue).baseValue), shouldUpdate);
        }
    }

    public void TryUnEquip()
    {
        EventManager.OnItemAdd?.Invoke(this);
        displayInInventory = true;
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
    public StatTypes statType;
    public StatValue statValue;
    public int constantImprovement;
    public float fractionalImprovement;

    public int TotalChange(float baseStat)
    {
        return (int)(constantImprovement + baseStat * fractionalImprovement);
    }
}