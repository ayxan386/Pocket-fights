using TMPro;
using UnityEngine;

public class StatInfoDisplayController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI value;

    public StatTypes StatType { get; set; }

    public void UpdateDisplay(string title, string value)
    {
        this.title.text = title;
        this.value.text = value;
    }

    public void Clicked()
    {
        PlayerInputController.Instance.Stats.UpgradeStat(StatType, 1);
    }
}