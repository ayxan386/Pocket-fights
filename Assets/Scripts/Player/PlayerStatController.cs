using System.Collections.Generic;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    public static PlayerStatController Instance { get; private set; }
    private Dictionary<Stats, float> stats;

    private void Awake()
    {
        Instance = this;
        stats = new Dictionary<Stats, float>();
        stats[Stats.Vitality] = 4f;
        stats[Stats.Strength] = 4f;
        stats[Stats.Mana] = 3f;
        stats[Stats.Defense] = 4f;
    }

    public float GetStat(Stats statsType)
    {
        return stats[statsType];
    }
}

public enum Stats
{
    Vitality,
    Strength,
    Mana,
    Defense
}