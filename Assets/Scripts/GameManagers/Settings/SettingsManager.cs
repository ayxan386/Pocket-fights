using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private List<GameSetting> settingRefs;

    void Start()
    {
        foreach (var setting in settingRefs)
        {
            setting.DefaultUpdate();
        }
    }
}

public abstract class GameSetting : MonoBehaviour
{
    public abstract void DefaultUpdate();
}