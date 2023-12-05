using TMPro;
using UnityEngine;

public class ExitConditionDisplayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mainText;

    public void UpdateDisplay(ExitConditionType conditionType, int killsLeft, SpawnerController sc)
    {
        switch (conditionType)
        {
            case ExitConditionType.KillCounter:
                if (killsLeft > 0)
                    mainText.text = $"Kills left: {killsLeft}";
                else
                    mainText.text = "Conditions fulfilled";
                break;
            case ExitConditionType.SpawnerExhaust:
                if (sc != null && !sc.IsExhausted)
                {
                    mainText.text = "Conditions fulfilled";
                }
                else
                {
                    mainText.text = $"Defeat all mobs to leave the room ({killsLeft})";
                }

                break;
            default:
                print("Not found");
                break;
        }
    }
}