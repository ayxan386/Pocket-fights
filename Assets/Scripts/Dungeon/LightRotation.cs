using System.Collections;
using UnityEngine;

public class LightRotation : MonoBehaviour
{
    [SerializeField] private float fullRotationTime = 10;
    [SerializeField] private string lightTagName;
    private Transform target;

    private IEnumerator Start()
    {
        Start:
        yield return new WaitForSeconds(1f);
        target = GameObject.FindWithTag(lightTagName).transform;

        if (target == null) goto Start;

        while (target != null && target.gameObject.activeInHierarchy)
        {
            target.Rotate(Vector3.up, 360 / fullRotationTime / 60 * Time.deltaTime, Space.World);
            yield return null;
        }

        goto Start;
    }
}