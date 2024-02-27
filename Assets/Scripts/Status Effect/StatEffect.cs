using System;
using UnityEngine;
using UnityEngine.Events;

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
    public bool needsToBeDeleted = false;
    public bool checkAtTheEnd = true;
    public bool isAdditive = false;
    public DisplayDetails displayDetails;
    public StatusEffectDisplayManager RelatedDisplayManager;
    public UnityEvent<StatEffect, StatController> secondaryEffect;
    public UnityEvent<StatEffect, StatController> triggerEffects;
    public Color textColor = Color.black;

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
        if (baseValue == StatValue.None) LastAmount = amount;
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
            case StatusEffectType.StatusGiving:
                break;
            case StatusEffectType.DamageDealing:
                break;
            case StatusEffectType.TrueDamage:
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
                triggerEffects?.Invoke(this, relatedStats);
                break;
            case StatusEffectType.StatusGiving:
                secondaryEffect?.Invoke(this, relatedStats);
                triggerEffects?.Invoke(this, relatedStats);
                break;
            case StatusEffectType.TrueDamage:
                print("Reached");
                relatedStats.UpdateStatValue(StatValue.Health,
                    (int)GetAmount(relatedStats.GetStatValue(baseValue).currentValue));
                triggerEffects?.Invoke(this, relatedStats);
                break;
            case StatusEffectType.DamageDealing:
                relatedStats.ReceiveAttack(GetAmount(0));
                triggerEffects?.Invoke(this, relatedStats);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (RelatedDisplayManager != null)
            RelatedDisplayManager.UpdateDisplay(this);
    }
}

public enum StatusEffectType
{
    None,
    DamageBuffer,
    Healing,
    StatusGiving,
    DamageDealing,
    TrueDamage
}