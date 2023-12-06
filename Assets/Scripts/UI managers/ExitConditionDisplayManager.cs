using TMPro;
using UnityEngine;

public class ExitConditionDisplayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mainText;

    public void UpdateDisplay(int killsLeft, bool canExit)
    {
        if (canExit)
        {
            mainText.text = "Conditions fulfilled";
            return;
        }

        mainText.text = $"Defeat all mobs to leave the room ({killsLeft})";
    }
}