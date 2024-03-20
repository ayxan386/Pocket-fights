using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SimpleProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform source;
    [SerializeField] private UnityEvent onCompleteEvent;

    public void MoveToward(Transform target)
    {
        source.DOMove(target.position, speed).SetEase(Ease.Linear).onComplete += Complete;
    }

    private void Complete()
    {
        onCompleteEvent?.Invoke();
        // Destroy(source.gameObject);
    }
}