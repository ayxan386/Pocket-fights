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
    [SerializeField] private List<PossibleLoot> possibleDrops;

    [Header("AI parameters")] [SerializeField]
    private NavMeshAgent agent;

    public Guid Id { get; private set; }
    public List<PossibleLoot> PossibleLoots => possibleDrops;
    public bool IsDoneAttack { get; private set; }
    private bool isCombatModeActive = false;

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitUntil(() => gameObject.activeSelf && agent.enabled);
            yield return new WaitForSeconds(2);
            if (!isCombatModeActive)
                MoveTowardPlayer();
        }
    }

    private void MoveTowardPlayer()
    {
        if (agent.enabled)
            agent.SetDestination(PlayerInputController.Instance.transform.position);
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

    public void ActivateCombatMode(Transform mobStandPoint)
    {
        Id = Guid.NewGuid();
        agent.enabled = false;
        transform.position = mobStandPoint.position;
        transform.rotation = mobStandPoint.rotation;
        isCombatModeActive = true;
        EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
    }


    public void DeactivateCombatMode()
    {
        isCombatModeActive = false;
        agent.enabled = true;
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
                statController.GetStatValue(StatValue.BaseAttack).currentValue);
        }

        IsDoneAttack = true;
    }

    private void OnDeathCallback()
    {
        CombatModeGameManager.Instance.MobDefeated(this);
    }

    public bool IsAlive()
    {
        return gameObject.activeSelf && statController.GetStatValue(StatValue.Health).currentValue > 0;
    }
}