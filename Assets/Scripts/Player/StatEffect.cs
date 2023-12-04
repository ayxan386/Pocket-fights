using UnityEngine;

public class StatEffect : MonoBehaviour
{
    public int numberOfTurns;
    public float amount;
    public bool isMult;
    public StatValue statValue;
    private float LastAmount;

    public void AddPlayerStatusEffect()
    {
        PlayerInputController.Instance.Stats.StatusManager.AddStatusEffect(this);
    }

    public float GetAmount(float val)
    {
        LastAmount = isMult
            ? val * amount
            : amount;
        return LastAmount;
    }

    public float CurrentEffect()
    {
        return LastAmount;
    }
}