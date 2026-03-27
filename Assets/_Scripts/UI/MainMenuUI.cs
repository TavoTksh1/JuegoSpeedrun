using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelConfiguracion;

    [Header("High Score")]
    [SerializeField] private TextMeshProUGUI textoHighScore;

    private void Start()
    {
        MostrarPanelPrincipal();
        MostrarHighScore();
    }

    // ── Menú principal ──────────────────────────────

    private void MostrarHighScore()
    {
        if (textoHighScore == null)
        {
            Debug.LogError("textoHighScore es NULL");
            return;
        }
        float highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        Debug.Log($"High Score leído: {highScore}");
        textoHighScore.text = highScore > 0
            ? $"Mejor distancia: {highScore:F0}m"
            : "Sin record aun";
    }
    public void OnJugar()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnConfiguracion()
    {
        panelPrincipal.SetActive(false);
        panelConfiguracion.SetActive(true);
    }

    public void OnSalir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego");
    }

    // ── Configuración ───────────────────────────────

    public void OnVolverDesdeConfiguracion()
    {
        panelConfiguracion.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    // ── Helper ──────────────────────────────────────

    private void MostrarPanelPrincipal()
    {
        panelPrincipal.SetActive(true);
        panelConfiguracion.SetActive(false);
    }
}
