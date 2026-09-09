using UnityEngine;

public interface IPushable
{
    /// <summary>
    /// Requests a push toward the given speed in units per second; the receiver may reject it.
    /// </summary>
    void ApplyPush(Vector3 direction, float pushSpeed);
}
