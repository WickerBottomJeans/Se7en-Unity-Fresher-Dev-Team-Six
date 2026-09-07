using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    private void Start()
    {
        // TODO: just temporary, might add a proper GameManager to start the actual match later
        UIManager.Instance.ShowGamePlayUI();
    }
}
