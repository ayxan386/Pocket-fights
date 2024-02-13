using DG.Tweening;
using UnityEngine;

public class SlingShotPlayerAction : BasicAction
{
    [SerializeField] private float duration;
    [SerializeField] private Vector3 offset;

    private Vector3 initialPos;
    private Transform casterTransform;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
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