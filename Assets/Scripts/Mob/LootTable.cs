using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mob
{
    public class LootTable : MonoBehaviour
    {
        public List<RangeLimitedLootEntry> entries;
    }

    [Serializable]
    public class RangeLimitedLootEntry
    {
        public Vector2Int levelRange;
        public List<PossibleLoot> loots;
    }
}