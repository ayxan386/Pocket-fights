using TMPro;
using UnityEngine;

public class InfoRowIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI value;

    public void UpdateDisplay(string title, string value)
    {
        this.title.text = title;
        this.value.text = value;
    }
}