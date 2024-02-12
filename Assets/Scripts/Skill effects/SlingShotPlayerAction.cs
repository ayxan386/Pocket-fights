using DG.Tweening;
using UnityEngine;

public class SlingShotPlayerAction : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Vector3 offset;
    private Vector3 initialPos;
    private Transform casterTransform;

    public void ApplyEffectToCaster(Skill usedSkill, StatController caster, StatController target)
    {
        casterTransform = caster.transform;
        initialPos = casterTransform.position;
        casterTransform.DOMove(target.animationPosition.position + offset, duration)
            .OnComplete(OnComplete);
    }

    private void OnComplete()
    {
        casterTransform.DOMove(initialPos, duration);
    }
}