using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class DataManager : MonoBehaviour
{
    [SerializeField] private string pathName;
    [SerializeField] private string filename;
    [SerializeField] [TextArea] private string testJson;

    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        print($"Attached game object: {gameObject.name}");
    }

    public void OnStatSave(StatController toSave)
    {
        print("Saving player stats...");
        var basePath = PreSaveProcess("player_stats_");

        var statSaveData = new StatSaveData();
        statSaveData.level = toSave.Level;
        statSaveData.freePoints = toSave.FreePoints;
        statSaveData.skillPoints = toSave.SkillPoints;
        statSaveData.sourceName = toSave.SourceName;
        statSaveData.baseStats = new SerializedDictionary<StatTypes, StatData>();
        statSaveData.statValues = new SerializedDictionary<StatValue, StatData>();
        print($"LVL: {statSaveData.level}");
        print($"Free points: {statSaveData.freePoints}");
        print($"Skill points: {statSaveData.skillPoints}");
        print($"Source name: {statSaveData.sourceName}");
        foreach (var statType in Enum.GetValues(typeof(StatTypes)).Cast<StatTypes>())
        {
            var baseStat = toSave.GetBaseStat(statType);
            statSaveData.baseStats.Add(statType, baseStat);
        }

        foreach (var statValue in Enum.GetValues(typeof(StatValue)).Cast<StatValue>())
        {
            var baseStat = toSave.GetStatValue(statValue);
            statSaveData.statValues.Add(statValue, baseStat);
        }

        var json = JsonUtility.ToJson(statSaveData);

        File.WriteAllText(basePath, json);
    }

    private string PreSaveProcess(string filePrefix)
    {
        var basePath = Path.Combine(Application.persistentDataPath, pathName);
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        basePath = Path.Combine(basePath, filePrefix + filename);

        return basePath;
    }

    public void SaveInventory()
    {
        var instanceOwnedItem = InventoryController.Instance.SaveData;

        var basePath = PreSaveProcess("inventory");

        var json = JsonUtility.ToJson(instanceOwnedItem);
        print("Saving inventory...");
        print($"Gold amount: {instanceOwnedItem.goldAmount}");
        File.WriteAllText(basePath, json);
    }

    public void SaveEquippment()
    {
        var instanceOwnedItem = EquipmentManager.Instance.SaveData;

        var basePath = PreSaveProcess("equipment_");

        var json = JsonUtility.ToJson(instanceOwnedItem);

        File.WriteAllText(basePath, json);
    }

    public void SaveSkills()
    {
        var instanceOwnedItem = PlayerActionManager.Instance.SaveData;

        var basePath = PreSaveProcess("skills_");

        var json = JsonUtility.ToJson(instanceOwnedItem);

        File.WriteAllText(basePath, json);
    }

    public void LoadInventory()
    {
        print("Trying to load player inventory");
        var basePath = PreSaveProcess("inventory");
        if (!File.Exists(basePath)) return;

        var allJson = File.ReadAllText(basePath);
        var inventoryData = JsonUtility.FromJson<InventoryData>(allJson);
        print($"Loaded player inventory: inventory size {inventoryData.ownedItems.Count}");
        InventoryController.Instance.LoadData(inventoryData);
    }

    public void LoadPlayerStats()
    {
        print("Trying to load player stats");
        var basePath = PreSaveProcess("player_stats_");
        if (!File.Exists(basePath)) return;

        var allJson = File.ReadAllText(basePath);
        var statSaveData = JsonUtility.FromJson<StatSaveData>(allJson);
        print($"Loaded player stats: lvl {statSaveData.level}");
        if (statSaveData.sourceName == "Player")
        {
            PlayerInputController.Instance.Stats.LoadData(statSaveData);
        }
    }

    public void LoadEquipment()
    {
        print("Trying to load player equipment");
        var basePath = PreSaveProcess("equipment_");
        if (!File.Exists(basePath)) return;

        var allJson = File.ReadAllText(basePath);
        var inventoryData = JsonUtility.FromJson<InventoryData>(allJson);
        EquipmentManager.Instance.LoadData(inventoryData);
    }

    public void LoadSkills()
    {
        print("Trying to load player skills");
        var basePath = PreSaveProcess("skills_");
        if (!File.Exists(basePath)) return;

        var allJson = File.ReadAllText(basePath);
        var data = JsonUtility.FromJson<SkillSaveDataWrapper>(allJson);
        PlayerActionManager.Instance.LoadData(data);
    }

    public void SaveQuests()
    {
        var toSave = QuestManager.Instance.WrappedData;

        var basePath = PreSaveProcess("quests_");

        var json = JsonUtility.ToJson(toSave);

        File.WriteAllText(basePath, json);
    }

    public void LoadQuests()
    {
        print("Trying to load player quests");
        var basePath = PreSaveProcess("quests_");
        if (!File.Exists(basePath))
        {
            QuestManager.Instance.WrappedData = new QuestDataWrapper();
            return;
        }

        var allJson = File.ReadAllText(basePath);
        var data = JsonUtility.FromJson<QuestDataWrapper>(allJson);
        QuestManager.Instance.WrappedData = data;
    }

    [ContextMenu("Load player")]
    public void LoadPlayer(string caller)
    {
        print($"Loading player data: {caller}");
        LoadPlayerStats();
        LoadInventory();
        LoadEquipment();
        LoadSkills();
        LoadQuests();
    }

    [ContextMenu("Save trigger")]
    public void SaveEventTrigger(string caller)
    {
        print($"{caller} -> Saving everything to {Application.persistentDataPath}");
        OnStatSave(PlayerInputController.Instance.Stats);
        SaveInventory();
        SaveEquippment();
        SaveSkills();
        SaveQuests();
    }
}

[Serializable]
public class StatSaveData
{
    public string sourceName;
    public int level;
    public int freePoints;
    public int skillPoints;

    public SerializedDictionary<StatTypes, StatData> baseStats;
    public SerializedDictionary<StatValue, StatData> statValues;
}