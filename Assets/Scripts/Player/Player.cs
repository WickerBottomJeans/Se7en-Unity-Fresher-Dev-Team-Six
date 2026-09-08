using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerKick playerKick;

    private PlayerInputActions playerInputActions;
    private bool arePlayerControlsEnabled = true;

    public void InitializePlayer(SoccerField soccerField)
    {
        playerKick.InitializePlayerKick(soccerField);
    }

    public bool CanKick => playerKick.CanKick;

    public event Action<bool> KickAvailabilityChanged
    {
        add => playerKick.NormalKickAvailabilityChanged += value;
        remove => playerKick.NormalKickAvailabilityChanged -= value;
    }

    public event Action<SoccerBall> BallKicked
    {
        add => playerKick.BallKicked += value;
        remove => playerKick.BallKicked -= value;
    }

    public void RequestKick()
    {
        if (!arePlayerControlsEnabled)
        {
            return;
        }

        playerKick.Kick();
    }

    public void RequestAutoKick()
    {
        if (!arePlayerControlsEnabled)
        {
            return;
        }

        playerKick.AutoKick();
    }

    public void SetPlayerControlsEnabled(bool enabled)
    {
        arePlayerControlsEnabled = enabled;
        if (arePlayerControlsEnabled && isActiveAndEnabled)
        {
            playerInputActions.Player.Enable();
        }
        else
        {
            playerInputActions.Player.Disable();
            playerMovement.HandlePlayerMoveInput(Vector2.zero);
            playerAnimation.HandleMovementStateChanged(false);
        }
    }

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
        if (arePlayerControlsEnabled)
        {
            playerInputActions.Player.Enable();
        }
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
