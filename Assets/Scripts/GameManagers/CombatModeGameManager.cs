using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatModeGameManager : MonoBehaviour
{
    [SerializeField] private List<MobController> mobsInCombat;
    [SerializeField] private Button endTurnButton;

    [Header("Ending menu")] [SerializeField]
    private GameObject endingMenu;

    [SerializeField] private TextMeshProUGUI endText;
    public static CombatModeGameManager Instance { get; private set; }
    public MobController SelectedEnemy { get; private set; }
    public bool IsCombatMode { get; private set; }

    public bool IsPlayerTurn { get; private set; } = true;

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
        endTurnButton.interactable = false;
        IsPlayerTurn = false;
        EventManager.OnPlayerTurnEnd?.Invoke(true);
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(1.5f);
        foreach (var mobController in mobsInCombat)
        {
            mobController.AttackPlayer();
            yield return new WaitUntil(() => mobController.IsDoneAttack);
            yield return new WaitForSeconds(1.4f);
            CheckPlayerHealth();
        }

        IsPlayerTurn = true;
        endTurnButton.interactable = true;
    }

    private void CheckPlayerHealth()
    {
        var currentHealth = PlayerInputController.Instance.Stats.GetStat(StatValue.Health).currentValue;

        if (currentHealth <= 0)
        {
            Time.timeScale = 0;
            endingMenu.SetActive(true);
            EndOfCombat();
            endText.text = "You lost!!!";
        }
    }

    public void EndOfCombat()
    {
        mobsInCombat.ForEach(mob => mob.DeactivateCombatMode());
        Instance = null;
    }

    public void MobDefeated(MobController deadMob)
    {
        foreach (var mobController in mobsInCombat)
        {
            if (mobController.Id == deadMob.Id)
            {
                mobsInCombat.Remove(deadMob);
                StartCoroutine(MobDeath(deadMob));
                break;
            }
        }
    }

    private IEnumerator MobDeath(MobController deadMob)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(deadMob.gameObject);

        if (mobsInCombat.Count == 0)
        {
            Time.timeScale = 0;
            endingMenu.SetActive(true);
            endText.text = "You won!!!";
            EndOfCombat();
        }
    }
}