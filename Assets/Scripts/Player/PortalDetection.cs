using UnityEngine;

public class PortalDetection : MonoBehaviour
{
    [SerializeField] private LayerMask portalLayer;
    [SerializeField] private float detectionRadius;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool shouldCheck = true;

    private void FixedUpdate()
    {
        if (!shouldCheck) return;
        startPosition = transform.position + Vector3.up / 2;
        endPosition = startPosition - Vector3.up;
        var isTherePortal = Physics.CheckCapsule(startPosition, endPosition, detectionRadius, portalLayer);

        if (isTherePortal)
        {
            shouldCheck = false;
            EventManager.OnExitPortalDetected?.Invoke(true);
        }
    }
}