using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnerData : MonoBehaviour
{
    public List<MobController> mobs;
    public List<int> numberOfMobsLeft;
    public MobController bossMonster;
    public float spawnerRate;
    public int maxNumberOfMobs;
}