using System;
using System.Collections;
using System.Collections.Generic;
using Quest;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestUi questUiPrefab;
    [SerializeField] private PotentialQuests potentialQuestSource;
    [SerializeField] private int targetCount;
    public static QuestManager Instance { get; private set; }

    public QuestDataWrapper WrappedData { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => WrappedData != null);

        potentialQuestSource.AddQuests(WrappedData.quests, targetCount);

        DataManager.Instance.SaveQuests();
    }

    public void OpenUi(Transform targetParent)
    {
        for (var index = 0; index < targetParent.childCount; index++)
        {
            Destroy(targetParent.GetChild(index).gameObject);
        }

        var quests = WrappedData.quests;

        foreach (var quest in quests)
        {
            var questUi = Instantiate(questUiPrefab, targetParent);
            questUi.UpdateDisplay(quest);
        }
    }
}

[Serializable]
public class QuestData
{
    public string title;
    public int goalCount;
    public int currentProgress;
    public string targetName;
    public int gold;
    public int expAmount;

    public QuestData(PotentialQuest potentialQuest)
    {
        title = potentialQuest.title;
        goalCount = Random.Range(potentialQuest.goalRange.x, potentialQuest.goalRange.y);
        targetName = potentialQuest.targetName;
        currentProgress = 0;
        gold = potentialQuest.goldAmount;
        expAmount = potentialQuest.expAmount;
    }
}

[Serializable]
public class QuestDataWrapper
{
    public List<QuestData> quests;

    public QuestDataWrapper()
    {
        quests = new List<QuestData>();
    }
}