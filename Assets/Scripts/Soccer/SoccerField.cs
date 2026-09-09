using System;
using UnityEngine;

/// <summary>
/// [Duong] Provides information about the field's balls and goals
/// </summary>
public class SoccerField : MonoBehaviour
{
    [SerializeField] private SoccerBall[] soccerBalls;
    [SerializeField] private Goal[] goals;

    #region Unity Lifecycle

    private void Awake()
    {
        if (goals == null || goals.Length == 0)
        {
            throw new InvalidOperationException("At least one Goal must be assigned to the soccer field.");
        }

        foreach (Goal goal in goals)
        {
            if (goal == null)
            {
                throw new InvalidOperationException("Every soccer field Goals entry must reference a Goal.");
            }

            if (goal.AimTarget == null)
            {
                throw new InvalidOperationException("Every soccer field Goal must have an Aim Target assigned.");
            }
        }
    }

    private void OnEnable()
    {
        foreach (Goal goal in goals)
        {
            goal.BallEnteredGoal += HandleBallEnteredGoal;
        }
    }

    private void OnDisable()
    {
        if (goals == null)
        {
            return;
        }

        foreach (Goal goal in goals)
        {
            if (goal != null)
            {
                goal.BallEnteredGoal -= HandleBallEnteredGoal;
            }
        }
    }

    #endregion

    #region Public API

    public event Action<SoccerBall> BallEnteredGoal;

    /// <summary>
    /// [Duong] Returns the closest goal marker
    /// </summary>
    public Transform GetNearestGoal(Vector3 position)
    {
        if (goals == null || goals.Length == 0)
        {
            Debug.LogError("The goal collection must contain at least one goal.");
            return null;
        }

        Transform nearestGoal = null;
        float nearestSquaredDistance = float.PositiveInfinity;

        foreach (Goal goal in goals)
        {
            float squaredDistance = (goal.AimTarget.position - position).sqrMagnitude;
            if (squaredDistance < nearestSquaredDistance)
            {
                nearestSquaredDistance = squaredDistance;
                nearestGoal = goal.AimTarget;
            }
        }

        return nearestGoal;
    }

    /// <summary>
    /// [Duong] Returns the nearest kickable ball within range
    /// </summary>
    public bool TryGetNearestKickableBall(Vector3 position, float range, out SoccerBall ball)
    {
        float nearestSquaredDistance = range * range;
        ball = null;

        foreach (SoccerBall soccerBall in soccerBalls)
        {
            if (soccerBall == null || !soccerBall.IsKickable)
            {
                continue;
            }

            float squaredDistance = (soccerBall.transform.position - position).sqrMagnitude;
            if (squaredDistance <= nearestSquaredDistance)
            {
                nearestSquaredDistance = squaredDistance;
                ball = soccerBall;
            }
        }

        return ball != null;
    }

    /// <summary>
    /// [Duong] Returns the farthest kickable ball
    /// </summary>
    public bool TryGetFarthestKickableBall(Vector3 position, out SoccerBall ball)
    {
        float farthestSquaredDistance = 0f;
        ball = null;

        foreach (SoccerBall soccerBall in soccerBalls)
        {
            if (soccerBall == null || !soccerBall.IsKickable)
            {
                continue;
            }

            float squaredDistance = (soccerBall.transform.position - position).sqrMagnitude;
            if (ball == null || squaredDistance > farthestSquaredDistance)
            {
                farthestSquaredDistance = squaredDistance;
                ball = soccerBall;
            }
        }

        return ball != null;
    }

    #endregion

    #region Private Methods

    private void HandleBallEnteredGoal(SoccerBall ball)
    {
        BallEnteredGoal?.Invoke(ball);
    }

    #endregion
}
