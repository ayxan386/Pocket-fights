using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobController : MonoBehaviour, BaseEntityCallbacks
{
    [SerializeField] private Animator animator;
    [SerializeField] private StatController statController;
    [SerializeField] private List<PossibleLoot> possibleDrops;
    [SerializeField] private GameObject combatUiRef;
    [SerializeField] private GameObject selectionIndicators;
    [SerializeField] private MobActionManager actionManager;
    [field: SerializeField] public MobDisplayData DisplayData { get; private set; }
    [field: SerializeField] public List<string> Tags { get; private set; }

    [Header("AI parameters")] [SerializeField]
    private MobMovementController movementController;

    [SerializeField] private float statScalingFactor;
    private int randomAction;

    public Guid Id { get; private set; }
    public List<PossibleLoot> PossibleLoots => possibleDrops;

    public StatController Stats => statController;
    public bool IsDoneAttack;


    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerCombatInitiation.Instance != null);
        statController.AttachedEntity = this;
        statController.Animator = animator;
        if (!PlayerCombatInitiation.Instance.IsCombatScene)
        {
            DeactivateCombatMode();
        }
    }

    public void SetLevel(int level)
    {
        var totalSumOfStats = 0f;
        foreach (var statType in Enum.GetValues(typeof(StatTypes)).Cast<StatTypes>())
        {
            totalSumOfStats += statController.GetBaseStat(statType).baseValue;
        }

        statController.Level += level;
        var statsToAllocate = statController.Level * statScalingFactor;
        foreach (var statType in Enum.GetValues(typeof(StatTypes)).Cast<StatTypes>())
        {
            statController.UpgradeBaseStat(statType,
                (int)(statsToAllocate * statController.GetBaseStat(statType).baseValue / totalSumOfStats));
        }

        statController.UpdateStatValue(StatValue.Mana, (int)statController.GetStatValue(StatValue.Mana).maxValue);
        statController.UpdateStatValue(StatValue.Health, (int)statController.GetStatValue(StatValue.Health).maxValue);
    }

    private void OnDestroy()
    {
        DeactivateCombatMode();
    }

    public void ActivateCombatMode(Transform mobStandPoint)
    {
        Id = Guid.NewGuid();
        animator.SetBool("move", false);
        movementController.ToggleMovement(false);
        transform.SetPositionAndRotation(mobStandPoint.position, mobStandPoint.rotation);
        actionManager.ShowIntentPanel();

        EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
        EventManager.OnPlayerTurnStart += OnPlayerTurnStart;
    }

    public void DeactivateCombatMode()
    {
        combatUiRef.SetActive(false);
        movementController.ToggleMovement(true);
        EventManager.OnPlayerTurnEnd -= OnPlayerTurnEnd;
        EventManager.OnPlayerTurnStart -= OnPlayerTurnStart;
    }

    private void OnPlayerTurnEnd(bool obj)
    {
        statController.RegenMana();
    }

    private void OnPlayerTurnStart(bool obj)
    {
        actionManager.NextTurn();
    }

    public void AttackPlayer()
    {
        IsDoneAttack = false;
        actionManager.UseAttack(statController, res => IsDoneAttack = res);
    }

    public void OnReceiveAttack(float receivedDamage)
    {
        actionManager.AttackReceived(statController, receivedDamage);
    }

    public void OnDeathCallback()
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