using System;
using System.Collections.Generic;
using UnityEngine;

public class StatController : MonoBehaviour
{
    [SerializeField] private StatBarIndicator healthBarIndicator;
    [SerializeField] private StatBarIndicator manaBarIndicator;
    [SerializeField] private int level;
    [SerializeField] private int freePoints;
    [SerializeField] private int skillPoints;
    [SerializeField] private string sourceName;

    [Header("Starting stats")] [SerializeField]
    private int startingVitality = 4;

    [SerializeField] private int startingStrength = 4;
    [SerializeField] private int startingMana = 4;
    [SerializeField] private int startingDefense = 4;

    [SerializeField] [TextArea] private string statDebug;

    private Dictionary<StatTypes, StatData> baseStats;
    private Dictionary<StatValue, StatData> statValues;
    
    public float Lsf => Mathf.Pow(1.03f, level);
    public int Level => level;
    public int FreePoints => freePoints;
    public int SkillPoints => skillPoints;

    public string SourceName => sourceName;

    private void Awake()
    {
        baseStats = new Dictionary<StatTypes, StatData>
        {
            [StatTypes.Vitality] = new(startingVitality),
            [StatTypes.Strength] = new(startingStrength),
            [StatTypes.Mana] = new(startingMana),
            [StatTypes.Defense] = new(startingDefense)
        };

        statValues = new Dictionary<StatValue, StatData>();
        CalculateStatBasedValues();
    }

    private void CalculateStatBasedValues()
    {
        statValues[StatValue.Health] = new(baseStats[StatTypes.Vitality].currentValue * 10 * Lsf,
            baseStats[StatTypes.Vitality].currentValue * 10 * Lsf);
        statValues[StatValue.Mana] = new(baseStats[StatTypes.Mana].currentValue * 10 * Lsf,
            baseStats[StatTypes.Mana].currentValue * 10 * Lsf);
        statValues[StatValue.BaseAttack] = new(baseStats[StatTypes.Strength].currentValue * 5 * Lsf,
            baseStats[StatTypes.Strength].currentValue * 5 * Lsf);
        statValues[StatValue.DamageReduction] = new(baseStats[StatTypes.Defense].currentValue * 2 * Lsf,
            baseStats[StatTypes.Defense].currentValue * 2 * Lsf);
        statValues[StatValue.ManaRegen] = new(10, 10);
    }

    private void Start()
    {
        healthBarIndicator ??= GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<StatBarIndicator>();
        manaBarIndicator ??= GameObject.FindGameObjectWithTag("PlayerManaBar").GetComponent<StatBarIndicator>();
    }


    public StatData GetStatValue(StatValue statValue)
    {
        return statValues[statValue];
    }

    public StatData GetBaseStat(StatTypes stat)
    {
        return baseStats[stat];
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

    public void UpgradeStat(StatTypes statType)
    {
        if (freePoints > 0)
        {
            UpdateFreePoints(-1);
            UpdateBaseStat(statType, 1);
        }
    }

    private void UpdateBaseStat(StatTypes statType, int diff)
    {
        baseStats[statType].maxValue += diff;
        baseStats[statType].currentValue += diff;
        EventManager.OnBaseStatUpdate?.Invoke(baseStats[statType].maxValue);
    }

    private void UpdateFreePoints(int diff)
    {
        freePoints += diff;
        EventManager.OnPlayerCoreUpdate?.Invoke(freePoints);
    }

    public void LoadData(StatSaveData savedData)
    {
        baseStats = savedData.baseStats;
        statValues = savedData.statValues;
        CalculateStatBasedValues();
        
        EventManager.OnBaseStatUpdate?.Invoke(baseStats[StatTypes.Vitality].maxValue);
        EventManager.OnPlayerCoreUpdate?.Invoke(freePoints);
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