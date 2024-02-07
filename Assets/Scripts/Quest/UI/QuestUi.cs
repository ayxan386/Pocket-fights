using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUi : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;

    public void UpdateDisplay(QuestData quest)
    {
        // title.text = $"{quest.title} {quest.currentProgress}";
        title.text = string.Format(quest.title, quest.gold, quest.expAmount, quest.currentProgress, quest.goalCount);
    }
}