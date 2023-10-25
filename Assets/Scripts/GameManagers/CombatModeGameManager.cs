using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatModeGameManager : MonoBehaviour
{
    [SerializeField] private List<MobController> mobsInCombat;
    [SerializeField] private Button endTurnButton;
    public static CombatModeGameManager Instance { get; private set; }
    public MobController SelectedEnemy { get; private set; }
    public bool IsCombatMode { get; private set; }

    public bool IsPlayerTurn { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IsCombatMode = SceneManager.GetActiveScene().name.Contains("Combat");
        mobsInCombat.ForEach(mob => mob.ActivateCombatMode());
        SelectedEnemy = mobsInCombat[0];
    }

    public void EndPlayerTurn()
    {
        IsPlayerTurn = false;
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator AttackPlayer()
    {
        foreach (var mobController in mobsInCombat)
        {
            mobController.AttackPlayer();
            yield return new WaitUntil(() => mobController.IsDoneAttack);
            yield return new WaitForSeconds(1.4f);
        }

        endTurnButton.interactable = true;
    }

    public void EndOfCombat()
    {
        mobsInCombat.ForEach(mob => mob.DeactivateCombatMode());
    }
}