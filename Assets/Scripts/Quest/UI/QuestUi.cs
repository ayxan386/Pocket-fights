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

    private QuestData questRef;

    public void UpdateDisplay(QuestData quest)
    {
        if (quest == null) return;
        questRef = quest;
        title.text = quest.title;
        progressText.text = $"{quest.currentProgress}/{quest.goalCount}";
        goldReward.text = "" + quest.gold;
        expReward.text = "" + quest.expAmount;
        topButton.interactable = !quest.inProgress;
    }

    public void AcceptQuest()
    {
        QuestManager.Instance.AcceptQuest(questRef, this);
    }
}