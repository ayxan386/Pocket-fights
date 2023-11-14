using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] private SpawnerData data;
    [SerializeField] private float activationRange;

    [SerializeField] private List<MobController> spawnedMobs;

    private IEnumerator Start()
    {
        while (data.numberOfMobsLeft.TrueForAll(number => number > 0))
        {
            yield return new WaitUntil(() =>
                Vector3.Distance(
                    PlayerInputController.Instance.transform.position, transform.position) <= activationRange
            );

            var count = spawnedMobs.Count(mob => mob.gameObject.activeSelf);
            for (int i = count; i < data.maxNumberOfMobs;)
            {
                var index = Random.Range(0, data.mobs.Count);
                if (data.numberOfMobsLeft[index] > 0)
                {
                    i++;

                    var newMob = Instantiate(data.mobs[index], transform);
                    spawnedMobs.Add(newMob);
                }
            }

            yield return new WaitForSeconds(data.spawnerRate);
        }
    }
}