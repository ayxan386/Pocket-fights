using UnityEngine;

public class AnimationPlayingAction : BasicAction
{
    [SerializeField] private string animationName;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
    {
        PlayerInputController.Instance.Animator.SetTrigger(animationName);
    }
}