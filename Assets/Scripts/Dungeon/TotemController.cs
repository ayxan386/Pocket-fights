using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TotemController : MonoBehaviour
{
    [Header("Entity detection")]
    [SerializeField] private float radius;

    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float effectRate;

    [Space(10)]
    [SerializeField] private UnityEvent effects;

    private bool isInArea;

    private void OnEnable()
    {
        StartCoroutine(ApplyEffect());
    }

    private void FixedUpdate()
    {
        isInArea = Physics.CheckSphere(transform.position, radius, detectionLayer);
    }

    private IEnumerator ApplyEffect()
    {
        while (true)
        {
            yield return new WaitUntil(() => isInArea);
            effects.Invoke();
            yield return new WaitForSeconds(effectRate);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}