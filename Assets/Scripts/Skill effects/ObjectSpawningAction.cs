using UnityEngine;

public class ObjectSpawningAction : BasicAction
{
    [SerializeField] private GameObject objPrefab;
    [SerializeField] private float lifespan;
    [SerializeField] private Vector3 offset;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
    {
        var casterTransform = caster.transform;
        var ins = Instantiate(objPrefab,
            casterTransform.position + offset,
            Quaternion.LookRotation(casterTransform.forward),
            casterTransform);

        if (lifespan > 0)
        {
            Destroy(ins, lifespan);
        }
    }
}