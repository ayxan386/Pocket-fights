using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Quaternion referenceRotation;
    [SerializeField] [Range(0, 1f)] private float rotationLerpFactor;
    private CharacterController cc;

    private Vector3 movementVector;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        animator.SetBool("moving", movementVector.sqrMagnitude > 0);
        transform.forward = Vector3.Lerp(transform.forward, movementVector, rotationLerpFactor * Time.deltaTime);
        cc.SimpleMove(movementVector);
    }

    private void OnMove(InputValue inputValue)
    {
        var inputVector = inputValue.Get<Vector2>();

        movementVector = referenceRotation * new Vector3(inputVector.x, 0, inputVector.y) * movementSpeed;
    }
}