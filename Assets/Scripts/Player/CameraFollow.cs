using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float allowedMaxDistance;
    [SerializeField] private float orthoSize;
    [SerializeField] private Camera camera;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => camera != null);
        orthoSize = camera.orthographicSize;
    }

    private void Update()
    {
        if (target == null) return;
        var desiredPosition = target.position + offset;
        if (Vector3.Distance(transform.position, desiredPosition) > allowedMaxDistance)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
        }
    }

    public void ResetOrtho()
    {
        if (camera == null) return;
        camera.orthographicSize = orthoSize;
    }
}