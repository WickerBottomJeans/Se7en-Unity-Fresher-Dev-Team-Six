using System.Collections;
using UnityEngine;

public class SingleEffectPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem effectPrefab;

    public void PlayEffect(Vector3 position)
    {
        ParticleSystem effectInstance = Instantiate(effectPrefab, position, effectPrefab.transform.rotation, transform);
        effectInstance.Play();
        StartCoroutine(CleanupEffect(effectInstance));
    }

    private IEnumerator CleanupEffect(ParticleSystem effectInstance)
    {
        while (effectInstance.IsAlive())
        {
            yield return null;
        }

        Destroy(effectInstance.gameObject);
    }
}
