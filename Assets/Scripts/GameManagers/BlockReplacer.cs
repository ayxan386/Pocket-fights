using System.Collections;
using UnityEngine;

public class BlockReplacer : MonoBehaviour
{
    [SerializeField] private LayerMask layerToReplace;
    [SerializeField] private float checkRate;
    [SerializeField] private float checkRadius;
    [SerializeField] private GameObject replacementObject;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Vector3 offset;

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkRate);
                Checks();
        }
    }

    [ContextMenu("Do one iteration")]
    public void DoOneIteration()
    {
        Checks();
    }

    private void Checks()
    {
        if (Physics.Raycast(transform.position, Vector3.down,
                out RaycastHit hit, checkRadius, layerToReplace))
        {
            Destroy(hit.transform.gameObject);
            Instantiate(replacementObject, hit.transform.position + offset, Quaternion.identity, spawnParent);
        }
    }
}