using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.StartAGame();
    }
}
