using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerKickPresenter playerKickPresenter;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        if (Instance == this)
        {
            EndGame();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            EndGame();
            Instance = null;
        }
    }

    #endregion

    #region Public API

    public static GameManager Instance { get; private set; }

    public void StartAGame()
    {
        EndGame();
        UIManager.Instance.ShowGamePlayUI();
        playerKickPresenter = new PlayerKickPresenter();
        UIManager.Instance.ResetButtonClicked += HandleResetButtonClicked;
    }

    public void EndGame()
    {
        if (playerKickPresenter == null)
        {
            return;
        } 

        playerKickPresenter.Dispose();
        playerKickPresenter = null;
        UIManager.Instance.ResetButtonClicked -= HandleResetButtonClicked;
        UIManager.Instance.HideGamePlayUI();
    }
    
    /// <summary>
    /// [Duong] Ends the current game and reloads the scene
    /// </summary>
    public void ResetGame()
    {
        if (playerKickPresenter == null)
        {
            return;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        EndGame();
        SceneManager.LoadScene(currentScene.path);
    }

    #endregion

    #region Private Methods

    private void HandleResetButtonClicked()
    {
        ResetGame();
    }

    #endregion
}
