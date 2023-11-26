using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    public Sprite icon;
    public new string name;
    public string description;
    public int stackSize;
    public int count;
    public int buyPrice;
    public bool displayInInventory;
    [Space(10)] [Header("Selling")] public bool canBeSold;
    public int sellPrice;
    public int maxSellPrice;
    public float priceDropRate;

    [Space(10)] public ItemType type;
    public UnityEvent onUseAction;

    public void Use()
    {
        onUseAction.Invoke();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => count <= 0);
        Destroy(gameObject);
    }

    public override string ToString()
    {
        return $"Item: {name} {count}";
    }
}

public enum ItemType
{
    Useable,
    Consumable,
    Currency,
    Equipment
}