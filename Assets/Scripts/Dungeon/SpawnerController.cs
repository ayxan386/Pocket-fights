using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] private List<SpawnerData> possibleSpawnPatterns;
    [SerializeField] private SpawnerData data;
    [SerializeField] private float activationRange;

    [SerializeField] private List<MobController> spawnedMobs;
    [SerializeField] private LayerMask obstructionLayer;

    [SerializeField] private Animator boxAnimator;
    [SerializeField] private GameObject boxMainBody;
    private GameObject mobParent;

    public bool IsExhausted => !boxMainBody.activeSelf;

    private void Start()
    {
        mobParent = GameObject.FindWithTag("MobParent");
    }

    public SpawnerData SetSpawnerData()
    {
        data = Instantiate(possibleSpawnPatterns[Random.Range(0, possibleSpawnPatterns.Count)], transform);
        return data;
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitUntil(() => mobParent != null);
        while (CheckIfThereMobsLeft())
        {
            yield return new WaitUntil(() =>
                gameObject.activeSelf
                && Vector3.Distance(
                    PlayerInputController.Instance.transform.position, transform.position) <= activationRange
            );

            var count = spawnedMobs.Count(mob => mob.gameObject.activeSelf);
            for (int mobCount = count; mobCount < data.maxNumberOfMobs;)
            {
                var index = Random.Range(0, data.mobs.Count);
                if (data.numberOfMobsLeft[index] > 0)
                {
                    mobCount++;
                    var pos = FindRandomPos(0);
                    var newMob = Instantiate(data.mobs[index], pos, Quaternion.identity, mobParent.transform);
                    spawnedMobs.Add(newMob);
                    data.numberOfMobsLeft[index]--;
                }

                if (!CheckIfThereMobsLeft())
                {
                    SpawnerFinished();
                    yield break;
                }
            }

            yield return new WaitForSeconds(data.spawnerRate);
        }
    }

    private void SpawnerFinished()
    {
        boxAnimator.SetTrigger("destroy");
        boxMainBody.SetActive(false);
    }

    private bool CheckIfThereMobsLeft()
    {
        return data.numberOfMobsLeft.Exists(number => number > 0);
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