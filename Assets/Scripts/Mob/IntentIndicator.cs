using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntentIndicator : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshPro attackTitle;

    public void UpdateIntent(MobTurnSequence turn)
    {
        icon.sprite = turn.intentIcon;
        icon.color = turn.intentColor;
    }

    public void UpdateTitle(MobTurnSequence turn)
    {
        attackTitle.gameObject.SetActive(true);
        attackTitle.text = turn.attackTitle;
        attackTitle.color = turn.titleColor;
    }

    public void HideTitle()
    {
        attackTitle.gameObject.SetActive(false);
    }
}