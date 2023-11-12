using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    public Sprite icon;
    public string name;
    public int count;
    public int stackSize;
    public string description;
    public UnityEvent onUseAction;

    public void Use()
    {
        onUseAction.Invoke();
    }

    public override string ToString()
    {
        return $"Item: {name} {count}";
    }
}