using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform transformRef;
    [SerializeField] private Vector3 appearLocation;
    [SerializeField] private Vector3 disappearLocation;
    [SerializeField] private float duration;
    [SerializeField] private GameObject appearanceSelection;

    [ContextMenu("Appear")]
    public void Appear()
    {
        transformRef.DOAnchorPos(appearLocation, duration);
        CheckForSelection();
    }

    private void CheckForSelection()
    {
        if (appearanceSelection != null)
        {
            EventSystem.current.SetSelectedGameObject(appearanceSelection);
        }
    }

    [ContextMenu("Disappear")]
    public void Disappear()
    {
        transformRef.DOAnchorPos(disappearLocation, duration);
    }

    public void ScaleAppear()
    {
        transformRef.DOScale(appearLocation, duration);
        CheckForSelection();
    }

    public void ScaleDisappear()
    {
        transformRef.DOScale(disappearLocation, duration);
    }
}