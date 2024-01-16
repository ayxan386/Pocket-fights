using UnityEngine;

public class StdDamageDealingAction : MonoBehaviour
{
    public void ApplyEffect(Skill usedSkill, StatController caster, StatController target)
    {
        target.ReceiveAttack(caster.GetStatValue(StatValue.BaseAttack).currentValue * usedSkill.multiplier);
    }
}