using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatModeGameManager : MonoBehaviour
{
    [SerializeField] private List<MobController> mobsInCombat;
    [SerializeField] private Button endTurnButton;

    [Header("Entity scene placement")]
    [SerializeField] private Transform playerStandPoint;

    [SerializeField] private List<Transform> mobStandPoints;

    [Header("Menus")]
    [SerializeField] private GameObject loadingMenu;

    [Header("Loot stuff")]
    [SerializeField] private LootItemPanel lootItemPanelPrefab;

    [SerializeField] private Animator lootPanelAnimation;
    [SerializeField] private Transform lootHolder;

    [field: Header("Effects")]
    [field: SerializeField] public AudioSource SkillEffectsAudio { get; set; }

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
        EventManager.OnChangeSelection += OnChangeSelection;
    }


    private void OnDisable()
    {
        EventManager.OnCombatSceneLoading -= OnCombatSceneLoading;
        EventManager.OnChangeSelection -= OnChangeSelection;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.5f);
        loadingMenu.SetActive(false);
    }

    private void OnCombatSceneLoading(bool isLoaded)
    {
        if (!isLoaded) return;

        var statController = PlayerInputController.Instance.Stats;
        statController.UpdateStatValue(StatValue.Mana, (int)statController.GetStatValue(StatValue.Mana).maxValue);

        PlayerInputController.Instance.PlacePlayer(playerStandPoint, false);
        if (PlayerCombatInitiation.Instance.mobs != null)
        {
            mobsInCombat = PlayerCombatInitiation.Instance.mobs;
        }

        for (var index = 0; index < mobsInCombat.Count; index++)
        {
            var mob = mobsInCombat[index];
            mob.ActivateCombatMode(mobStandPoints[index]);
        }

        IsCombatGoing = isLoaded;
        FindNextSelectedMobs();
        EventManager.OnPlayerTurnStart?.Invoke(true);
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
        if (!IsCombatGoing) yield break;
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

        EventManager.OnPlayerTurnStart?.Invoke(true);
    }

    private void CheckPlayerHealth()
    {
        var player = PlayerInputController.Instance;
        var currentHealth = player.Stats.GetStatValue(StatValue.Health).currentValue;

        if (currentHealth <= 0)
        {
            IsCombatGoing = false;
            player.AddXp(player.CurrentXp * FloorManager.Instance.DeathPenalty);
            EndOfCombat();
            GlobalGameManager.Instance.EndDungeon();
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

    private void OnChangeSelection(float dirF)
    {
        var it = 0;
        var dir = Mathf.RoundToInt(dirF);
        var index = mobsInCombat.FindIndex(enm => enm == SelectedEnemy);
        var count = mobsInCombat.Count;
        while (it < count)
        {
            index = (index + dir + count) % count;
            if (mobsInCombat[index].IsAlive())
            {
                ChangeSelectedEnemy(mobsInCombat[index]);
                break;
            }

            it++;
        }
    }

    private void FindNextSelectedMobs()
    {
        foreach (var mob in mobsInCombat)
        {
            if (mob.IsAlive())
            {
                ChangeSelectedEnemy(mob);
                break;
            }
        }
    }

    private void ChangeSelectedEnemy(MobController mob)
    {
        SelectedEnemy?.Selected(false);
        SelectedEnemy = mob;
        SelectedEnemy.Selected(true);
    }

    [ContextMenu("Player victory")]
    public void PlayerVictory()
    {
        IsCombatGoing = false;

        EventManager.OnPlayerTurnEnd?.Invoke(true);
        EventManager.OnPlayerVictory?.Invoke(true);

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

        if (lootHolder.childCount > 0)
            EventSystem.current.SetSelectedGameObject(lootHolder.GetChild(0).gameObject);
        lootPanelAnimation.SetTrigger("open");
    }
}