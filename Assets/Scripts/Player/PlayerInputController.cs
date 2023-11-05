using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraRotation;
    [Header("Misc")]
    [SerializeField] private PlayerCombatInitiation combatInitiation;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private StatController statController;
    
    [Header("Leveling")]
    [SerializeField] private float currentXp;
    [SerializeField] private AnimationCurve xpRequirements;
    [SerializeField] private float xpMultiplier;
    [SerializeField] private int maxLevel;


    private CharacterController cc;
    private PlayerInput playerInput;
    private Vector3 movementVector;

    public StatController Stats => statController;

    public static PlayerInputController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        print("Assigned");
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
        var isCombatScene = SceneManager.GetActiveScene().name.Contains("Combat");
        DataManager.Instance.LoadPlayerStats();
        if (isCombatScene)
        {
            playerInput.SwitchCurrentActionMap("CombatMode");
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
            var tempEuler = cameraRotation.rotation.eulerAngles;
            tempEuler.x = 0;
            var temp = Quaternion.Euler(tempEuler);
            var dir = temp * movementVector;
            transform.forward = dir;
            cc.SimpleMove(dir * movementSpeed);
        }
    }

    public void ReceiveAttack(float baseDamage)
    {
        Stats.ReceiveAttack(baseDamage, OnDeathCallback);
    }

    private void OnDeathCallback()
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
        if (CombatModeGameManager.Instance.IsCombatMode)
        {
            CombatModeGameManager.Instance.EndPlayerTurn();
        }
    }

    private void OnCombatInitiate()
    {
        combatInitiation.StartInitiation(3);
    }

    private void OnPause()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }


    private void OnActionUsed(int index)
    {
        if (CombatModeGameManager.Instance != null
            && !CombatModeGameManager.Instance.IsPlayerTurn) return;
        var actionDetails = PlayerActionManager.Instance.GetAction(index);
        var usedAction = statController.UsedAction(actionDetails.manaConsumption);
        if (usedAction)
        {
            animator.SetTrigger(actionDetails.animationName);
            CombatModeGameManager.Instance.SelectedEnemy.ReceiveAttack(
                statController.GetStatValue(StatValue.BaseAttack).currentValue * actionDetails.attackMult);
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
        print("Xp required: " + xpRequired );
        while (currentXp >= xpRequired)
        {
            currentXp -= xpRequired;
            xpRequired = CalculateXpRequirements();
            Stats.UpdateLevel(1);
        }
    }
}