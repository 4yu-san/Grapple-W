using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    
    public bool gameOver = false;
    public bool hasGameStarted = false;
    public GameState State { get; private set; } = GameState.Playing;

    public static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = UnityEngine.Object.FindAnyObjectByType<GameManager>();
            }

            return _instance;
        }
    }

    public enum GameState
    {
        Playing,
        GameOver,
        Paused
    }

    public void SetState(GameState newState)
{
    State = newState;

    switch (newState)
    {
        case GameState.Playing:
            Time.timeScale = 1f;
            hasGameStarted = true;
            gameOver = false;
            break;
        case GameState.GameOver:
            Time.timeScale = 0f;
            gameOver = true;
            break;
        case GameState.Paused:
            Time.timeScale = 0f;
            break;
    }
}

    //Function LIst
    public void StartGame()
    {
        hasGameStarted = true;
    }
    public void GameOver()
    {
        gameOver = true;
        State = GameState.GameOver;
        Debug.Log("Game Over Triggered");
        UIManager.instance.HandleGameOverUI();
        Time.timeScale = 0f; // Freeze game time
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
