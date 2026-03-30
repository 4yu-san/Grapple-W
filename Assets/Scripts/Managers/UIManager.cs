using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Variable list
    public GameObject gameOverPanel;
    public GameObject pauseMenuScreen;
    public GameObject gameUIPanel;
    public GameObject levelCompleteScreen;
    public TMPro.TextMeshProUGUI levelCompleteTimer;    

    // Singleton instance
    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (_instance == null)
                _instance = UnityEngine.Object.FindAnyObjectByType<UIManager>();
            return _instance;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.instance.State == GameManager.GameState.Paused)
                Resume();
            else if (GameManager.instance.State == GameManager.GameState.Playing)
                Pause();
        }
    }

    // --- Pause ---
    public void Pause()
    {
        GameManager.instance.SetState(GameManager.GameState.Paused);
        pauseMenuScreen.SetActive(true);
        gameUIPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        GameManager.instance.SetState(GameManager.GameState.Playing);
        pauseMenuScreen.SetActive(false);
        gameUIPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- Level Complete ---
    public void HandleLevelCompleteUI()
    {
        GameManager.instance.SetState(GameManager.GameState.GameOver);
        levelCompleteScreen.SetActive(true);
        gameUIPanel.SetActive(false);
        levelCompleteTimer.text = "Time : " + FormatTime(GameManager.instance.finalTime);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;        
    }

    // --- Game Over ---
    public void HandleGameOverUI()
    {
        GameManager.instance.SetState(GameManager.GameState.GameOver);
        gameOverPanel.SetActive(true);
        gameUIPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private string FormatTime(float elapsedTime)
    {
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 100) % 100);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    // --- Navigation ---
    public void StartGame()
    {
        ResetCursor();
        SceneManager.LoadScene("SampleScene");
    }

    public void BacktoMenu()
    {
        GameManager.instance.SetState(GameManager.GameState.Playing);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void ReloadScene()
    {
        ResetCursor();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.ResetGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // --- Helpers ---
    private void ResetCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void NextLevel()
    {
        GameManager.instance.SetState(GameManager.GameState.Playing);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}