using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private Transform skillHolder;
    [SerializeField] private List<Skill> actions;

    public List<Skill> AllSkills => actions.FindAll(action => action.type != ActionType.ReceiveAttack);

    public static PlayerActionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        actions = new List<Skill>();
    }

    private void Start()
    {
        actions = skillHolder.GetComponentsInChildren<Skill>().ToList();
    }

    public Skill GetAction(int actionIndex)
    {
        return actions[actionIndex];
    }
}