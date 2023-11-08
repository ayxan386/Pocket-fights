using System;
using UnityEngine;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    public Sprite icon;
    public string name;
    public int count;
    public int stackSize;
    public string description;

    public override string ToString()
    {
        return $"Item: {name} {count}";
    }
}