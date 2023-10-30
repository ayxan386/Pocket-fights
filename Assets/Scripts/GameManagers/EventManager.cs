using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action<bool> OnPlayerTurnEnd;
    public static Action<float> OnBaseStatUpdate;
    public static Action<int> OnPlayerCoreUpdate;
}