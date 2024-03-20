using System;
using System.Collections.Generic;
using UnityEngine;

public class StatController : MonoBehaviour
{
    [SerializeField] private StatBarIndicator healthBarIndicator;
    [SerializeField] private StatBarIndicator manaBarIndicator;

    [field: Header("Core values")]
    [field: SerializeField] public int Level { get; set; }

    [field: SerializeField] public int FreePoints { get; private set; }
    [field: SerializeField] public int SkillPoints { get; private set; }
    [field: SerializeField] public string SourceName { get; private set; }
    [SerializeField] private StatConversionFactors conversionFactors;

    [Header("Starting stats")]
    [SerializeField] private int startingVitality = 4;

    [SerializeField] private int startingStrength = 4;
    [SerializeField] private int startingMana = 4;
    [SerializeField] private int startingDefense = 4;

    [field: Space(10)]
    [field: SerializeField]
    public StatusManager StatusManager { get; private set; }

    [SerializeField] [TextArea] private string statDebug;

    [Header("Receive attack VFX")]
    [SerializeField] private FloatingTextManager damagePrefab;
    [SerializeField] private Transform textSpawn;

    [field: SerializeField] public Transform animationPosition { get; set; }
    [field: SerializeField] public Transform hitPosition { get; set; }

    private Dictionary<StatTypes, StatData> baseStats;
    private Dictionary<StatValue, StatData> statValues;

    public float Lsf => Mathf.Pow(1.02f, Level);
    public BaseEntityCallbacks AttachedEntity { get; set; }

    public Animator Animator { get; set; }

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

        StatusManager.RelatedStats = this;
        statValues = new Dictionary<StatValue, StatData>();
        InitiateStatValues();
    }

    private void InitiateStatValues()
    {
        statValues[StatValue.Health] = new(baseStats[StatTypes.Vitality].currentValue * conversionFactors.health * Lsf);
        statValues[StatValue.Mana] = new(baseStats[StatTypes.Mana].currentValue * conversionFactors.mana * Lsf);
        statValues[StatValue.BaseAttack] =
            new(baseStats[StatTypes.Strength].currentValue * conversionFactors.baseAttack * Lsf);
        statValues[StatValue.DamageReduction] =
            new(baseStats[StatTypes.Defense].currentValue * conversionFactors.damageReduction * Lsf);
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
        statValues[StatValue.Health].baseValue = baseStats[StatTypes.Vitality].currentValue * conversionFactors.health * Lsf;
        statValues[StatValue.Health].maxValue = statValues[StatValue.Health].baseValue;
        statValues[StatValue.Mana].baseValue = baseStats[StatTypes.Mana].currentValue * conversionFactors.mana * Lsf;
        statValues[StatValue.Mana].maxValue = statValues[StatValue.Mana].baseValue;

        statValues[StatValue.BaseAttack].UpdateBaseValue(baseStats[StatTypes.Strength].currentValue * conversionFactors.baseAttack * Lsf);
        statValues[StatValue.DamageReduction].UpdateBaseValue(baseStats[StatTypes.Defense].currentValue * conversionFactors.damageReduction * Lsf);
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

    public bool UsedAction(float manaConsumption)
    {
        if (statValues[StatValue.Mana].currentValue < manaConsumption) return false;
        statValues[StatValue.Mana].currentValue -= manaConsumption;
        EventManager.OnStatChanged?.Invoke(StatValue.Mana, statValues[StatValue.Mana]);
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
        EventManager.OnStatChanged?.Invoke(StatValue.Mana, statValues[StatValue.Mana]);
        manaBarIndicator.UpdateDisplay(statValues[StatValue.Mana]);
    }

    public void UpgradeStat(StatTypes statType, int diff = 1)
    {
        var statLimit = PlayerInputController.Instance.LevelData.statLimit;
        if (FreePoints > 0 && GetBaseStat(statType).currentValue < statLimit)
        {
            UpdatePoints(-diff, 0);
            UpgradeBaseStat(statType, diff);
        }
    }

    public void UpgradeBaseStat(StatTypes statType, int diff)
    {
        var statLimit = PlayerInputController.Instance.LevelData.statLimit;
        baseStats[statType].maxValue = statLimit;
        baseStats[statType].currentValue += diff;
        baseStats[statType].baseValue += diff;
        CalculateStatValues();
        EventManager.OnBaseStatUpdate?.Invoke(baseStats[statType].maxValue);
    }

    public void UpdateStatValue(StatValue statType, int diff)
    {
        var currentValue = Mathf.Round(statValues[statType].currentValue + diff);
        statValues[statType].currentValue = Mathf.Clamp(currentValue, 0, statValues[statType].maxValue);
        EventManager.OnStatChanged?.Invoke(statType, statValues[statType]);

        if (statValues[StatValue.Health].currentValue <= 0)
        {
            AttachedEntity.OnDeathCallback();
        }

        UpdateOverallDisplay();
    }

    public void BoostStatValue(StatValue stat, float diff, bool shouldUpdate = false)
    {
        var statValue = statValues[stat];
        statValue.currentValue += diff;
        statValue.maxValue += diff;

        statValue.RoundDown();

        if (shouldUpdate)
            UpdateOverallDisplay();

        EventManager.OnPlayerCoreUpdate?.Invoke(FreePoints);
    }

    public void UpdatePoints(int freePointsDiff, int skillPointsDiff)
    {
        FreePoints += freePointsDiff;
        SkillPoints += skillPointsDiff;
        EventManager.OnPlayerCoreUpdate?.Invoke(FreePoints);
    }

    public void LoadData(StatSaveData savedData)
    {
        Level = savedData.level;
        FreePoints = savedData.freePoints;
        SkillPoints = savedData.skillPoints;
        baseStats = savedData.baseStats;
        InitiateStatValues();
        statValues = savedData.statValues;
    }

    public void SetBaseStatLimit(int limit = 8)
    {
        foreach (var baseStat in baseStats.Keys)
        {
            baseStats[baseStat].maxValue = limit;
        }
    }

    public void UpdateLevel(int increment)
    {
        Level += increment;
    }

    public void UseSkillPoints(int usedPointsAmount)
    {
        if (usedPointsAmount > SkillPoints)
            throw new ArgumentException("Cannot use more than owned amount");

        UpdatePoints(0, -usedPointsAmount);
    }

    public void ReceiveAttack(float baseDmg)
    {
        var receivedDamage = Mathf.Max(baseDmg - statValues[StatValue.DamageReduction].currentValue, 0);
        receivedDamage = StatusManager.CheckForDamage(receivedDamage);

        var healthStat = statValues[StatValue.Health];
        healthStat.currentValue -= receivedDamage;
        healthStat.RoundDown();
        SpawnDamageIndicator(receivedDamage);

        if (healthStat.currentValue <= 0)
        {
            AttachedEntity.OnDeathCallback();
        }
        else
        {
            AttachedEntity.OnReceiveAttack(receivedDamage);
        }

        healthBarIndicator.UpdateDisplay(healthStat);
    }

    private void SpawnDamageIndicator(float receivedDamage)
    {
        var floatingText = Instantiate(damagePrefab, textSpawn.position, Quaternion.identity, transform);
        floatingText.SetDirection(transform.eulerAngles.y > 200);
        floatingText.transform.forward = transform.forward;
        floatingText.floatingText.text = $"-{receivedDamage:N0}HP";
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

        RoundDown();
    }

    public void RoundDown()
    {
        maxValue = Mathf.Round(maxValue);
        currentValue = Mathf.Round(currentValue);
        baseValue = Mathf.Round(baseValue);
    }
}

[Serializable]
public class StatConversionFactors
{
    public float health;
    public float mana;
    public float baseAttack;
    public float damageReduction;
}