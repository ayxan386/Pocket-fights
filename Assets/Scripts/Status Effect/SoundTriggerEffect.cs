using UnityEngine;

public class SoundTriggerEffect : BaseTriggerEffect
{
    [SerializeField] private UiSoundEffect soundEffect;

    protected override void MainEffect(StatEffect baseEffect, StatController effectHolder)
    {
        soundEffect.Play(CombatModeGameManager.Instance.SkillEffectsAudio);
    }
}