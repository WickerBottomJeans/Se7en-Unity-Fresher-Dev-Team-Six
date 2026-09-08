using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Controls the ball's flight and whether its states
/// </summary>
public class SoccerBall : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float flightDuration = 1f;
    [SerializeField, Min(0f)] private float arcHeight = 2f;
    [SerializeField] private Transform visualRoot;
    [SerializeField, Min(0f)] private float rotationSpeed = 360f;

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

    public event Action<SoccerBall> DestinationReached;

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
