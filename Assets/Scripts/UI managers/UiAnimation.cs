using DG.Tweening;
using UnityEngine;

public class UiAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform transformRef;
    [SerializeField] private Vector3 appearLocation;
    [SerializeField] private Vector3 disappearLocation;
    [SerializeField] private float duration;

    [ContextMenu("Appear")]
    public void Appear()
    {
        transformRef.DOAnchorPos(appearLocation, duration);
    }

    [ContextMenu("Disappear")]
    public void Disappear()
    {
        transformRef.DOAnchorPos(disappearLocation, duration);
    }

    public void ScaleAppear()
    {
        transformRef.DOScale(appearLocation, duration);
    }

    public void ScaleDisappear()
    {
        transformRef.DOScale(disappearLocation, duration);
    }
}