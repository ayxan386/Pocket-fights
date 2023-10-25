using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    public static PlayerStatController Instance { get; private set; }
    private Dictionary<Stats, StatData> stats;
    private StatBarIndicator healthBarIndicator;
    private StatBarIndicator manaBarIndicator;

    private void Awake()
    {
        Instance = this;
        stats = new Dictionary<Stats, StatData>
        {
            [Stats.Vitality] = new(4, 4),
            [Stats.Strength] = new(4, 4),
            [Stats.Mana] = new(3, 3),
            [Stats.Defense] = new(4, 4)
        };
    }

    private void Start()
    {
        healthBarIndicator = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<StatBarIndicator>();
        manaBarIndicator = GameObject.FindGameObjectWithTag("PlayerManaBar").GetComponent<StatBarIndicator>();
    }

    public StatData GetStat(Stats statsType)
    {
        return stats[statsType];
    }

    public void ReceiveAttack(StatData statData)
    {
        stats[Stats.Vitality].currentValue -= statData.currentValue;
        healthBarIndicator.UpdateDisplay(stats[Stats.Vitality]);
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