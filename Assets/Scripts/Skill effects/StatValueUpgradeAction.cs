using UnityEngine;

public class StatValueUpgradeAction : MonoBehaviour
{
    [SerializeField] private StatValue stat;

    public void ApplyEffect(Skill usedSkill, StatController caster, StatController target)
    {
        caster.BoostStatValue(stat, caster.GetStatValue(stat).currentValue * (usedSkill.multiplier - 1), true);
    }
}