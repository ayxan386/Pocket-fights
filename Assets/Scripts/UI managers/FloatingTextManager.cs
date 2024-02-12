using TMPro;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [field: SerializeField] public TextMeshPro floatingText { get; set; }
    [SerializeField] private Quaternion rightAngle, leftAngle;
    [SerializeField] private Vector3 movementDir;
    [SerializeField] private float lifespan;

    private void Start()
    {
        Destroy(gameObject, lifespan);
    }

    private void Update()
    {
        transform.Translate(movementDir * Time.deltaTime);
    }

    public void SetDirection(bool rightToLeft)
    {
        floatingText.transform.rotation = rightToLeft ? rightAngle : leftAngle;
    }
}