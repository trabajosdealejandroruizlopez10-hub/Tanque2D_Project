using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool isPaused = false;
    public bool isGameOver = false;

    [Header("Score System")]
    public int currentScore = 0;
    public int enemiesKilled = 0;
    public int scorePerKill = 100;

    [Header("References")]
    public PlayerHealth playerHealth;
    public UIManager uiManager;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // No destruir al cambiar de escena (si quieres persistencia)
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;

        // Inicializar el UI según el estado del juego
        if (uiManager != null)
        {
            // Esta línea se asegura de que los paneles se configuren correctamente al inicio
            uiManager.ShowPauseMenu(isPaused);
        }

        // Suscribirse a eventos del jugador
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath.AddListener(HandlePlayerDeath);
        }
    }


    void Update()
    {
        // Pausar con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
        }
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        AddScore(scorePerKill);
        Debug.Log("Enemies killed: " + enemiesKilled);
    }

    void HandlePlayerDeath()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("Game Over!");
        
        // Mostrar pantalla de Game Over
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
        
        // Opcional: detener el tiempo
        // Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (uiManager != null)
            uiManager.ShowPauseMenu(isPaused);

        // Bloquear input del player mientras está en pausa
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = !isPaused;
    }


    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void OnDestroy()
    {
        // Desuscribirse de eventos para evitar errores
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath.RemoveListener(HandlePlayerDeath);
        }
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
