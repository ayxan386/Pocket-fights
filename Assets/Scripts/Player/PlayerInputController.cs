using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour, BaseEntityCallbacks
{
    [Header("Movement")] [SerializeField] private float movementSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 movementCorrectionAngle;

    [Header("Misc")] [SerializeField] private PlayerCombatInitiation combatInitiation;

    [SerializeField] private StatController statController;
    [SerializeField] private GameObject inGameUiRef;
    [SerializeField] private GameObject loadingScreen;

    [Header("Leveling")] [SerializeField] private float currentXp;
    [SerializeField] private AnimationCurve xpRequirements;
    [SerializeField] private float xpMultiplier;
    [SerializeField] private int maxLevel;

    [Header("Footsteps")] [SerializeField] private float spawnRate;
    [SerializeField] private Transform leftFoot, rightFoot;
    [SerializeField] private FadingSprite leftFootSprite, rightFootSprite;


    private CharacterController cc;
    private Vector3 movementVector;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private bool InUiMode => isPaused || State.isLookingAtQuests;
    public bool UsingAction { get; set; }

    public bool isPaused { get; private set; }
    public PlayerInput playerInput { get; private set; }

    public StatController Stats => statController;

    public static PlayerInputController Instance { get; private set; }

    public PlayerState State { get; private set; }

    private float lastSpawnTime;
    private bool isLeftFoot;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        Instance = this;
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        State = new PlayerState();
    }


    private IEnumerator Start()
    {
        EventManager.OnCombatSceneLoading += OnCombatSceneLoading;
        EventManager.OnPauseMenuToggled += OnPauseMenuToggled;
        EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
        statController.AttachedEntity = this;
        statController.Animator = animator;
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        DataManager.Instance.LoadPlayer();
        yield return new WaitForSeconds(0.1f);
        loadingScreen.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.OnCombatSceneLoading -= OnCombatSceneLoading;
        EventManager.OnPauseMenuToggled -= OnPauseMenuToggled;
        EventManager.OnPlayerTurnEnd -= OnPlayerTurnEnd;
    }

    private void OnCombatSceneLoading(bool isCombatScene)
    {
        if (isCombatScene)
        {
            playerInput.SwitchCurrentActionMap("CombatMode");
            cc.enabled = false;
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
            PlaceAtLastState();
        }
    }

    private void Update()
    {
        animator.SetBool("moving", movementVector.sqrMagnitude > 0);
        if (movementVector.sqrMagnitude > 0)
        {
            var temp = Quaternion.Euler(movementCorrectionAngle);
            var dir = temp * movementVector;
            transform.forward = dir;
            cc.SimpleMove(dir * movementSpeed);

            if (lastSpawnTime + spawnRate < Time.time)
            {
                lastSpawnTime = Time.time;
                SpawnFootstep();
            }
        }
    }

    private void SpawnFootstep()
    {
        if (isLeftFoot)
        {
            RaycastAndFootprint(leftFoot);
        }
        else
        {
            RaycastAndFootprint(rightFoot);
        }

        isLeftFoot = !isLeftFoot;
    }

    private void RaycastAndFootprint(Transform origin)
    {
        if (Physics.Raycast(origin.position, Vector3.down, out RaycastHit hit, 0.2f, groundLayer))
        {
            Instantiate(leftFootSprite, hit.point + hit.normal * 0.05f,
                Quaternion.LookRotation(origin.forward));
        }
    }

    private void OnPlayerTurnEnd(bool obj)
    {
        statController.RegenMana();
    }

    private void OnPauseMenuToggled(bool updatedState)
    {
        isPaused = updatedState;
        inGameUiRef.SetActive(!isPaused);
    }

    public void OnReceiveAttack(float receivedDamage)
    {
        var baseSkill = PlayerActionManager.Instance.ReceiveAttack;
        baseSkill.usageEffects?.Invoke(baseSkill, Stats, null);
    }

    public void OnDeathCallback()
    {
    }

    private void OnMove(InputValue inputValue)
    {
        if (InUiMode) return;
        var inputVector = inputValue.Get<Vector2>();
        movementVector = new Vector3(inputVector.x, 0, inputVector.y);
    }

    private void OnAction1()
    {
        OnActionUsed(0);
    }

    private void OnAction2()
    {
        OnActionUsed(1);
    }

    private void OnAction3()
    {
        OnActionUsed(2);
    }

    private void OnAction4()
    {
        OnActionUsed(3);
    }

    private void OnEndTurn()
    {
        if (InUiMode) return;
        CombatModeGameManager.Instance.EndPlayerTurn();
    }

    private void OnCombatInitiate()
    {
        if (InUiMode) return;
        combatInitiation.StartInitiation(2.5f);
    }

    private void OnPause()
    {
        movementVector = Vector3.zero;
        EventManager.OnPauseMenuToggled?.Invoke(!State.isLookingAtQuests && !isPaused);
    }

    private void OnChangeSelection(InputValue inp)
    {
        if (InUiMode) return;
        var changeDir = inp.Get<float>();
        EventManager.OnChangeSelection?.Invoke(changeDir);
    }

    private void OnActionUsed(int index)
    {
        if (InUiMode) return;
        if (CombatModeGameManager.Instance != null
            && !CombatModeGameManager.Instance.IsPlayerTurn) return;
        if (!CombatModeGameManager.Instance.IsCombatGoing) return;
        if (UsingAction) return;


        var actionDetails = PlayerActionManager.Instance.GetAction(index + 1);

        if (actionDetails == null || !actionDetails.canBeUsed) return;

        UsingAction = true;
        var usedAction = statController.UsedAction(actionDetails.manaConsumption);

        if (usedAction)
        {
            var selectedEnemy = CombatModeGameManager.Instance.SelectedEnemy;
            actionDetails.usageEffects?.Invoke(actionDetails, statController, selectedEnemy.Stats);
            EventManager.OnSkillUsedByPlayer?.Invoke(actionDetails, true);
        }
    }


    private float CalculateXpRequirements()
    {
        return xpRequirements.Evaluate(1f * Stats.Level / maxLevel) * xpMultiplier;
    }

    public void AddXp(float xpAmount)
    {
        currentXp += xpAmount;
        var xpRequired = CalculateXpRequirements();
        print("Xp required: " + xpRequired);
        while (currentXp >= xpRequired)
        {
            currentXp -= xpRequired;
            xpRequired = CalculateXpRequirements();
            Stats.UpdateLevel(1);
        }
    }

    public void PlaceAtLastState()
    {
        cc.enabled = false;
        transform.position = lastPosition;
        transform.rotation = lastRotation;
        cc.enabled = true;
    }

    public void PlacePlayer(Transform playerStandPoint, bool finalState = true)
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        cc.enabled = false;
        transform.position = playerStandPoint.position;
        transform.rotation = playerStandPoint.rotation;
        cc.enabled = finalState;
    }
}

[Serializable]
public class PlayerState
{
    public bool isLookingAtQuests;
}