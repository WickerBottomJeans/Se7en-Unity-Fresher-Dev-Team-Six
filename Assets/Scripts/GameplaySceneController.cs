using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySceneController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SoccerField soccerField;
    [SerializeField] private CameraController cameraController;

    private PlayerKickPresenter playerKickPresenter;
    private SoccerBall followedBall;

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
        cameraController.SetFollowTarget(player.transform);
        UIManager.Instance.ShowGamePlayUI();
        playerKickPresenter = new PlayerKickPresenter(player);
        player.BallKicked += HandleBallKicked;
        UIManager.Instance.ResetButtonClicked += HandleResetButtonClicked;
    }

    public void EndGameplay()
    {
        if (playerKickPresenter == null)
        {
            return;
        } 

        player.BallKicked -= HandleBallKicked;
        if (followedBall != null)
        {
            followedBall.DestinationReached -= HandleBallDestinationReached;
            followedBall = null;
        }

        cameraController.SetFollowTarget(null);
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

    private void HandleBallKicked(SoccerBall ball)
    {
        if (ball == null)
        {
            Debug.LogError("BallKicked event received a null SoccerBall.", this);
            return;
        }

        //[Duong] Follow the newest ball, which shouldnt happen cuz i unenalbe kicking 
        if (followedBall != null)
        {
            Debug.LogWarning("A new ball was kicked before the previous ball reached its destination.", this);
            followedBall.DestinationReached -= HandleBallDestinationReached;
        }

        followedBall = ball;
        followedBall.DestinationReached += HandleBallDestinationReached;
        cameraController.SetFollowTarget(followedBall.transform);
    }

    private void HandleBallDestinationReached(SoccerBall ball)
    {
        ball.DestinationReached -= HandleBallDestinationReached;
        followedBall = null;
        cameraController.SetFollowTarget(player.transform);
    }

    private void HandleResetButtonClicked()
    {
        ResetGame();
    }

    #endregion
}
