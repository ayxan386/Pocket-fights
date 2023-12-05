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

    [Header("Starting stats")]
    [SerializeField] private int startingVitality = 4;

    [SerializeField] private int startingStrength = 4;
    [SerializeField] private int startingMana = 4;
    [SerializeField] private int startingDefense = 4;

    [Space(10)]
    [SerializeField] private StatusManager statusManager;

    [SerializeField] [TextArea] private string statDebug;

    private Dictionary<StatTypes, StatData> baseStats;
    private Dictionary<StatValue, StatData> statValues;

    public float Lsf => Mathf.Pow(1.03f, level);
    public int Level
    {
        get => level;
        set => level = value;
    }

    public int FreePoints => freePoints;
    public int SkillPoints => skillPoints;

    public string SourceName => sourceName;
    public StatusManager StatusManager => statusManager;

    private void Awake()
    {
        baseStats = new Dictionary<StatTypes, StatData>
        {
            [StatTypes.Vitality] = new(startingVitality),
            [StatTypes.Strength] = new(startingStrength),
            [StatTypes.Mana] = new(startingMana),
            [StatTypes.Defense] = new(startingDefense),
            [StatTypes.None] = new(0)
        };

        statusManager.RelatedStats = this;
        statValues = new Dictionary<StatValue, StatData>();
        InitiateStatValues();
    }

    private void InitiateStatValues()
    {
        statValues[StatValue.Health] = new(baseStats[StatTypes.Vitality].maxValue * 10 * Lsf,
            baseStats[StatTypes.Vitality].currentValue * 10 * Lsf);
        statValues[StatValue.Mana] = new(baseStats[StatTypes.Mana].maxValue * 10 * Lsf,
            baseStats[StatTypes.Mana].currentValue * 10 * Lsf);
        statValues[StatValue.BaseAttack] = new(baseStats[StatTypes.Strength].maxValue * 5 * Lsf,
            baseStats[StatTypes.Strength].currentValue * 5 * Lsf);
        statValues[StatValue.DamageReduction] = new(baseStats[StatTypes.Defense].maxValue * 2 * Lsf,
            baseStats[StatTypes.Defense].currentValue * 2 * Lsf);
        statValues[StatValue.ManaRegen] = new(10, 10);
        statValues[StatValue.None] = new(10, 10);
    }

    public void UpdateOverallDisplay()
    {
        healthBarIndicator.UpdateDisplay(statValues[StatValue.Health]);
        manaBarIndicator.UpdateDisplay(statValues[StatValue.Mana]);
    }

    private void CalculateStatValues()
    {
        statValues[StatValue.Health].baseValue = baseStats[StatTypes.Vitality].maxValue * 10 * Lsf;
        statValues[StatValue.Health].maxValue = statValues[StatValue.Health].baseValue;
        statValues[StatValue.Mana].baseValue = baseStats[StatTypes.Mana].maxValue * 10 * Lsf;
        statValues[StatValue.Mana].maxValue = statValues[StatValue.Mana].baseValue;

        statValues[StatValue.BaseAttack].UpdateBaseValue(baseStats[StatTypes.Strength].currentValue * 5 * Lsf);
        statValues[StatValue.DamageReduction].UpdateBaseValue(baseStats[StatTypes.Defense].currentValue * 2 * Lsf);
        statValues[StatValue.ManaRegen].UpdateBaseValue(10);
        statValues[StatValue.None].UpdateBaseValue(10);

        // EquipmentManager.Instance.ApplyAllEquipments();

        UpdateOverallDisplay();
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
        statValues[StatValue.Health].currentValue -=
            Mathf.Max(baseDmg - statValues[StatValue.DamageReduction].currentValue, 0);
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

    public void UpgradeStat(StatTypes statType, int diff = 1)
    {
        if (freePoints > 0)
        {
            UpdateFreePoints(-diff);
            UpgradeBaseStat(statType, diff);
        }
    }

    public void UpgradeBaseStat(StatTypes statType, int diff)
    {
        baseStats[statType].maxValue += diff;
        baseStats[statType].currentValue += diff;
        baseStats[statType].baseValue += diff;
        CalculateStatValues();
        EventManager.OnBaseStatUpdate?.Invoke(baseStats[statType].maxValue);
    }

    public void UpdateStatValue(StatValue statType, int diff)
    {
        var currentValue = statValues[statType].currentValue + diff;
        statValues[statType].currentValue = Mathf.Clamp(currentValue, 0, statValues[statType].maxValue);
        UpdateOverallDisplay();
    }

    public void BoostStatValue(StatValue statType, float diff, bool shouldUpdate = false)
    {
        print($"Boosting {statType} by {diff}");
        statValues[statType].currentValue += diff;
        statValues[statType].maxValue = Mathf.Max(statValues[statType].currentValue, statValues[statType].maxValue);
        if (shouldUpdate)
            UpdateOverallDisplay();

        EventManager.OnPlayerCoreUpdate?.Invoke(freePoints);
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
        InitiateStatValues();

        EventManager.OnBaseStatUpdate?.Invoke(baseStats[StatTypes.Vitality].maxValue);
        EventManager.OnPlayerCoreUpdate?.Invoke(freePoints);
    }

    public void UpdateLevel(int increment)
    {
        level += increment;
        UpdateFreePoints(level);
        EventManager.OnPlayerCoreUpdate?.Invoke(freePoints);
    }
}

public enum StatTypes
{
    Vitality,
    Strength,
    Mana,
    Defense,
    None
}

public enum StatValue
{
    Health,
    Mana,
    BaseAttack,
    DamageReduction,
    ManaRegen,
    None
}

[Serializable]
public class StatData
{
    public float maxValue;
    public float baseValue;
    public float currentValue;

    public StatData() : this(1, 1)
    {
    }

    public StatData(float maxValue, float currentValue)
    {
        baseValue = maxValue;
        this.maxValue = maxValue;
        this.currentValue = currentValue;
    }

    public StatData(float maxValue) : this(maxValue, maxValue)
    {
    }

    public void UpdateBaseValue(float newBase)
    {
        maxValue = newBase;
        currentValue += newBase - baseValue;
        baseValue = newBase;
    }
}