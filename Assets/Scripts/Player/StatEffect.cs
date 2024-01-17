using System;
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

    [ContextMenu("Add status")]
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

    public string GetDescription()
    {
        var suffix = "";
        switch (affectedValue)
        {
            case StatValue.Health:
                break;
            case StatValue.Mana:
                break;
            case StatValue.BaseAttack:
                break;
            case StatValue.DamageReduction:
                break;
            case StatValue.ManaRegen:
                break;
            case StatValue.None:
                break;
            case StatValue.DamageBuffer:
                suffix = $"Can block {DamageBuffer} DMG";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return displayDetails.descriptionBase + "\n" + suffix;
    }
}