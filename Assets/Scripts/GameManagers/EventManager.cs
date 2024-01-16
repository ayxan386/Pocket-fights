using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action<bool> OnPlayerTurnEnd;
    public static Action<float> OnBaseStatUpdate;
    public static Action<int> OnPlayerCoreUpdate;
    public static Action<StatValue, StatData> OnStatChanged;
    public static Action<int> OnSaveStarted;
    public static Action<StatController> OnStatSave;
    public static Action<InventoryItem> OnItemAdd;
    public static Action<InventoryItem, LootItemPanel> OnItemAddAsLoot;
    public static Action<InventoryItem> OnItemRemove;
    public static Action<bool> OnCombatSceneLoading;
    public static Action<bool> OnPauseMenuToggled;
    public static Action<bool> OnShopToggled;
    public static Action<MobController> OnMobDeath;

    public static Action<float> OnChangeSelection;
    public static Action<bool> OnPlayerVictory;

    //Skill related events
    public static Action<SkillCellManager, bool> OnSkillCellSelected;
    public static Action<SkillCellManager> OnSkillCellClicked;
    public static Action<SkillCellManager> OnSkillUpgraded;
}