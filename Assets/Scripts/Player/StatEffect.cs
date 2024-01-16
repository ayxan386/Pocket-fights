using UnityEngine;

public class StatEffect : MonoBehaviour
{
    public int numberOfTurns;
    public bool isDamageBased;
    public float amount;
    public bool isMult;
    public StatValue baseValue;
    public StatValue affectedValue;
    public float LastAmount;
    public float DamageBuffer;
    public bool isPositive;
    public DisplayDetails displayDetails;
    public StatusEffectDisplayManager RelatedDisplayManager;

    public void AddPlayerStatusEffect()
    {
        PlayerInputController.Instance.Stats.StatusManager.AddStatusEffect(this);
    }

    public float GetAmount(float val)
    {
        LastAmount = isMult
            ? val * amount
            : amount;
        if (isDamageBased) DamageBuffer = LastAmount;
        return LastAmount;
    }

    public float CurrentEffect()
    {
        return LastAmount;
    }
}