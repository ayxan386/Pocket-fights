using System.Collections.Generic;
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
        EventManager.OnBaseStatUpdate += OnBaseStatUpdate;
    }

    private void OnBaseStatUpdate(float obj)
    {
        ReapplyAllEffect();
    }

    private void OnPlayerTurnEnd(bool obj)
    {
        foreach (var statusEffect in statusEffects)
        {
            statusEffect.numberOfTurns--;

            if (statusEffect.numberOfTurns <= 0)
            {
                RemoveStatusEffect(statusEffect);
            }
        }

        statusEffects = statusEffects.FindAll(effect => effect.numberOfTurns > 0);
    }

    public void ReapplyAllEffect()
    {
        foreach (var effect in statusEffects)
        {
            var currentEffect = effect.CurrentEffect();
            RelatedStats.BoostStatValue(effect.statValue,
                effect.GetAmount(RelatedStats.GetStatValue(effect.statValue).baseValue) - currentEffect);
        }
    }

    public void AddStatusEffect(StatEffect effect)
    {
        RelatedStats.BoostStatValue(effect.statValue,
            effect.GetAmount(RelatedStats.GetStatValue(effect.statValue).baseValue));
        statusEffects.Add(effect);

        RelatedStats.UpdateOverallDisplay();
    }

    public void RemoveStatusEffect(StatEffect effect)
    {
        RelatedStats.BoostStatValue(effect.statValue,
            -effect.GetAmount(RelatedStats.GetStatValue(effect.statValue).baseValue));
        Destroy(effect.gameObject, 0.1f);

        RelatedStats.UpdateOverallDisplay();
    }
}