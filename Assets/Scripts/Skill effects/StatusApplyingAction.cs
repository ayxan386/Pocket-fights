using UnityEngine;

public class StatusApplyingAction : MonoBehaviour
{
    [SerializeField] private StatEffect effect;
    [SerializeField] private bool negate = false;

    public void ApplyEffectToCaster(Skill usedSkill, StatController caster, StatController target)
    {
        var statEffect = Instantiate(effect, transform);
        statEffect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        statEffect.amount = (negate ? -1 : 1) * usedSkill.multiplier;
        caster.StatusManager.AddStatusEffect(statEffect);
    }

    public void ApplyEffectToTarget(Skill usedSkill, StatController caster, StatController target)
    {
        effect.amount = (negate ? -1 : 1) * usedSkill.multiplier;
        target.StatusManager.AddStatusEffect(Instantiate(effect, transform));
    }

    public void ApplyEffectToCaster(StatEffect usedSkill, StatController caster)
    {
        var statEffect = Instantiate(effect, caster.StatusManager.transform);
        statEffect.amount *= usedSkill.LastAmount;
        caster.StatusManager.AddStatusEffect(statEffect);
    }
}