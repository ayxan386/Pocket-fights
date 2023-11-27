using UnityEngine;

public class DeletionAction : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Component targetComponent;

    public void Activate()
    {
        if (target)
            Destroy(target);
        if (targetComponent)
            Destroy(targetComponent);
    }
}