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
    public StatusEffectType effectType;
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
        switch (effectType)
        {
            case StatusEffectType.None:
                break;
            case StatusEffectType.DamageBuffer:
                suffix = $"Can block {DamageBuffer} DMG";
                break;
            case StatusEffectType.Healing:
                suffix = $"Heals {LastAmount:N0} HP";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return displayDetails.descriptionBase + "\n" + suffix;
    }

    public void TriggerEffect(StatController relatedStats)
    {
        switch (effectType)
        {
            case StatusEffectType.None:
                break;
            case StatusEffectType.DamageBuffer:
                break;
            case StatusEffectType.Healing:
                relatedStats.UpdateStatValue(StatValue.Health, (int)LastAmount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        RelatedDisplayManager.UpdateDisplay(this);
    }
}

public enum StatusEffectType
{
    None,
    DamageBuffer,
    Healing
}