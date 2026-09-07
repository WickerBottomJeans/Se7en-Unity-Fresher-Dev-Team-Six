using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private const float IdleBlendValue = 0f;
    private const float RunningBlendValue = 0.6f;

    [SerializeField] private Animator animator;

    public void HandleMovementStateChanged(bool isMoving)
    {
        animator.SetFloat("Blend", isMoving ? RunningBlendValue : IdleBlendValue);
    }
}
