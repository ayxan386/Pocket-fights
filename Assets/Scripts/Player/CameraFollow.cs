using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float allowedMaxDistance;

    private void LateUpdate()
    {
        var desiredPosition = target.position + offset;
        if (Vector3.Distance(transform.position, desiredPosition) > allowedMaxDistance)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
        }
    }
}