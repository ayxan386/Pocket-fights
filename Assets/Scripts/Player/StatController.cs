using System;
using System.Collections.Generic;
using UnityEngine;

public class StatController : MonoBehaviour
{
    private Dictionary<StatTypes, StatData> stats;
    [SerializeField] private StatBarIndicator healthBarIndicator;
    [SerializeField] private StatBarIndicator manaBarIndicator;

    private void Awake()
    {
        stats = new Dictionary<StatTypes, StatData>
        {
            [StatTypes.Vitality] = new(4, 4),
            [StatTypes.Strength] = new(4, 4),
            [StatTypes.Mana] = new(3, 3),
            [StatTypes.Defense] = new(4, 4)
        };
    }

    private void Start()
    {
        healthBarIndicator ??= GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<StatBarIndicator>();
        manaBarIndicator ??= GameObject.FindGameObjectWithTag("PlayerManaBar").GetComponent<StatBarIndicator>();
    }

    public StatData GetStat(StatTypes statTypesType)
    {
        return stats[statTypesType];
    }

    public void ReceiveAttack(float baseDmg)
    {
        stats[StatTypes.Vitality].currentValue -= baseDmg;
        healthBarIndicator.UpdateDisplay(stats[StatTypes.Vitality]);
    }
}

public enum StatTypes
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