using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private List<StatEffect> statusEffects;
    [SerializeField] private Transform statusDisplayParent;
    [SerializeField] private StatusEffectDisplayManager statusEffectDisplayPrefab;

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
            statusEffect.TriggerEffect(RelatedStats);

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
        statusDisplayParent.gameObject.SetActive(true);
        var statusEffectDisplayManager = Instantiate(statusEffectDisplayPrefab, statusDisplayParent);
        statusEffectDisplayManager.UpdateDisplay(effect);
        effect.RelatedDisplayManager = statusEffectDisplayManager;

        RelatedStats.UpdateOverallDisplay();
    }

    private void RemoveStatusEffect(StatEffect effect)
    {
        RelatedStats.BoostStatValue(effect.affectedValue, -effect.CurrentEffect());
        RelatedStats.UpdateOverallDisplay();

        Destroy(effect.RelatedDisplayManager.gameObject);

        if (effect.gameObject.name.ContainsInsensitive("clone"))
        {
            Destroy(effect.gameObject);
        }

        statusDisplayParent.gameObject.SetActive(statusDisplayParent.childCount > 0);
    }

    public float CheckForDamage(float receivedDamage)
    {
        foreach (var statusEffect in statusEffects)
        {
            if (!statusEffect.isDamageBased || statusEffect.DamageBuffer <= 0) continue;

            if (receivedDamage <= 0) break;
            var diff = Mathf.Min(receivedDamage, statusEffect.DamageBuffer);
            statusEffect.DamageBuffer -= diff;

            if (statusEffect.effectType == StatusEffectType.DamageBuffer)
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