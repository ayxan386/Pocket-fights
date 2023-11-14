using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] private SpawnerData data;
    [SerializeField] private float activationRange;

    [SerializeField] private List<MobController> spawnedMobs;
    [SerializeField] private LayerMask obstructionLayer;

    private IEnumerator Start()
    {
        while (data.numberOfMobsLeft.TrueForAll(number => number > 0))
        {
            yield return new WaitUntil(() =>
                gameObject.activeSelf
                && Vector3.Distance(
                    PlayerInputController.Instance.transform.position, transform.position) <= activationRange
            );

            var count = spawnedMobs.Count(mob => mob.gameObject.activeSelf);
            for (int i = count; i < data.maxNumberOfMobs;)
            {
                var index = Random.Range(0, data.mobs.Count);
                if (data.numberOfMobsLeft[index] > 0)
                {
                    i++;
                    var pos = FindRandomPos(0);
                    var newMob = Instantiate(data.mobs[index], pos, Quaternion.identity, transform);
                    spawnedMobs.Add(newMob);
                }
            }

            yield return new WaitForSeconds(data.spawnerRate);
        }
    }

    private Vector3 FindRandomPos(int depth)
    {
        if (depth > 250) throw new StackOverflowException();
        var offset = Random.insideUnitCircle * activationRange;
        var pos = transform.position;
        pos.x += offset.x;
        pos.z += offset.y;
        return !Physics.CheckSphere(pos, 0.3f, obstructionLayer) ? pos : FindRandomPos(depth + 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(FindRandomPos(0), 0.3f);
    }
}