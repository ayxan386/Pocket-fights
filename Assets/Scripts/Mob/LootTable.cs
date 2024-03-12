using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mob
{
    public class LootTable : MonoBehaviour
    {
        public List<InventoryItem> items;
        public int entryIndex;
        public float chance;
        public Vector2Int range;

        public List<RangeLimitedLootEntry> entries;

        [ContextMenu("Add items")]
        public void AddItemsToEntry()
        {
            var entry = entries[entryIndex];
            foreach (var item in items)
            {
                var possibleLoot = new PossibleLoot
                {
                    itemPrefab = item,
                    chance = chance,
                    count = range
                };
                entry.loots.Add(possibleLoot);
            }
        }

        [ContextMenu("Reduce chance in range")]
        public void ReduceChanceInRange()
        {
            var entry = entries[entryIndex];
            for (var index = 0; index < entry.loots.Count; index++)
            {
                if (index >= range.x && index <= range.y)
                {
                    entry.loots[index].chance -= chance;
                }
            }
        }
    }

    [Serializable]
    public class RangeLimitedLootEntry
    {
        public Vector2Int levelRange;
        public List<PossibleLoot> loots;
    }
}