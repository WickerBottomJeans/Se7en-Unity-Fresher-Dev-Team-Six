using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    /// <summary>
    /// [Duong] Sets Cinemachine's follow target, or clears it when null.
    /// </summary>
    public void SetFollowTarget(Transform followTarget)
    {
        virtualCamera.Follow = followTarget;
    }
}
