using UnityEngine;

public class UsageUnlockingAction : MonoBehaviour
{
    public void ApplyEffectToCaster(Skill usedSkill, StatController caster, StatController target)
    {
        PlayerInputController.Instance.UsingAction = false;
    }
}