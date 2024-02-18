using UnityEngine;

public class SoundMakingAction : BasicAction
{
    [SerializeField] private UiSoundEffect soundEffect;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
    {
        soundEffect.Play(CombatModeGameManager.Instance.SkillEffectsAudio);
    }
}