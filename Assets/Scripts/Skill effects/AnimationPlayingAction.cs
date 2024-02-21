using UnityEngine;

public class AnimationPlayingAction : BasicAction
{
    [SerializeField] private string[] animationName;
    [SerializeField] private bool onCaster = true;

    private int lastUsedIndex;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
    {
        if (onCaster)
            caster.Animator.SetTrigger(animationName[lastUsedIndex]);
        else
            target.Animator.SetTrigger(animationName[lastUsedIndex]);

        lastUsedIndex = (lastUsedIndex + 1) % animationName.Length;
    }
}