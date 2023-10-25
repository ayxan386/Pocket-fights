using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private List<ActionDetails> actions;

    public static PlayerActionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public ActionDetails GetAction(int actionIndex)
    {
        return actions[actionIndex];
    }
}

[Serializable]
public class ActionDetails
{
    public string animationName;
    public float attackMult;
    public float manaConsumption;
    public ActionType type;
}

[Serializable]
public enum ActionType
{
    Attack,
    ReceiveAttack
}