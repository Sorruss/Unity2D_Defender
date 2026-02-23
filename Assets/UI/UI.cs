using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [Header("Dynamic")]
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private TextMeshProUGUI BestTimeText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI KillCountText;
    [SerializeField] private TextMeshProUGUI WaveCountText;
    [Space]
    [SerializeField] private GameObject WaveUI;
    [SerializeField] private CanvasGroup WaveUICanvasGroup;
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject ControlsUI;

    [Header("Flags")]

    private RectTransform controlsRT;
    private Vector3 originalControlsPos;
    private float controlsRToffsetX = 400.0f;

    private float timer = 0.0f;
    private float updateFrequency = 1.0f;

    private string GameOverLostString = "GAME OVER!";
    private string GameOverWonString = "YOU WON!";

    private float recordedBestTime = 0.0f;
    private float currentTime = 0.0f;
    private readonly string bestTimeString = "BestTime";

    public static UI instance;

    private bool isGamePaused = false;
    private bool isGameOver = false;
    public bool isUIVisible = false;
    [HideInInspector] public bool isControlsShown = false;

    private void Awake()
    {
        instance = this;
        controlsRT = ControlsUI.GetComponent<RectTransform>();
        originalControlsPos = controlsRT.position;
    }

    private void Start()
    {
        UpdateBestTime();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            HandleTimerTimer();
        }
    }

    private void HandleTimerTimer()
    {
        timer += Time.deltaTime;
        if (timer >= updateFrequency)
        {
            UpdateTimerText();
            timer = 0.0f;
        }
    }

    private void UpdateTimerText()
    {
        currentTime = Time.timeSinceLevelLoad;
        TimerText.text = ToStringTime(currentTime);
    }

    private string ToStringTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}.{seconds:00}";
    }

    public void ShowControls(bool show)
    {
        isControlsShown = show;
        if (show)
        {
            LeanTween.moveX(controlsRT, originalControlsPos.x + controlsRToffsetX, 0.4f);
        }
        else
        {
            LeanTween.moveX(controlsRT, originalControlsPos.x, 0.4f);
        }
    }

    private void SetIsUIVisible(bool visible)
    {
        isUIVisible = visible;
        CursorManager.EnableCursor(isUIVisible);
    }

    public void UpdateKillCountText(int killCount)
    {
        KillCountText.text = killCount.ToString();
    }

    public void EnableGameOverUI(bool enable = true, bool victory = false)
    {
        if (enable && victory)
        {
            UpdateBestTime();
        }

        isGameOver = enable;
        gameOverText.text = victory ? GameOverWonString : GameOverLostString;
        Time.timeScale = enable ? 0.1f : 1.0f;
        GameOverUI.SetActive(enable);
        SetIsUIVisible(enable);
    }

    private void UpdateBestTime()
    {
        if (PlayerPrefs.HasKey(bestTimeString))
        {
            recordedBestTime = PlayerPrefs.GetFloat(bestTimeString);
            if (currentTime != 0.0f && recordedBestTime > currentTime)
            {
                SetNewBestTime();
            }
            else
            {
                BestTimeText.text = ToStringTime(recordedBestTime);
            }
        }
        else
        {
            if (currentTime != 0.0f)
            {
                SetNewBestTime();
            }
            else
            {
                BestTimeText.text = "--:--";
            }
        }
    }

    private void SetNewBestTime()
    {
        PlayerPrefs.SetFloat(bestTimeString, currentTime);
        BestTimeText.text = ToStringTime(currentTime);
    }

    public void RestartLevel()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
        EnableGameOverUI(false);
    }

    public void GoToMainMenu()
    {
        if (isGamePaused)
        {
            PauseGame();
        }
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        if (isGameOver)
        {
            return;
        }

        Time.timeScale = isGamePaused ? 1.0f : 0.0f;
        isGamePaused = !isGamePaused;
        EnablePauseUI(isGamePaused);
    }

    private void EnablePauseUI(bool enable)
    {
        PauseUI.SetActive(enable);
        SetIsUIVisible(enable);
        WaveUICanvasGroup.alpha = enable ? 0.0f : 1.0f;
    }

    public void AnnounceWave(int waveNumber)
    {
        WaveCountText.text = waveNumber.ToString();
        WaveUI.SetActive(true);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}
