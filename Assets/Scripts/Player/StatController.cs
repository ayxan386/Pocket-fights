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
    [field: SerializeField] public Transform animationPosition { get; set; }

    private Dictionary<StatTypes, StatData> baseStats;
    private Dictionary<StatValue, StatData> statValues;

    public float Lsf => Mathf.Pow(1.03f, Level);
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
        if (FreePoints > 0)
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
        EventManager.OnStatChanged?.Invoke(statType, statValues[statType]);
        UpdateOverallDisplay();
    }

    public void BoostStatValue(StatValue stat, float diff, bool shouldUpdate = false)
    {
        print($"Boosting {stat} by {diff}");
        statValues[stat].currentValue += diff;
        statValues[stat].maxValue = Mathf.Max(statValues[stat].currentValue, statValues[stat].maxValue);
        if (shouldUpdate)
            UpdateOverallDisplay();

        EventManager.OnPlayerCoreUpdate?.Invoke(FreePoints);
    }

    private void UpdateFreePoints(int diff)
    {
        FreePoints += diff;
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

        EventManager.OnBaseStatUpdate?.Invoke(baseStats[StatTypes.Vitality].maxValue);
        EventManager.OnPlayerCoreUpdate?.Invoke(FreePoints);
    }

    public void UpdateLevel(int increment)
    {
        Level += increment;
        SkillPoints += Mathf.Max(Level / 2, 1);
        UpdateFreePoints(Level);
        EventManager.OnPlayerCoreUpdate?.Invoke(FreePoints);
    }

    public void UseSkillPoints(int usedPointsAmount)
    {
        if (usedPointsAmount > SkillPoints)
            throw new ArgumentException("Cannot use more than owned amount");

        SkillPoints -= usedPointsAmount;
        EventManager.OnPlayerCoreUpdate?.Invoke(SkillPoints);
    }

    public void ReceiveAttack(float baseDmg)
    {
        var receivedDamage = Mathf.Max(baseDmg - statValues[StatValue.DamageReduction].currentValue, 0);
        receivedDamage = StatusManager.CheckForDamage(receivedDamage);

        statValues[StatValue.Health].currentValue -= receivedDamage;
        SpawnDamageIndicator(receivedDamage);

        if (statValues[StatValue.Health].currentValue <= 0)
        {
            AttachedEntity.OnDeathCallback();
        }
        else
        {
            AttachedEntity.OnReceiveAttack(receivedDamage);
        }

        healthBarIndicator.UpdateDisplay(statValues[StatValue.Health]);
    }

    private void SpawnDamageIndicator(float receivedDamage)
    {
        var floatingText = Instantiate(damagePrefab, transform.position, Quaternion.identity, transform);
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
    }
}