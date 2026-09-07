using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAnimation playerAnimation;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerMovement.MovementStateChanged += HandleMovementStateChanged;
        playerAnimation.HandleMovementStateChanged(playerMovement.IsMoving);
        playerInputActions.Player.Move.performed += HandlePlayerMoveInput;
        playerInputActions.Player.Move.canceled += HandlePlayerMoveInput;
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
        playerMovement.HandlePlayerMoveInput(Vector2.zero);
        playerInputActions.Player.Move.performed -= HandlePlayerMoveInput;
        playerInputActions.Player.Move.canceled -= HandlePlayerMoveInput;
        playerMovement.MovementStateChanged -= HandleMovementStateChanged;
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }

    private void HandlePlayerMoveInput(InputAction.CallbackContext context)
    {
        playerMovement.HandlePlayerMoveInput(context.ReadValue<Vector2>());
    }

    private void HandleMovementStateChanged(bool isMoving)
    {
        playerAnimation.HandleMovementStateChanged(isMoving);
    }
}
