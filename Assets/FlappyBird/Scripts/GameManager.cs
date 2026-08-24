using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Start Screen")]
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject playButton;

    [Header("Score")]
    [SerializeField] private TMP_Text score;

    [Header("Game Ready Section")]
    [SerializeField] private GameObject gameReadyPanel;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverScore;
    [SerializeField] private TMP_Text gameOverBestScore;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PipeSpawner pipeSpawner;

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeAmount = 0.5f;

    private const string BEST_SCORE_KEY = "BestScore";
    private GameState gameState = GameState.Home;
    public GameState GameState => gameState;

    private Camera mainCamera;

    private int currentScore;
    public int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = value;
            if (score != null) score.text = currentScore.ToString();
        }
    }

    public int BestScore
    {
        get => PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        set
        {
            PlayerPrefs.SetInt(BEST_SCORE_KEY, value);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        gameState = GameState.Home;
        if (logo != null) logo.SetActive(true);
        if (playButton != null) playButton.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameReadyPanel != null) gameReadyPanel.SetActive(false);
        if (score != null) score.gameObject.SetActive(false);
    }

    public void PlayButtonClick()
    {
        gameState = GameState.GetReady;
        CurrentScore = 0;

        if (logo != null) logo.SetActive(false);
        if (playButton != null) playButton.SetActive(false);

        if (gameReadyPanel != null) gameReadyPanel.SetActive(true);
        if (score != null) score.gameObject.SetActive(true);

        ResetGame();
    }

    private void ResetGame()
    {
        if (playerController != null) playerController.ResetPlayer();
        if (pipeSpawner != null) pipeSpawner.ResetSpawner();
    }

    public void GamePlay()
    {
        gameState = GameState.Playing;
        if (gameReadyPanel != null) gameReadyPanel.SetActive(false);
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;

        if (mainCamera != null)
        {
            StartCoroutine(ShakeCamera());
        }

        if (score != null) score.gameObject.SetActive(false);

        // Update high score if current score is greater
        if (CurrentScore > BestScore)
        {
            BestScore = CurrentScore;
        }

        // Display both scores on Game Over screen
        if (gameOverScore != null)
            gameOverScore.text = CurrentScore.ToString();

        if (gameOverBestScore != null)
            gameOverBestScore.text = BestScore.ToString();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (playButton != null) playButton.SetActive(true);
    }

    private IEnumerator ShakeCamera()
    {
        Vector3 originalPos = mainCamera.transform.position;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);

            mainCamera.transform.position = originalPos + new Vector3(x, y, 0);
            yield return null;
        }

        mainCamera.transform.position = originalPos;
    }

    public void AddScore()
    {
        if (gameState != GameState.Playing)
            return;

        CurrentScore++;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Score();
        }
    }
}
