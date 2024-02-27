using UnityEngine;

public class TurnAmountRelation : BaseTriggerEffect
{
    [SerializeField] private bool negate = true;

    protected override void MainEffect(StatEffect baseEffect, StatController effectHolder)
    {
        baseEffect.amount = (negate ? -1 : 1) * baseEffect.numberOfTurns;
        baseEffect.GetAmount(effectHolder.GetStatValue(baseEffect.baseValue).currentValue);
    }
}