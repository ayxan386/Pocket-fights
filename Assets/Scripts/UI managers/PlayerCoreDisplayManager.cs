using System.Collections.Generic;
using UnityEngine;

public class PlayerCoreDisplayManager : MonoBehaviour
{
    [SerializeField] private List<StatInfoDisplayController> displayControllers;

    private void Start()
    {
        EventManager.OnPlayerCoreUpdate += OnPlayerCoreUpdate;
        OnPlayerCoreUpdate(0);
    }

    private void OnPlayerCoreUpdate(int newValue)
    {
        displayControllers[0].UpdateDisplay("LVL", PlayerInputController.Instance.Stats.Level.ToString());
        displayControllers[1].UpdateDisplay("Lsf", PlayerInputController.Instance.Stats.Lsf.ToString("N2"));
        displayControllers[2].UpdateDisplay("Free Points", PlayerInputController.Instance.Stats.FreePoints.ToString());
        displayControllers[3]
            .UpdateDisplay("Skill Points", PlayerInputController.Instance.Stats.SkillPoints.ToString());
    }
}