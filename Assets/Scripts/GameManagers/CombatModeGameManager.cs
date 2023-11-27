using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatModeGameManager : MonoBehaviour
{
    [SerializeField] private List<MobController> mobsInCombat;
    [SerializeField] private Button endTurnButton;

    [Header("Entity scene placement")] [SerializeField]
    private Transform playerStandPoint;

    [SerializeField] private List<Transform> mobStandPoints;

    [Header("Menus")] [SerializeField] private GameObject endingMenu;

    [SerializeField] private GameObject loadingMenu;

    [SerializeField] private TextMeshProUGUI endText;

    [Header("Loot stuff")] [SerializeField]
    private LootItemPanel lootItemPanelPrefab;

    [SerializeField] private Animator lootPanelAnimation;
    [SerializeField] private Transform lootHolder;

    public static CombatModeGameManager Instance { get; private set; }
    public MobController SelectedEnemy { get; private set; }

    public bool IsPlayerTurn { get; private set; } = true;

    public bool IsCombatGoing { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.OnCombatSceneLoading += OnCombatSceneLoading;
    }

    private void OnDisable()
    {
        EventManager.OnCombatSceneLoading -= OnCombatSceneLoading;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.5f);
        loadingMenu.SetActive(false);
    }

    private void OnCombatSceneLoading(bool isLoaded)
    {
        if (!isLoaded) return;

        PlayerInputController.Instance.PlacePlayer(playerStandPoint);
        if (PlayerCombatInitiation.Instance.mobs != null)
        {
            mobsInCombat = PlayerCombatInitiation.Instance.mobs;
        }

        for (var index = 0; index < mobsInCombat.Count; index++)
        {
            var mob = mobsInCombat[index];
            mob.ActivateCombatMode(mobStandPoints[index]);
        }

        SelectedEnemy = mobsInCombat[0];
        IsCombatGoing = isLoaded;
    }

    public void EndPlayerTurn()
    {
        if (!IsCombatGoing) return;
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
            if (!mobController.IsAlive()) continue;
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
        var currentHealth = PlayerInputController.Instance.Stats.GetStatValue(StatValue.Health).currentValue;

        if (currentHealth <= 0)
        {
            IsCombatGoing = false;
            Time.timeScale = 0;
            endingMenu.SetActive(true);
            EndOfCombat();
            endText.text = "You lost!!!";
        }
    }

    public void EndOfCombat()
    {
        mobsInCombat.ForEach(mob => { mob.DeactivateCombatMode(); });
        PlayerCombatInitiation.Instance.UnloadCombatScene();
    }

    public void MobDefeated(MobController deadMob)
    {
        foreach (var mobController in mobsInCombat)
        {
            if (mobController.Id == deadMob.Id)
            {
                StartCoroutine(MobDeath(deadMob));
                break;
            }
        }
    }

    private IEnumerator MobDeath(MobController deadMob)
    {
        yield return new WaitForSeconds(0.5f);
        deadMob.gameObject.SetActive(false);
        FindNextSelectedMobs();
        EventManager.OnMobDeath?.Invoke(deadMob);
        if (mobsInCombat.TrueForAll(mob => !mob.isActiveAndEnabled))
        {
            PlayerVictory();
        }
    }

    private void FindNextSelectedMobs()
    {
        foreach (var mob in mobsInCombat)
        {
            if (mob.IsAlive())
            {
                SelectedEnemy?.Selected(false);
                SelectedEnemy = mob;
                SelectedEnemy.Selected(true);
                break;
            }
        }
    }

    [ContextMenu("Player victory")]
    public void PlayerVictory()
    {
        IsCombatGoing = false;
        var allDrops = mobsInCombat.ConvertAll(mob => mob.PossibleLoots);
        var statController = PlayerInputController.Instance.Stats;
        statController.UpdateStatValue(StatValue.Mana, (int)statController.GetStatValue(StatValue.Mana).maxValue);
        var generatedLoot = LootManager.GenerateLoot(allDrops);
        foreach (var valueTuple in generatedLoot)
        {
            var newDrop = Instantiate(valueTuple.Item1);
            newDrop.count = valueTuple.Item2;
            var lootItemPanel = Instantiate(lootItemPanelPrefab, lootHolder);
            lootItemPanel.UpdateDisplay(newDrop);
        }

        lootPanelAnimation.SetTrigger("open");
    }
}