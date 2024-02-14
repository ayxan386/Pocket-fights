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
    [SerializeField] private InventoryItem singleGoldPrefab;
    [SerializeField] private InventoryItem expStarPrefab;
    public static QuestManager Instance { get; private set; }

    public QuestDataWrapper WrappedData { get; set; }

    private List<QuestData> pendingUpdate;

    private void Awake()
    {
        Instance = this;
        pendingUpdate = new List<QuestData>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => WrappedData != null);

        potentialQuestSource.AddQuests(WrappedData.quests, targetCount);

        DataManager.Instance.SaveQuests();

        EventManager.OnMobDeath += OnMobDeath;
        EventManager.OnPlayerVictory += OnPlayerVictory;
    }

    private void OnDestroy()
    {
        EventManager.OnMobDeath -= OnMobDeath;
        EventManager.OnPlayerVictory -= OnPlayerVictory;
    }

    private void OnPlayerVictory(bool victory)
    {
        foreach (var questData in pendingUpdate)
        {
            questData.currentProgress += questData.pendingProgress;
            questData.pendingProgress = 0;

            if (questData.currentProgress == questData.goalCount)
            {
                questData.completed = true;
            }
        }

        pendingUpdate.Clear();
    }


    private void OnMobDeath(MobController deadMob)
    {
        var quests = WrappedData.quests;

        foreach (var quest in quests)
        {
            if (deadMob.Tags.Contains(quest.targetName) &&
                quest.currentProgress + quest.pendingProgress < quest.goalCount)
            {
                quest.pendingProgress++;
                pendingUpdate.Add(quest);
                break;
            }
        }
    }

    public void OpenUi(Transform targetParent, bool shouldFilterForAcceptance = false)
    {
        for (var index = 0; index < targetParent.childCount; index++)
        {
            Destroy(targetParent.GetChild(index).gameObject);
        }

        var quests = WrappedData.quests;
        foreach (var quest in quests)
        {
            if (shouldFilterForAcceptance && !quest.inProgress) continue;
            var questUi = Instantiate(questUiPrefab, targetParent);
            questUi.UpdateDisplay(quest, !shouldFilterForAcceptance);

            if (questUi.SelectionRef.interactable)
            {
                questUi.SelectionRef.Select();
            }
        }
    }

    public void AcceptQuest(QuestData questRef, QuestUi questUi)
    {
        questRef.inProgress = true;
        questUi.UpdateDisplay(questRef);
        SelectOtherQuest(questUi);

        DataManager.Instance.SaveQuests();
    }

    private static void SelectOtherQuest(QuestUi questUi)
    {
        var questHolder = questUi.transform.parent;
        for (var i = 0; i < questHolder.childCount; i++)
        {
            if (questHolder.GetChild(i).TryGetComponent(out QuestUi ui) && ui != questUi &&
                ui.SelectionRef.interactable)
            {
                ui.SelectionRef.Select();
                return;
            }
        }
    }

    public void ClaimQuestRewards(QuestData questRef, QuestUi questUi)
    {
        if (questRef.gold > 0)
        {
            var goldItem = Instantiate(singleGoldPrefab, transform);
            goldItem.count = questRef.gold;
            EventManager.OnItemAdd?.Invoke(goldItem);
        }

        if (questRef.expAmount > 0)
        {
            var expStar = Instantiate(expStarPrefab, transform);
            expStar.count = questRef.expAmount;
            EventManager.OnItemAdd?.Invoke(expStar);
        }

        WrappedData.quests.Remove(questRef);
        Destroy(questUi.gameObject);
    }
}

[Serializable]
public class QuestData
{
    public string id;
    public string title;
    public int goalCount;
    public int currentProgress;
    public int pendingProgress;
    public string targetName;
    public int gold;
    public int expAmount;
    public bool inProgress;
    public bool completed;

    public QuestData(PotentialQuest potentialQuest)
    {
        id = Guid.NewGuid().ToString();
        targetName = potentialQuest.targetName;
        title = potentialQuest.title;
        goalCount = Random.Range(potentialQuest.goalRange.x, potentialQuest.goalRange.y);
        gold = potentialQuest.goldAmount * goalCount;
        expAmount = potentialQuest.expAmount * goalCount;
        currentProgress = 0;
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