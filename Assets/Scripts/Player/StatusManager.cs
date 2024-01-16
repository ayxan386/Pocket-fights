using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private List<StatEffect> statusEffects;

    public StatController RelatedStats { get; set; }

    private void Awake()
    {
        statusEffects = new List<StatEffect>();
    }

    private void Start()
    {
        EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
        EventManager.OnPlayerVictory += OnPlayerVictory;
        EventManager.OnBaseStatUpdate += OnBaseStatUpdate;
    }

    private void OnDestroy()
    {
        EventManager.OnPlayerTurnEnd -= OnPlayerTurnEnd;
        EventManager.OnPlayerVictory -= OnPlayerVictory;
        EventManager.OnBaseStatUpdate -= OnBaseStatUpdate;
    }

    private void OnPlayerVictory(bool obj)
    {
        foreach (var statusEffect in statusEffects)
        {
            if (!statusEffect.isDamageBased) continue;

            RemoveStatusEffect(statusEffect);
        }

        RemoveUselessEffects();
    }


    private void OnBaseStatUpdate(float obj)
    {
        ReapplyAllEffect();
    }

    private void OnPlayerTurnEnd(bool obj)
    {
        foreach (var statusEffect in statusEffects)
        {
            if (statusEffect.isDamageBased) continue;

            statusEffect.numberOfTurns--;

            if (statusEffect.numberOfTurns <= 0)
            {
                RemoveStatusEffect(statusEffect);
            }
        }

        RemoveUselessEffects();
    }

    public void ReapplyAllEffect()
    {
        foreach (var effect in statusEffects)
        {
            var currentEffect = effect.CurrentEffect();
            RelatedStats.BoostStatValue(effect.affectedValue,
                effect.GetAmount(RelatedStats.GetStatValue(effect.baseValue).baseValue) - currentEffect);
        }
    }

    public void AddStatusEffect(StatEffect effect)
    {
        RelatedStats.BoostStatValue(effect.affectedValue,
            effect.GetAmount(RelatedStats.GetStatValue(effect.baseValue).baseValue));

        statusEffects.Add(effect);

        RelatedStats.UpdateOverallDisplay();
    }

    private void RemoveStatusEffect(StatEffect effect)
    {
        RelatedStats.BoostStatValue(effect.affectedValue, -effect.CurrentEffect());
        RelatedStats.UpdateOverallDisplay();

        if (effect.gameObject.name.ContainsInsensitive("clone"))
        {
            Destroy(effect.gameObject);
        }
    }

    public float CheckForDamage(float receivedDamage)
    {
        foreach (var statusEffect in statusEffects)
        {
            if (!statusEffect.isDamageBased || statusEffect.DamageBuffer <= 0) continue;

            if (receivedDamage <= 0) break;
            var diff = Mathf.Min(receivedDamage, statusEffect.DamageBuffer);
            statusEffect.DamageBuffer -= diff;

            if (statusEffect.affectedValue == StatValue.DamageBuffer)
                receivedDamage -= diff;

            CheckRemainingAmount(statusEffect);
        }

        return receivedDamage;
    }

    private void CheckRemainingAmount(StatEffect statusEffect)
    {
        if (statusEffect.DamageBuffer <= 0)
        {
            RemoveStatusEffect(statusEffect);
            RemoveUselessEffects();
        }
    }


    private void RemoveUselessEffects()
    {
        statusEffects = statusEffects.FindAll(effect => (effect.isDamageBased && effect.DamageBuffer > 0)
                                                        || effect.numberOfTurns > 0);
    }
}