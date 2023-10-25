using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    [SerializeField] private StatBarIndicator healthIndicator;
    [SerializeField] private Animator animator;
    [SerializeField] private List<ActionDetails> actions;
    public bool IsDoneAttack { get; private set; }


    public void ReceiveAttack(float baseDamage)
    {
        var actionDetails = actions.Find(action => action.type == ActionType.ReceiveAttack);
        animator.SetTrigger(actionDetails.animationName);
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
        print("Player turn ended trying to attack");
        var randomAction = Random.Range(0, actions.Count);
        while (actions[randomAction].type != ActionType.Attack)
        {
            randomAction = Random.Range(0, actions.Count);
        }

        print("Attacking using " + randomAction);

        var actionDetails = actions[randomAction];
        animator.SetTrigger(actionDetails.animationName);
        PlayerStatController.Instance.ReceiveAttack(new StatData());
        IsDoneAttack = true;
    }
}