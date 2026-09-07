using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    private Vector2 playerMovementInput;

    public bool IsMoving { get; private set; }

    public event Action<bool> MovementStateChanged;

    private void Update()
    {
        MovePlayer(playerMovementInput, Time.deltaTime);
    }

    public void HandlePlayerMoveInput(Vector2 movementInput)
    {
        playerMovementInput = movementInput;
        
        bool isMoving = playerMovementInput.sqrMagnitude > Mathf.Epsilon;
        if (IsMoving != isMoving)
        {
            IsMoving = isMoving;
            MovementStateChanged?.Invoke(IsMoving);
        }
    }

    /// <summary>
    /// [Duong] Moves the player and turns them toward the movement input
    /// </summary>
    /// <param name="playerMovementInput">Must be normalized</param>
    /// <param name="deltaTime"></param>
    public void MovePlayer(Vector2 playerMovementInput, float deltaTime)
    {
        Vector3 playerMovementInputDirection = new Vector3(playerMovementInput.x, 0f, playerMovementInput.y);

        //[Duong] Move the player GO
        characterController.SimpleMove(playerMovementInputDirection * movementSpeed);

        //[Duong] If player practically not moving
        if (playerMovementInputDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        //[Duong] Rotate player
        Quaternion targetPlayerRotation = Quaternion.LookRotation(playerMovementInputDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetPlayerRotation, rotationSpeed * deltaTime);
    }
}
