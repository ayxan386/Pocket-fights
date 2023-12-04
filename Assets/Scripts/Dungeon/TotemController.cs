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

    [Header("Effects")]
    [SerializeField] private GameObject particleEffects;


    private GameObject particles;
    private Transform playerTransformRef;
    private bool isInArea;

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
        isInArea = Physics.CheckSphere(transform.position, radius, detectionLayer);

        if (isInArea && particleEffects != null && particles == null)
        {
            particles = Instantiate(particleEffects, playerTransformRef.position, Quaternion.identity,
                playerTransformRef);
        }
        else if (!isInArea && particles != null)
        {
            Destroy(particles);
        }
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