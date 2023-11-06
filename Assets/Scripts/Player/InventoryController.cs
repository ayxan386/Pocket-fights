using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> ownedItems;
    [SerializeField] private Transform itemCellHolder;

    private List<InventoryCell> itemCells;

    private void Start()
    {
        ownedItems = new List<InventoryItem>();
        itemCells = GetComponentsInChildren<InventoryCell>().ToList();
    }
}

[Serializable]
public class InventoryItem
{
    public Sprite icon;
    public string name;
    public int count;
    public string description;
}