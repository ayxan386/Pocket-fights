using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class LootManager
{
    public static List<(InventoryItem, int)> GenerateLoot(List<List<PossibleLoot>> possibleDrops, float rollBoost = 1)
    {
        var res = new List<(InventoryItem, int)>();
        Debug.Log($"Generating loot with roll boost {rollBoost}");
        foreach (var mobLoots in possibleDrops)
        {
            foreach (var possibleLoot in mobLoots)
            {
                var roll = Random.value * rollBoost;
                if (roll >= 1 - possibleLoot.chance)
                {
                    var dropCount = Random.Range(possibleLoot.count.x, possibleLoot.count.y + 1);
                    var index = res.FindIndex(item => item.Item1.name == possibleLoot.itemPrefab.name);
                    if (index >= 0)
                    {
                        var (item, count) = res[index];
                        res[index] = (item, count + dropCount);
                    }
                    else
                    {
                        res.Add((possibleLoot.itemPrefab, dropCount));
                    }
                }
            }
        }

        return res;
    }
}

[Serializable]
public class PossibleLoot
{
    public GameObject inWorldDisplayItem;
    public InventoryItem itemPrefab;
    [Range(0, 1f)] public float chance;
    public Vector2Int count;
}