using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private List<StatEffect> statusEffects;
    [SerializeField] private HashSet<StatEffect> pendingEffects;
    [SerializeField] private HashSet<StatEffect> removalPendingEffects;
    [SerializeField] private Transform statusDisplayParent;
    [SerializeField] private StatusEffectDisplayManager statusEffectDisplayPrefab;

    public StatController RelatedStats { get; set; }

    private bool isIterating;

    private void Awake()
    {
        statusEffects = new List<StatEffect>();
        pendingEffects = new HashSet<StatEffect>();
        removalPendingEffects = new HashSet<StatEffect>();
    }

    private void Start()
    {
        EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
        EventManager.OnPlayerTurnStart += OnPlayerTurnStart;
        EventManager.OnPlayerVictory += OnPlayerVictory;
        EventManager.OnBaseStatUpdate += OnBaseStatUpdate;
    }

    private void OnDestroy()
    {
        EventManager.OnPlayerTurnEnd -= OnPlayerTurnEnd;
        EventManager.OnPlayerTurnStart -= OnPlayerTurnStart;
        EventManager.OnPlayerVictory -= OnPlayerVictory;
        EventManager.OnBaseStatUpdate -= OnBaseStatUpdate;
    }


    private void OnPlayerVictory(bool obj)
    {
        if (!gameObject.activeSelf) return;
        foreach (var statusEffect in statusEffects)
        {
            RemoveStatusEffect(statusEffect);
        }

        RemoveUselessEffects();
    }

    private void OnBaseStatUpdate(float obj)
    {
        if (!gameObject.activeSelf) return;
        ReapplyAllEffect();
    }

    private void OnPlayerTurnStart(bool obj)
    {
        if (!gameObject.activeSelf) return;
        UpdateStatusEffectTurns(effect => effect.checkAtTheEnd);
    }

    private void OnPlayerTurnEnd(bool obj)
    {
        if (!gameObject.activeSelf) return;
        UpdateStatusEffectTurns(effect => !effect.checkAtTheEnd);
    }

    private void UpdateStatusEffectTurns(Predicate<StatEffect> skippingValues)
    {
        isIterating = true;
        foreach (var statusEffect in statusEffects)
        {
            if (statusEffect.isDamageBased || skippingValues(statusEffect)) continue;

            statusEffect.numberOfTurns--;
            statusEffect.TriggerEffect(RelatedStats);

            if (statusEffect.numberOfTurns <= 0)
            {
                RemoveStatusEffect(statusEffect);
            }
        }

        isIterating = false;
        statusEffects.AddRange(pendingEffects);
        pendingEffects.Clear();

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
        effect.transform.SetParent(transform);
        BoostByEffect(effect);
        if (isIterating)
        {
            pendingEffects.Add(effect);
        }
        else
        {
            statusEffects.Add(effect);
        }

        if (statusDisplayParent != null)
        {
            statusDisplayParent.gameObject.SetActive(true);
            var statusEffectDisplayManager = Instantiate(statusEffectDisplayPrefab, statusDisplayParent);
            statusEffectDisplayManager.UpdateDisplay(effect);
            effect.RelatedDisplayManager = statusEffectDisplayManager;
        }

        RelatedStats.UpdateOverallDisplay();
    }

    private void BoostByEffect(StatEffect effect)
    {
        RelatedStats.BoostStatValue(effect.affectedValue,
            effect.GetAmount(RelatedStats.GetStatValue(effect.baseValue).baseValue));
    }

    private void RemoveStatusEffect(StatEffect effect)
    {
        if (effect == null || !statusEffects.Contains(effect)) return;

        RelatedStats.BoostStatValue(effect.affectedValue, -effect.CurrentEffect());
        RelatedStats.UpdateOverallDisplay();

        if (effect.RelatedDisplayManager != null)
            Destroy(effect.RelatedDisplayManager.gameObject);

        if (effect.needsToBeDeleted)
            removalPendingEffects.Add(effect);

        if (statusDisplayParent != null)
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
                                                        || (effect.numberOfTurns > 0
                                                            && !removalPendingEffects.Contains(effect)));

        foreach (var removalPendingEffect in removalPendingEffects)
        {
            Destroy(removalPendingEffect.gameObject);
        }

        removalPendingEffects.Clear();
        if (statusDisplayParent != null)
            statusDisplayParent.gameObject.SetActive(statusDisplayParent.childCount > 0);
    }
}