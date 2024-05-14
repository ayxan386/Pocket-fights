using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class SlingShotPlayerAction : BasicAction
{
    [SerializeField] private float duration;
    [SerializeField] private Vector3 offset;

    private Vector3 initialPos;
    private Transform casterTransform;
    private TweenerCore<Vector3, Vector3, VectorOptions> movement;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
    {
        if (casterTransform == null)
        {
            casterTransform = caster.transform;
            initialPos = casterTransform.position;
        }

        if (movement != null && movement.IsPlaying())
        {
            movement.Kill(); 
        }
        
        movement = casterTransform.DOMove(target.animationPosition.position + offset, duration);
        movement.OnComplete(OnComplete);
    }

    private void OnComplete()
    {
        casterTransform.DOMove(initialPos, duration);
    }
}