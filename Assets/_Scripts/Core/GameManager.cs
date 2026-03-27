using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Instancia global accesible desde cualquier script
    public static GameManager Instance { get; private set; }

    // Estados posibles del juego
    public enum GameState { Playing, GameOver, Paused }
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        // Singleton: solo puede existir uno
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        Debug.Log("Juego iniciado");
    }

    public void GameOver()
    {
        if (CurrentState == GameState.GameOver) return;
        CurrentState = GameState.GameOver;
        Debug.Log("Game Over");
        // Aquí después llamaremos al UIManager
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        Debug.Log("Juego pausado");
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        Debug.Log("Juego reanudado");
    }

    public bool IsPlaying()
    {
        return CurrentState == GameState.Playing;
    }
}