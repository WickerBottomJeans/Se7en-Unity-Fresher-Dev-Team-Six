using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Goal : MonoBehaviour
{
    [SerializeField] private Transform aimTarget;

    public Transform AimTarget => aimTarget;

    /// <summary>
    /// Reports a physical ball entering the trigger; repeated entries may report the same ball.
    /// </summary>
    public event Action<SoccerBall> BallEnteredGoal;

    private void Awake()
    {
        if (!GetComponent<BoxCollider>().isTrigger)
        {
            throw new InvalidOperationException("The goal BoxCollider must have Is Trigger enabled.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled || other.isTrigger)
        {
            return;
        }

        Rigidbody ballRigidbody = other.attachedRigidbody;
        if (ballRigidbody == null || ballRigidbody.isKinematic)
        {
            return;
        }

        if (ballRigidbody.TryGetComponent(out SoccerBall soccerBall) && soccerBall.IsKickable)
        {
            BallEnteredGoal?.Invoke(soccerBall);
        }
    }
}
