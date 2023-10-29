using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MobController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<ActionDetails> actions;
    [SerializeField] private StatController statController;

    [Header("AI parameters")] [SerializeField]
    private NavMeshAgent agent;

    [SerializeField] private Transform[] patrolPoints;

    public Guid Id { get; private set; }
    public bool IsDoneAttack { get; private set; }
    private bool isCombatModeActive = false;
    private int patrolPointIndex = 0;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        if (!isCombatModeActive) StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (!isCombatModeActive)
        {
            agent.SetDestination(patrolPoints[patrolPointIndex].position);
            yield return new WaitUntil(() =>
                Vector3.Distance(transform.position, patrolPoints[patrolPointIndex].position) <= 0.3);
            yield return new WaitForSeconds(0.4f);
            patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;
        }
    }

    public void ReceiveAttack(float baseDamage)
    {
        var actionDetails = actions.Find(action => action.type == ActionType.ReceiveAttack);
        animator.SetTrigger(actionDetails.animationName);
        statController.ReceiveAttack(baseDamage, OnDeathCallback);
    }

    private void OnDestroy()
    {
        DeactivateCombatMode();
    }

    public void ActivateCombatMode()
    {
        Id = Guid.NewGuid();
        isCombatModeActive = true;
        EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
    }


    public void DeactivateCombatMode()
    {
        isCombatModeActive = true;
        EventManager.OnPlayerTurnEnd -= OnPlayerTurnEnd;
    }

    private void OnPlayerTurnEnd(bool obj)
    {
        statController.RegenMana();
    }

    public void AttackPlayer()
    {
        var randomAction = Random.Range(0, actions.Count);
        while (actions[randomAction].type != ActionType.Attack)
        {
            randomAction = Random.Range(0, actions.Count);
        }


        var actionDetails = actions[randomAction];
        var usedAction = statController.UsedAction(actionDetails.manaConsumption);
        if (usedAction)
        {
            animator.SetTrigger(actionDetails.animationName);
            PlayerInputController.Instance.ReceiveAttack(
                statController.GetStat(StatValue.BaseAttack).currentValue);
        }

        IsDoneAttack = true;
    }

    private void OnDeathCallback()
    {
        CombatModeGameManager.Instance.MobDefeated(this);
    }
}