using System;
using UnityEngine;
using UnityEngine.InputSystem;

//TODO: think i might make this a prefab, not singleton no more 
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerKick playerKick;

    private PlayerInputActions playerInputActions;

    public bool CanKick => playerKick.CanKick;

    public event Action<bool> KickAvailabilityChanged
    {
        add => playerKick.NormalKickAvailabilityChanged += value;
        remove => playerKick.NormalKickAvailabilityChanged -= value;
    }

    public void RequestKick()
    {
        playerKick.Kick();
    }

    public void RequestAutoKick()
    {
        playerKick.AutoKick();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Duplicate {nameof(Player)} detected on {gameObject.name}.", gameObject);
            enabled = false;
            Destroy(gameObject);
            return;
        }

        Instance = this;
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        if (Instance != this)
        {
            return;
        }

        playerMovement.MovementStateChanged += HandleMovementStateChanged;
        playerAnimation.HandleMovementStateChanged(playerMovement.IsMoving);
        playerInputActions.Player.Move.performed += HandlePlayerMoveInput;
        playerInputActions.Player.Move.canceled += HandlePlayerMoveInput;
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        if (Instance != this)
        {
            return;
        }

        playerInputActions.Player.Disable();
        playerMovement.HandlePlayerMoveInput(Vector2.zero);
        playerInputActions.Player.Move.performed -= HandlePlayerMoveInput;
        playerInputActions.Player.Move.canceled -= HandlePlayerMoveInput;
        playerMovement.MovementStateChanged -= HandleMovementStateChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            playerInputActions.Dispose();
            Instance = null;
        }
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
