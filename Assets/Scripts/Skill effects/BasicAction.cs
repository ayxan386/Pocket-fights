using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasicAction : MonoBehaviour
{
    [SerializeField] private float delayNext;
    public UnityEvent<Skill, StatController, StatController> usageEffects;

    public void ApplyEffectToCaster(Skill usedSkill, StatController caster, StatController target)
    {
        StartCoroutine(DelayNextAction(usedSkill, caster, target, MainAction));
    }

    protected IEnumerator DelayNextAction(Skill usedSkill, StatController caster, StatController target,
        Action<Skill, StatController, StatController> action)
    {
        action.Invoke(usedSkill, caster, target);
        yield return new WaitForSeconds(delayNext);
        usageEffects?.Invoke(usedSkill, caster, target);
    }

    protected abstract void MainAction(Skill skill, StatController caster, StatController target);
}