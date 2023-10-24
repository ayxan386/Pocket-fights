using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private float rotationLerpFactor;
    private CharacterController cc;

    private Vector3 movementVector;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
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
}