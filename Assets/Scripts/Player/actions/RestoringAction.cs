using UnityEngine;

public class RestoringAction : MonoBehaviour
{
    [SerializeField] private int amount;
    [SerializeField] private StatValue statValue = StatValue.Health;



    public void Use()
    {
        PlayerInputController.Instance.Stats.UpdateStatValue(statValue, amount);
    }
}