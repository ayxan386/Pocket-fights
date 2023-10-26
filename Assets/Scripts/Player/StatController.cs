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

    [SerializeField] [TextArea] private string statDebug;

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
            [StatValue.ManaRegen] = new(10, 10)
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

    public void ReceiveAttack(float baseDmg, Action onDeathCallback)
    {
        statValues[StatValue.Health].currentValue -= baseDmg - statValues[StatValue.DamageReduction].currentValue;
        if (statValues[StatValue.Health].currentValue <= 0)
        {
            onDeathCallback.Invoke();
        }

        healthBarIndicator.UpdateDisplay(statValues[StatValue.Health]);
    }

    public bool UsedAction(float manaConsumption)
    {
        if (statValues[StatValue.Mana].currentValue < manaConsumption) return false;
        statValues[StatValue.Mana].currentValue -= manaConsumption;
        manaBarIndicator.UpdateDisplay(statValues[StatValue.Mana]);
        return true;
    }

    [ContextMenu("Update values")]
    public void UpdateDebugValues()
    {
        statDebug = "";
        foreach (var baseStat in baseStats)
        {
            statDebug += $"{baseStat.Key} -> {baseStat.Value.currentValue}\n";
        }

        statDebug += "-------------\n";
        foreach (var statValue in statValues)
        {
            statDebug += $"{statValue.Key} -> {statValue.Value.currentValue}/{statValue.Value.maxValue}\n";
        }
    }

    public void RegenMana()
    {
        statValues[StatValue.Mana].currentValue = Mathf.Clamp(
            statValues[StatValue.Mana].currentValue + statValues[StatValue.ManaRegen].currentValue,
            0, statValues[StatValue.Mana].maxValue);
        manaBarIndicator.UpdateDisplay(statValues[StatValue.Mana]);
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
    DamageReduction,
    ManaRegen
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