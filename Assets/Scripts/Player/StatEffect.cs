using UnityEngine;

public class StatEffect : MonoBehaviour
{
    public int numberOfTurns;
    public float amount;
    public bool isMult;
    public bool eachTurn;
    public StatTypes baseStat;
    public StatValue statValue;
    public StatEffectType type;
    private float finalAmount;

    public void AddPlayerStatusEffect()
    {
        PlayerInputController.Instance.Stats.AddStatusEffect(this);
    }

    public float GetAmount(float val)
    {
        finalAmount = isMult
            ? val * amount
            : amount;
        return finalAmount;
    }

    public float GetFinalAmount()
    {
        return finalAmount;
    }
}

public enum StatEffectType
{
    BaseStat,
    StatValue,
    Misc
}