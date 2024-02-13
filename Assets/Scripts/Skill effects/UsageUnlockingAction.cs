using System.Collections;
using UnityEngine;

public class UsageUnlockingAction : MonoBehaviour
{
    public void ApplyEffectToCaster(Skill usedSkill, StatController caster, StatController target)
    {
        StartCoroutine(WaitForLock(usedSkill));
    }

    private IEnumerator WaitForLock(Skill usedSkill)
    {
        yield return new WaitUntil(() => !usedSkill.Lock);
        PlayerInputController.Instance.UsingAction = false;
    }
}