using DG.Tweening;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform source;

    public void MoveToward(Transform target)
    {
        source.DOMove(target.position, speed).onComplete += () => Destroy(source.gameObject);
    }
}