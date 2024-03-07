using System.Collections.Generic;
using UnityEngine;

public class PlayerCoreDisplayManager : MonoBehaviour
{
    [SerializeField] private List<InfoBlockBundle> infoBlocks;

    private void Start()
    {
        EventManager.OnPlayerCoreUpdate += OnPlayerCoreUpdate;
    }

    private void OnDestroy()
    {
        EventManager.OnPlayerCoreUpdate -= OnPlayerCoreUpdate;
    }

    private void OnPlayerCoreUpdate(int newValue)
    {
        var player = PlayerInputController.Instance;
        var stats = player.Stats;
        for (int i = 0; i < infoBlocks.Count; i++)
        {
            infoBlocks[i].lvl.UpdateDisplay("LVL", stats.Level.ToString());
            infoBlocks[i].lsf.UpdateDisplay("Lsf", stats.Lsf.ToString("N2"));
            infoBlocks[i].freePoints.UpdateDisplay("Free Points", stats.FreePoints.ToString());
            infoBlocks[i].skillPoints.UpdateDisplay("Skill Points", stats.SkillPoints.ToString());
            infoBlocks[i].gold.UpdateDisplay("Gold", InventoryController.Instance.Gold.ToString());
            infoBlocks[i].xp.UpdateDisplay("XP", (player.LevelUpProgress * 100).ToString("N1") + "%");
        }
    }
}