using UnityEngine;

public class StatusApplyingAction : MonoBehaviour
{
    [SerializeField] private StatEffect effect;

    public void ApplyEffectToCaster(Skill usedSkill, StatController caster, StatController target)
    {
        effect.amount = usedSkill.multiplier;
        caster.StatusManager.AddStatusEffect(Instantiate(effect, transform));
    }

    public void ApplyEffectToTarget(Skill usedSkill, StatController caster, StatController target)
    {
        effect.amount = usedSkill.multiplier;
        target.StatusManager.AddStatusEffect(Instantiate(effect, transform));
    }

    public void ApplyEffectToCaster(StatEffect usedSkill, StatController caster)
    {
        var statEffect = Instantiate(effect, caster.StatusManager.transform);
        statEffect.amount *= usedSkill.LastAmount;
        caster.StatusManager.AddStatusEffect(statEffect);
    }
}