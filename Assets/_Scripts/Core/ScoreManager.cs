using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // Puntuación por distancia (se actualiza cada frame)
    public float Distance { get; private set; }

    // Monedas recogidas en esta partida
    public int Coins { get; private set; }

    // Mejor distancia guardada
    public float HighScore { get; private set; }

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
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
        // Carga el high score guardado
        HighScore = PlayerPrefs.GetFloat(HighScoreKey, 0f);
        ResetScore();
    }

    // Llamado cada frame desde PlayerController con la posición X del jugador
    public void UpdateDistance(float playerX)
    {
        if (!GameManager.Instance.IsPlaying()) return;

        if (playerX > Distance)
        {
            Distance = playerX;

            // Actualiza high score si se supera
            if (Distance > HighScore)
            {
                HighScore = Distance;
                PlayerPrefs.SetFloat(HighScoreKey, HighScore);
            }
        }
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        Debug.Log($"Monedas: {Coins}");
    }

    public void ResetScore()
    {
        Distance = 0f;
        Coins = 0;
        Debug.Log("Puntuación reiniciada");
    }
}
