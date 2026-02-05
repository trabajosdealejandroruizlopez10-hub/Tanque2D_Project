using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Health UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Pause UI")]
    public GameObject pausePanel;

    [Header("Biome UI")]
    public TextMeshProUGUI biomeNameText;
    public float biomeFadeDuration = 2f;

    private CanvasGroup biomeTextCanvasGroup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Inicializar UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Setup para el texto de bioma con fade
        if (biomeNameText != null)
        {
            biomeTextCanvasGroup = biomeNameText.GetComponent<CanvasGroup>();
            if (biomeTextCanvasGroup == null)
            {
                biomeTextCanvasGroup = biomeNameText.gameObject.AddComponent<CanvasGroup>();
            }
            biomeTextCanvasGroup.alpha = 0f;
        }

        // Suscribirse al jugador para actualizar vida
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.OnHealthChanged.AddListener(UpdateHealth);
            // Inicializar con vida actual
            UpdateHealth(player.GetCurrentHealth(), player.GetMaxHealth());
        }

        UpdateScore(0);
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }

        if (healthText != null)
        {
            healthText.text = current + " / " + max;
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null && GameManager.Instance != null)
            {
                finalScoreText.text = "Final Score: " + GameManager.Instance.currentScore;
            }
        }
    }

    public void ShowPauseMenu(bool show)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(show);
        }
    }

    public void ShowBiomeTransition(string biomeName)
    {
        if (biomeNameText != null)
        {
            biomeNameText.text = biomeName;
            StartCoroutine(FadeBiomeText());
        }
    }

    private System.Collections.IEnumerator FadeBiomeText()
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < biomeFadeDuration / 2)
        {
            elapsed += Time.deltaTime;
            biomeTextCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / (biomeFadeDuration / 2));
            yield return null;
        }

        // Esperar un momento
        yield return new WaitForSeconds(1f);

        // Fade out
        elapsed = 0f;
        while (elapsed < biomeFadeDuration / 2)
        {
            elapsed += Time.deltaTime;
            biomeTextCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / (biomeFadeDuration / 2));
            yield return null;
        }

        biomeTextCanvasGroup.alpha = 0f;
    }

    // Métodos para botones
    public void OnRestartButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    public void OnResumeButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
    }

    public void OnQuitButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }
}
