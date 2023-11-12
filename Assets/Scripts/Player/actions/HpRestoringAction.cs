using UnityEngine;

public class HpRestoringAction : MonoBehaviour
{
    [SerializeField] private int amount;

    public void Use()
    {
        PlayerInputController.Instance.Stats.UpdateStatValue(StatValue.Health, amount);
    }
}