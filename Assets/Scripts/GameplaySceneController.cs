using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySceneController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SoccerField soccerField;

    private PlayerKickPresenter playerKickPresenter;

    #region Unity Lifecycle

    private void Awake()
    {
        if (player == null || soccerField == null)
        {
            Debug.LogError("Player and SoccerField references must be assigned.", this);
            return;
        }
        
        player.InitializePlayer(soccerField);
    }

    private void Start()
    {
        StartGameplay();
    }

    private void OnApplicationQuit()
    {
        EndGameplay();
    }

    private void OnDestroy()
    {
        EndGameplay();
    }

    #endregion

    #region Public API

    public void StartGameplay()
    {
        EndGameplay();
        UIManager.Instance.ShowGamePlayUI();
        playerKickPresenter = new PlayerKickPresenter(player);
        UIManager.Instance.ResetButtonClicked += HandleResetButtonClicked;
    }

    public void EndGameplay()
    {
        if (playerKickPresenter == null)
        {
            return;
        } 

        playerKickPresenter.Dispose();
        playerKickPresenter = null;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ResetButtonClicked -= HandleResetButtonClicked;
            UIManager.Instance.HideGamePlayUI();
        }
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

        Scene currentScene = gameObject.scene;
        EndGameplay();
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
