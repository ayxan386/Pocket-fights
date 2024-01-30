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

    [Header("Leveling")] [SerializeField] private float currentXp;
    [SerializeField] private AnimationCurve xpRequirements;
    [SerializeField] private float xpMultiplier;
    [SerializeField] private int maxLevel;


    private CharacterController cc;
    private Vector3 movementVector;
    private bool isPaused;
    public Vector3 lastPosition;
    public Quaternion lastRotation;

    public PlayerInput playerInput { get; private set; }

    public StatController Stats => statController;

    public static PlayerInputController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        EventManager.OnPlayerTurnEnd += OnPlayerTurnEnd;
    }

    private void OnPlayerTurnEnd(bool obj)
    {
        statController.RegenMana();
    }

    private void Start()
    {
        EventManager.OnSaveStarted += OnSaveStarted;
        EventManager.OnCombatSceneLoading += OnCombatSceneLoading;
        DataManager.Instance.LoadPlayerStats();

        statController.AttachedEntity = this;
    }

    private void OnCombatSceneLoading(bool isCombatScene)
    {
        if (isCombatScene)
            playerInput.SwitchCurrentActionMap("CombatMode");
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
            PlaceAtLastState();
        }
    }


    private void OnSaveStarted(int obj)
    {
        EventManager.OnStatSave?.Invoke(Stats);
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
        }
    }

    public void OnReceiveAttack()
    {
    }

    public void OnDeathCallback()
    {
    }

    private void OnMove(InputValue inputValue)
    {
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
        CombatModeGameManager.Instance.EndPlayerTurn();
    }

    private void OnCombatInitiate()
    {
        combatInitiation.StartInitiation(3);
    }

    private void OnPause()
    {
        inGameUiRef.SetActive(isPaused);
        isPaused = !isPaused;
        EventManager.OnPauseMenuToggled?.Invoke(isPaused);
    }

    private void OnChangeSelection(InputValue inp)
    {
        var changeDir = inp.Get<float>();
        EventManager.OnChangeSelection?.Invoke(changeDir);
    }


    private void OnActionUsed(int index)
    {
        if (CombatModeGameManager.Instance != null
            && !CombatModeGameManager.Instance.IsPlayerTurn) return;
        if (!CombatModeGameManager.Instance.IsCombatGoing) return;

        var actionDetails = PlayerActionManager.Instance.GetAction(index + 1);

        if (actionDetails == null || !actionDetails.canBeUsed) return;

        var usedAction = statController.UsedAction(actionDetails.manaConsumption);

        if (usedAction)
        {
            animator.SetTrigger(actionDetails.animationName);
            var selectedEnemy = CombatModeGameManager.Instance.SelectedEnemy;
            actionDetails.usageEffects?.Invoke(actionDetails, statController, selectedEnemy.Stats);
            EventManager.OnSkillUsedByPlayer?.Invoke(actionDetails, true);
        }
    }

    [ContextMenu("Save trigger")]
    public void SaveEventTrigger()
    {
        EventManager.OnSaveStarted?.Invoke(1);
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

    public void PlacePlayer(Transform playerStandPoint)
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        cc.enabled = false;
        transform.position = playerStandPoint.position;
        transform.rotation = playerStandPoint.rotation;
        cc.enabled = true;
    }
}