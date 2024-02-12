using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBarIndicator : MonoBehaviour
{
    [SerializeField] private Image filler;
    [SerializeField] private TextMeshProUGUI textIndicator;
    [SerializeField] private bool percentage;

    public void UpdateDisplay(StatData data)
    {
        filler.fillAmount = data.currentValue / data.maxValue;
        textIndicator.text = percentage ? $"{(int)(filler.fillAmount * 100)}%" : data.currentValue.ToString("N0");
    }
}