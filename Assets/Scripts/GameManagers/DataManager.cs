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
    }

    private void Start()
    {
        EventManager.OnStatSave += OnStatSave;
    }

    private void OnStatSave(StatController toSave)
    {
        var basePath = Application.persistentDataPath;
        basePath += pathName;
        if (Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        basePath += filename;
        if (File.Exists(basePath))
        {
            File.Create(basePath);
        }

        var statSaveData = new StatSaveData();
        statSaveData.sourceName = toSave.SourceName;
        statSaveData.baseStats = new SerializedDictionary<StatTypes, StatData>();
        statSaveData.statValues = new SerializedDictionary<StatValue, StatData>();
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

        print(json);
    }

    [ContextMenu("Load from string")]
    public void LoadPlayerStats()
    {
        var statSaveData = JsonUtility.FromJson<StatSaveData>(testJson);
        if (statSaveData.sourceName == "Player")
        {
            PlayerInputController.Instance.Stats.LoadData(statSaveData);
        }
    }
}

[Serializable]
public class StatSaveData
{
    public string sourceName;
    public SerializedDictionary<StatTypes, StatData> baseStats;
    public SerializedDictionary<StatValue, StatData> statValues;

}