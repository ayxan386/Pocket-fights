using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private float rotationLerpFactor;
    [SerializeField] private StatController statController;

    private CharacterController cc;
    private PlayerInput playerInput;
    private Vector3 movementVector;

    public StatController Stats => statController;

    public static PlayerInputController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        var isCombatScene = SceneManager.GetActiveScene().name.Contains("Combat");
        if (isCombatScene)
        {
            playerInput.SwitchCurrentActionMap("CombatMode");
        }
    }

    private void Update()
    {
        animator.SetBool("moving", movementVector.sqrMagnitude > 0);
        if (movementVector.sqrMagnitude > 0)
        {
            transform.Rotate(Vector3.up, movementVector.x * rotationLerpFactor * Time.deltaTime);
            cc.SimpleMove(transform.forward * (movementVector.z * movementSpeed));
        }
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

    private void OnActionUsed(int index)
    {
        var actionDetails = PlayerActionManager.Instance.GetAction(index);
        animator.SetTrigger(actionDetails.animationName);
        CombatModeGameManager.Instance.SelectedEnemy.ReceiveAttack(
            statController.GetStat(StatTypes.Strength).currentValue * actionDetails.attackMult);
    }
}