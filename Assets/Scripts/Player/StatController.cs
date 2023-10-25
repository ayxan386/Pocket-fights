using System;
using System.Collections.Generic;
using UnityEngine;

public class StatController : MonoBehaviour
{
    [SerializeField] private StatBarIndicator healthBarIndicator;
    [SerializeField] private StatBarIndicator manaBarIndicator;
    [SerializeField] private int level;
    [SerializeField] private int startingVitality = 4;
    [SerializeField] private int startingStrength = 4;
    [SerializeField] private int startingMana = 4;
    [SerializeField] private int startingDefense = 4;

    private Dictionary<StatTypes, StatData> baseStats;
    private Dictionary<StatValue, StatData> statValues;
    private float Lsf => Mathf.Pow(1.03f, level);

    private void Awake()
    {
        baseStats = new Dictionary<StatTypes, StatData>
        {
            [StatTypes.Vitality] = new(startingVitality),
            [StatTypes.Strength] = new(startingStrength),
            [StatTypes.Mana] = new(startingMana),
            [StatTypes.Defense] = new(startingDefense)
        };

        statValues = new Dictionary<StatValue, StatData>
        {
            [StatValue.Health] = new(baseStats[StatTypes.Vitality].currentValue * 10 * Lsf,
                baseStats[StatTypes.Vitality].currentValue * 10 * Lsf),
            [StatValue.Mana] = new(baseStats[StatTypes.Mana].currentValue * 10 * Lsf,
                baseStats[StatTypes.Mana].currentValue * 10 * Lsf),
            [StatValue.BaseAttack] = new(baseStats[StatTypes.Strength].currentValue * 5 * Lsf,
                baseStats[StatTypes.Strength].currentValue * 5 * Lsf),
            [StatValue.DamageReduction] = new(baseStats[StatTypes.Defense].currentValue * 2 * Lsf,
                baseStats[StatTypes.Defense].currentValue * 2 * Lsf),
        };
    }

    private void Start()
    {
        healthBarIndicator ??= GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<StatBarIndicator>();
        manaBarIndicator ??= GameObject.FindGameObjectWithTag("PlayerManaBar").GetComponent<StatBarIndicator>();
    }

    public StatData GetStat(StatValue statValue)
    {
        return statValues[statValue];
    }

    public void ReceiveAttack(float baseDmg)
    {
        statValues[StatValue.Health].currentValue -= baseDmg - statValues[StatValue.DamageReduction].currentValue;
        healthBarIndicator.UpdateDisplay(statValues[StatValue.Health]);
    }
}

public enum StatTypes
{
    Vitality,
    Strength,
    Mana,
    Defense
}

public enum StatValue
{
    Health,
    Mana,
    BaseAttack,
    DamageReduction
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

    public StatData(float maxValue) : this(maxValue, maxValue)
    {
    }
}