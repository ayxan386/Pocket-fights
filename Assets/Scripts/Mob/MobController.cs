using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MobController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<ActionDetails> actions;
    [SerializeField] private StatController statController;
    [SerializeField] private List<PossibleLoot> possibleDrops;
    [SerializeField] private MobDisplayData displayData;
    [SerializeField] private GameObject combatUiRef;
    [SerializeField] private GameObject selectionIndicators;

    [Header("AI parameters")] [SerializeField]
    private MobMovementController movementController;

    public Guid Id { get; private set; }
    public List<PossibleLoot> PossibleLoots => possibleDrops;
    public bool IsDoneAttack { get; private set; }
    public MobDisplayData DisplayData => displayData;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerCombatInitiation.Instance != null);
        if (!PlayerCombatInitiation.Instance.IsCombatScene)
        {
            DeactivateCombatMode();
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

    public void ActivateCombatMode(Transform mobStandPoint)
    {
        Id = Guid.NewGuid();
        animator.SetBool("move", false);
        movementController.Navigate = false;

        transform.position = mobStandPoint.position;
        transform.rotation = mobStandPoint.rotation;

        EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
    }


    public void DeactivateCombatMode()
    {
        combatUiRef.SetActive(false);
        movementController.Navigate = true;
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

    public void Selected(bool isSelected)
    {
        if (isSelected)
        {
            MobDescriptionManager.Instance.DisplayMob(this);
        }

        combatUiRef.SetActive(isSelected);
        selectionIndicators.SetActive(isSelected);
    }

    public bool IsAlive()
    {
        return gameObject.activeSelf && statController.GetStatValue(StatValue.Health).currentValue > 0;
    }
}

[Serializable]
public class MobDisplayData
{
    public string displayName;
    public Sprite icon;
}