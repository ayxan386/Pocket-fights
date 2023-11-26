using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopItemsData : MonoBehaviour
{
    [SerializeField] private List<ShopItemData> possibleItems;

    public List<InventoryItem> GenerateShopData(Transform parent)
    {
        return possibleItems.FindAll(item => item.chance >= Random.value)
            .ConvertAll(possibleItem =>
            {
                var newItem = Instantiate(possibleItem.itemRef, parent);
                newItem.count = Random.Range(possibleItem.countRange.x, possibleItem.countRange.y);
                return newItem;
            });
    }
}

[Serializable]
public class ShopItemData
{
    public InventoryItem itemRef;
    [Range(0, 1f)] public float chance;
    public Vector2Int countRange;
}