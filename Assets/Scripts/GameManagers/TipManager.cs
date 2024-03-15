using TMPro;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tipTitle;

    private static int lastIndex = 0;

    private void OnEnable()
    {
        var tips = Resources.Load<TextAsset>("tips").text.Split("\n");
        lastIndex = (lastIndex + 1) % tips.Length;

        var tip = tips[lastIndex];
        tipTitle.text = tip;
    }
}