using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySceneController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SoccerField soccerField;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private SingleEffectPlayer singleEffectPlayer;
    [SerializeField, Min(0f)] private float gameplayResumeDelay = 2f;

    private PlayerKickPresenter playerKickPresenter;
    private SoccerBall followedBall;
    private Coroutine gameplayResumeCoroutine;

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
        player.SetPlayerControlsEnabled(true);
    }

    public void EndGameplay()
    {
        if (gameplayResumeCoroutine != null)
        {
            StopCoroutine(gameplayResumeCoroutine);
            gameplayResumeCoroutine = null;
        }

        if (playerKickPresenter == null)
        {
            return;
        } 

        player.SetPlayerControlsEnabled(false);
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

    /// <summary>
    /// [Duong] Handles a kicked ball by following it and suspending player interaction
    /// </summary>
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
        player.SetPlayerControlsEnabled(false);
        cameraController.SetFollowTarget(followedBall.transform);
        UIManager.Instance.HideGamePlayUI();
    }

    /// <summary>
    /// [Duong] Handles the ball reaching its destination and begins the gameplay resume delay
    /// </summary>
    private void HandleBallDestinationReached(SoccerBall ball)
    {
        ball.DestinationReached -= HandleBallDestinationReached;
        followedBall = null;
        
        singleEffectPlayer.PlayEffect(ball.transform.position);
        cameraController.SetFollowTarget(null);
        Destroy(ball.gameObject);
        
        gameplayResumeCoroutine = StartCoroutine(ResumeGameplayAfterDelay());
    }

    /// <summary>
    /// [Duong] Resumes player interaction after the configured delay.
    /// </summary>
    private IEnumerator ResumeGameplayAfterDelay()
    {
        if (gameplayResumeDelay > 0f)
        {
            yield return new WaitForSeconds(gameplayResumeDelay);
        }

        gameplayResumeCoroutine = null;
        cameraController.SetFollowTarget(player.transform);
        UIManager.Instance.ShowGamePlayUI();
        player.SetPlayerControlsEnabled(true);
    }

    private void HandleResetButtonClicked()
    {
        ResetGame();
    }

    #endregion
}
