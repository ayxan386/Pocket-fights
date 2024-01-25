using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TwoOfKindOption : MonoBehaviour
{
    [SerializeField] private Transform prizeParent;
    [SerializeField] private Transform coverPivot;
    [SerializeField] private Vector3 startRotation;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private float rotationDuration;


    public PossibleLoot PrizeName { get; set; }
    public TwoOfKindManager Manager { get; set; }

    private IEnumerator Start()
    {
        startRotation = transform.eulerAngles;
        yield return new WaitUntil(() => PrizeName != null);

        Instantiate(PrizeName.inWorldDisplayItem, prizeParent);
    }

    public void Uncover()
    {
        coverPivot.DORotate(targetRotation, rotationDuration).OnComplete(() => Manager.OptionOpened(this));
    }

    public void Cover()
    {
        coverPivot.DORotate(startRotation, rotationDuration);
    }

    private void OnMouseDown()
    {
        Uncover();
    }
}