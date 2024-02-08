using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUi : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI goldReward;
    [SerializeField] private TextMeshProUGUI expReward;
    [SerializeField] private Button topButton;
    [SerializeField] private GameObject rewardClaiming;
    [SerializeField] private Button claimingButton;

    private QuestData questRef;

    public void UpdateDisplay(QuestData quest, bool canClaim = false)
    {
        if (quest == null) return;
        questRef = quest;
        title.text = quest.title;
        progressText.text = $"{quest.currentProgress}/{quest.goalCount}";
        goldReward.text = "" + quest.gold;
        expReward.text = "" + quest.expAmount;
        // topButton.gameObject.SetActive(!quest.completed);
        rewardClaiming.SetActive(quest.completed);
        claimingButton.interactable = canClaim;
        topButton.interactable = !quest.inProgress;
    }

    public void AcceptQuest()
    {
        QuestManager.Instance.AcceptQuest(questRef, this);
    }

    public void ClaimRewards()
    {
        QuestManager.Instance.ClaimQuestRewards(questRef, this);
    }
}