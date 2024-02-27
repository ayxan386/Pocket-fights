using UnityEngine;

public class ObjectSpawningAction : BasicAction
{
    [SerializeField] private GameObject objPrefab;
    [SerializeField] private float lifespan;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool onCaster = true;

    protected override void MainAction(Skill skill, StatController caster, StatController target)
    {
        var transformSource = onCaster ? caster.transform : target.transform;
        var ins = Instantiate(objPrefab,
            transformSource.position + offset,
            Quaternion.LookRotation(transformSource.forward),
            transformSource);

        if (lifespan > 0)
        {
            Destroy(ins, lifespan);
        }
    }
}