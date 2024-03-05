using UnityEngine;

public class StatUpdateAction : BasicAction
{
    [SerializeField] private StatValue affectedValue;
    [SerializeField] private float mult;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
    {
        caster.UpdateStatValue(affectedValue, (int)(mult * skill.multiplier * caster.Lsf));
    }
}