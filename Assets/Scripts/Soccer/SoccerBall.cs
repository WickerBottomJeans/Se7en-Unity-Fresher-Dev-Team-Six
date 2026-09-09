using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Controls the ball's flight and whether its states
/// </summary>
public class SoccerBall : MonoBehaviour, IPushable
{
    [SerializeField, Min(0.01f)] private float flightDuration = 1f;
    [SerializeField, Min(0f)] private float arcHeight = 2f;
    [SerializeField] private Transform visualRoot;
    [SerializeField, Min(0f)] private float rotationSpeed = 360f;
    [SerializeField] private Rigidbody ballRigidbody;

    private bool isKickable = true;
    private Tween flightTween;
    private Vector3 destinationPosition;

    #region Unity Lifecycle

    private void Awake()
    {
        if (flightDuration <= 0f || float.IsNaN(flightDuration) || float.IsInfinity(flightDuration))
        {
            throw new InvalidOperationException("Flight duration must be finite and greater than zero.");
        }

        if (arcHeight < 0f || float.IsNaN(arcHeight) || float.IsInfinity(arcHeight))
        {
            throw new InvalidOperationException("Arc height must be finite and nonnegative.");
        }

        if (visualRoot == null)
        {
            throw new InvalidOperationException("The ball visual root must be assigned to a child of the ball.");
        }

        if (rotationSpeed < 0f || float.IsNaN(rotationSpeed) || float.IsInfinity(rotationSpeed))
        {
            throw new InvalidOperationException("Rotation speed must be finite and nonnegative.");
        }
    }

    private void OnDisable()
    {
        flightTween?.Kill();
        flightTween = null;
    }

    #endregion

    #region Public API

    public bool IsKickable => isKickable && isActiveAndEnabled;

    /// <summary>
    /// [Duong] When it reach the goal after being LaunchTo(), NOT when pushed by physics
    /// </summary>
    public event Action<SoccerBall> DestinationReached;

    /// <summary>
    /// Pushes an available ball up to the requested speed along a horizontal direction.
    /// </summary>
    public void ApplyPush(Vector3 direction, float pushSpeed)
    {
        if (!IsKickable || ballRigidbody.isKinematic)
        {
            return;
        }

        direction.y = 0f;
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        direction.Normalize();

        // Include pushes queued before the next physics step.
        Vector3 pendingVelocityChange = ballRigidbody.GetAccumulatedForce() * (Time.fixedDeltaTime / ballRigidbody.mass);
        float speedAlongDirection = Vector3.Dot(ballRigidbody.velocity + pendingVelocityChange, direction);
        float speedToAdd = Mathf.Max(0f, pushSpeed - speedAlongDirection);
        if (speedToAdd > 0f)
        {
            ballRigidbody.AddForce(direction * speedToAdd, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// [Duong] Launches the ball along an arc and makes it unavailable for further kicks.
    /// </summary>
    public void LaunchTo(Vector3 destination)
    {
        if (!IsKickable)
        {
            throw new InvalidOperationException("The ball is not available for launch.");
        }

        Vector3 rotationAxis = Vector3.Cross(Vector3.up, destination - transform.position).normalized;
        Quaternion initialVisualRotation = visualRoot.rotation;
        destinationPosition = destination;
        isKickable = false;

        // Hand ball movement over to the flight tween.
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.isKinematic = true;
        ballRigidbody.detectCollisions = false;

        flightTween = DOTween.Sequence()
            .Append(transform.DOJump(destination, jumpPower: arcHeight, numJumps: 1, duration: flightDuration).SetEase(Ease.Linear))
            .Join(DOTween.To(() => 0f, angle => visualRoot.rotation = Quaternion.AngleAxis(angle, rotationAxis) * initialVisualRotation, rotationSpeed * flightDuration, flightDuration).SetEase(Ease.Linear))
            .OnComplete(HandleDestinationReached).OnKill(HandleFlightTweenKilled);
    }

    #endregion

    #region Private Methods

    private void HandleDestinationReached()
    {
        transform.position = destinationPosition;
        flightTween = null;
        DestinationReached?.Invoke(this);
    }

    private void HandleFlightTweenKilled()
    {
        flightTween = null;
    }

    #endregion
}
