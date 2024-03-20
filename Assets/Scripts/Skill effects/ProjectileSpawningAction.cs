using UnityEngine;

public class ProjectileSpawningAction : BasicAction
{
    [SerializeField] private SimpleProjectile objPrefab;
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

        ins.MoveToward(target.hitPosition);

        if (lifespan > 0)
        {
            Destroy(ins, lifespan);
        }
    }
}