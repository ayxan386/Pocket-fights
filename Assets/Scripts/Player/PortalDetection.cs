using System;
using UnityEngine;

public class PortalDetection : MonoBehaviour
{
    [SerializeField] private LayerMask portalLayer;
    [SerializeField] private float detectionRadius;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        startPosition = transform.position + Vector3.up / 2;
        endPosition = startPosition - Vector3.up;
    }

    private void FixedUpdate()
    {
        var isTherePortal = Physics.CheckCapsule(startPosition, endPosition, detectionRadius, portalLayer);

        if (isTherePortal)
        {
            EventManager.OnExitPortalDetected?.Invoke(true);
        }
    }
}