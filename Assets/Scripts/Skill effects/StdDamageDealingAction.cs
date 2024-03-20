using UnityEngine;

public class StdDamageDealingAction : MonoBehaviour
{
    [SerializeField] private float conversionFactor = 1f;

    public void ApplyEffect(Skill usedSkill, StatController caster, StatController target)
    {
        target.ReceiveAttack(caster.GetStatValue(StatValue.BaseAttack).currentValue * usedSkill.multiplier *
                             conversionFactor);
    }
}