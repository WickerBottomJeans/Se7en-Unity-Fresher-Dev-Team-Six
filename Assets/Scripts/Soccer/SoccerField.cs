using UnityEngine;

/// <summary>
/// [Duong] Provides information about the field's balls and goals
/// </summary>
public class SoccerField : MonoBehaviour
{
    [SerializeField] private SoccerBall[] soccerBalls;
    [SerializeField] private Transform[] goals;

    public static SoccerField Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Duplicate {nameof(SoccerField)} detected on {gameObject.name}.", gameObject);
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

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

        foreach (Transform goal in goals)
        {
            float squaredDistance = (goal.position - position).sqrMagnitude;
            if (squaredDistance < nearestSquaredDistance)
            {
                nearestSquaredDistance = squaredDistance;
                nearestGoal = goal;
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
}
