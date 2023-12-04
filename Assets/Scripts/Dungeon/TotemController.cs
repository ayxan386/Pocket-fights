using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TotemController : MonoBehaviour
{
    [Header("Entity detection")]
    [SerializeField] private float radius;

    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float effectRate;
    [SerializeField] private int effectCountLimit;

    [Space(10)]
    [SerializeField] private UnityEvent effects;

    [SerializeField] private UnityEvent exhaustActions;

    [Header("Effects")]
    [SerializeField] private GameObject particleEffects;

    private GameObject particles;
    private Transform playerTransformRef;
    private bool canApply;
    private bool exhausted = false;

    private void OnEnable()
    {
        StartCoroutine(ApplyEffect());
    }

    private void Start()
    {
        playerTransformRef = PlayerInputController.Instance.transform;
    }

    private void FixedUpdate()
    {
        canApply = effectCountLimit > 0 && Physics.CheckSphere(transform.position, radius, detectionLayer);

        if (canApply && particleEffects != null && particles == null)
        {
            particles = Instantiate(particleEffects, playerTransformRef.position, Quaternion.identity,
                playerTransformRef);
        }
        else if (!canApply && particles != null)
        {
            Destroy(particles);
        }

        if (!exhausted && effectCountLimit <= 0)
        {
            exhausted = true;
            exhaustActions.Invoke();
        }
    }

    private IEnumerator ApplyEffect()
    {
        while (true)
        {
            yield return new WaitUntil(() => canApply);
            effects.Invoke();
            effectCountLimit--;
            yield return new WaitForSeconds(effectRate);

            if (exhausted)
                yield break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}