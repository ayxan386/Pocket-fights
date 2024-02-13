using UnityEngine;

public class AnimationPlayingAction : BasicAction
{
    [SerializeField] private string animationName;
    [SerializeField] private bool onCaster = true;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
    {
        if (onCaster)
            caster.Animator.SetTrigger(animationName);
        else
            target.Animator.SetTrigger(animationName);
    }
}