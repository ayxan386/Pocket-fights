using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private List<ActionDetails> actions;
    [SerializeField] private GameObject selectedEnemy;

    public static PlayerActionManager Instance { get; private set; }
    public GameObject SelectedEnemy => selectedEnemy;

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
    public ActionType type;
}

[Serializable]
public enum ActionType
{
   Attack,
   ReceiveAttack
}