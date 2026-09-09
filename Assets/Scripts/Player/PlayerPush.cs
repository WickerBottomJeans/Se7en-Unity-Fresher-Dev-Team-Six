using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPush : MonoBehaviour
{
    [SerializeField, Min(0f)] private float pushSpeed = 9f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        if (pushDirection.sqrMagnitude <= Mathf.Epsilon || Vector3.Dot(pushDirection, hit.normal) >= 0f)
        {
            return;
        }

        Rigidbody contactedRigidbody = hit.collider.attachedRigidbody;
        if (contactedRigidbody != null && contactedRigidbody.TryGetComponent(out IPushable pushable))
        {
            pushable.ApplyPush(pushDirection.normalized, pushSpeed);
        }
    }
}
