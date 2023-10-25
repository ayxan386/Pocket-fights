using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    public static PlayerStatController Instance { get; private set; }
    private Dictionary<Stats, StatData> stats;

    private void Awake()
    {
        Instance = this;
        stats = new Dictionary<Stats, StatData>
        {
            [Stats.Vitality] = new StatData(4, 4),
            [Stats.Strength] = new StatData(4, 4),
            [Stats.Mana] = new StatData(3, 3),
            [Stats.Defense] = new StatData(4, 4)
        };
    }

    public StatData GetStat(Stats statsType)
    {
        return stats[statsType];
    }
}

public enum Stats
{
    Vitality,
    Strength,
    Mana,
    Defense
}

[Serializable]
public class StatData
{
    public float maxValue;
    public float currentValue;

    public StatData() : this(1, 1)
    {
    }

    public StatData(float maxValue, float currentValue)
    {
        this.maxValue = maxValue;
        this.currentValue = currentValue;
    }
}