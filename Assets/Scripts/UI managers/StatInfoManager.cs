using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatInfoManager : MonoBehaviour
{
    [SerializeField] private List<StatInfoDisplayController> statBlocks;

    private void OnEnable()
    {
        EventManager.OnBaseStatUpdate += OnBaseStatUpdate;
    }

    private void OnDisable()
    {
        EventManager.OnBaseStatUpdate -= OnBaseStatUpdate;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerInputController.Instance != null);
        OnBaseStatUpdate(0);
    }

    private void OnBaseStatUpdate(float obj)
    {
        int k = 0;
        foreach (var statType in Enum.GetValues(typeof(StatTypes)).Cast<StatTypes>())
        {
            var playerInputController = PlayerInputController.Instance;
            var baseStat = playerInputController.Stats.GetBaseStat(statType);
            statBlocks[k].UpdateDisplay(statType.ToString(), $"{baseStat.currentValue}/{baseStat.maxValue}");
            statBlocks[k++].StatType = statType;

            if (k == statBlocks.Count) break;
        }
    }
}