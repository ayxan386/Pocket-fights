using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    public Sprite icon;
    public new string name;
    public int count;
    public int stackSize;
    public string description;
    public UnityEvent onUseAction;
    public ItemType type;

    public void Use()
    {
        onUseAction.Invoke();
    }

    public override string ToString()
    {
        return $"Item: {name} {count}";
    }
}

public enum ItemType
{
    Useable,
    Consumable
}