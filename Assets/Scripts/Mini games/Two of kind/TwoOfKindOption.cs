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

    private void Start()
    {
        startRotation = transform.eulerAngles;
    }

    public void Uncover()
    {
        coverPivot.DORotate(targetRotation, rotationDuration);

        Manager.OptionOpened(this);
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