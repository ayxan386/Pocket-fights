using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    public Sprite icon;
    public new string name;
    public int count;
    public int buyPrice;
    public int stackSize;
    public string description;
    public UnityEvent onUseAction;
    public ItemType type;
    public bool canBeSold;

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
    Consumable
}