using System;
using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    [SerializeField, Min(0f)] private float kickRange = 2f;

    private SoccerField soccerField;

    #region Unity Lifecycle

    private void LateUpdate()
    {
        bool hasKickableBall = soccerField.TryGetNearestKickableBall(transform.position, kickRange, out _);
        UpdateNormalKickAvailability(hasKickableBall);
    }

    private void OnDisable()
    {
        UpdateNormalKickAvailability(false);
    }

    #endregion

    #region Public API

    public bool CanKick { get; private set; }

    public void InitializePlayerKick(SoccerField soccerField)
    {
        this.soccerField = soccerField;
    }

    /// <summary>
    /// [Duong] Raised when a normal kick becomes available or unavailable.
    /// </summary>
    public event Action<bool> NormalKickAvailabilityChanged;

    public void Kick()
    {
        if (!soccerField.TryGetNearestKickableBall(transform.position, kickRange, out SoccerBall ball))
        {
            UpdateNormalKickAvailability(false);
            return;
        }

        LaunchBallToNearestGoal(ball);
    }

    /// <summary>
    /// [Duong] Kicks the ball farthest from the player toward the goal nearest that ball.
    /// </summary>
    public void AutoKick()
    {
        if (!soccerField.TryGetFarthestKickableBall(transform.position, out SoccerBall ball))
        {
            Debug.Log("No more balls to AutoKick");
            return;
        }

        LaunchBallToNearestGoal(ball);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// [Duong] Launch ball to the nearst goal from the ball pos
    /// </summary>
    private void LaunchBallToNearestGoal(SoccerBall ball)
    {
        Transform nearestGoal = soccerField.GetNearestGoal(ball.transform.position);
        if (nearestGoal == null)
        {
            Debug.Log("Have no goal to kick to");
            return;
        }

        ball.LaunchTo(nearestGoal.position);
        UpdateNormalKickAvailability(soccerField.TryGetNearestKickableBall(transform.position, kickRange, out _));
    }

    private void UpdateNormalKickAvailability(bool canKick)
    {
        if (CanKick == canKick)
        {
            return;
        }

        CanKick = canKick;
        NormalKickAvailabilityChanged?.Invoke(CanKick);
    }

    #endregion
}
