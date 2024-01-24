using UnityEngine;

public class SimpleTransformRotation : MonoBehaviour
{
    [SerializeField] private Vector3 speed;

    private void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
    }
}