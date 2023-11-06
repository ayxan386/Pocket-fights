using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action<bool> OnPlayerTurnEnd;
    public static Action<float> OnBaseStatUpdate;
    public static Action<int> OnPlayerCoreUpdate;
    public static Action<int> OnSaveStarted;
    public static Action<StatController> OnStatSave;
    public static Action<InventoryItem> OnItemAdd;
}