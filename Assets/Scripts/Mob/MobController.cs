using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    [SerializeField] private StatBarIndicator healthIndicator;
    [SerializeField] private Animator animator;
    [SerializeField] private List<ActionDetails> actions;
    [SerializeField] private StatController statController;
    public bool IsDoneAttack { get; private set; }


    public void ReceiveAttack(float baseDamage)
    {
        var actionDetails = actions.Find(action => action.type == ActionType.ReceiveAttack);
        animator.SetTrigger(actionDetails.animationName);
        statController.ReceiveAttack(baseDamage);
    }

    private void OnDestroy()
    {
        DeactivateCombatMode();
    }

    public void ActivateCombatMode()
    {
        // EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
    }

    public void DeactivateCombatMode()
    {
    }

    public void AttackPlayer()
    {
        var randomAction = Random.Range(0, actions.Count);
        while (actions[randomAction].type != ActionType.Attack)
        {
            randomAction = Random.Range(0, actions.Count);
        }


        var actionDetails = actions[randomAction];
        animator.SetTrigger(actionDetails.animationName);
        PlayerInputController.Instance.Stats.ReceiveAttack(statController.GetStat(StatValue.BaseAttack).currentValue);
        IsDoneAttack = true;
    }
}