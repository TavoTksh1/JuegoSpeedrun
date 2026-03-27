using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI textoVida;
    [SerializeField] private TextMeshProUGUI textoBalasNormales;
    [SerializeField] private TextMeshProUGUI textoBalasEspeciales;
    [SerializeField] private TextMeshProUGUI textoDistancia;
    [SerializeField] private TextMeshProUGUI textoMonedas;

    [Header("Pantalla Game Over")]
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private TextMeshProUGUI textoDistanciaFinal;
    [SerializeField] private TextMeshProUGUI textoMonedasFinal;
    [SerializeField] private TextMeshProUGUI textoHighScore;

    private PlayerController playerController;
    private PlayerCombat playerCombat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Busca los componentes del jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerCombat = player.GetComponent<PlayerCombat>();
        }

        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying()) return;
        ActualizarHUD();
    }

    // ── HUD ─────────────────────────────────────────

    private void ActualizarHUD()
    {
        if (playerController != null && textoVida != null)
            textoVida.text = $"Vida: {playerController.GetVida()} / {playerController.GetVidaMaxima()}";

        if (playerCombat != null)
        {
            if (textoBalasNormales != null)
                textoBalasNormales.text = $"Balas: {playerCombat.GetBalasNormales()} / {playerCombat.GetMaxBalasNormales()}";

            if (textoBalasEspeciales != null)
                textoBalasEspeciales.text = $"Especiales: {playerCombat.GetBalasEspeciales()} / {playerCombat.GetMaxBalasEspeciales()}";
        }

        if (ScoreManager.Instance != null)
        {
            if (textoDistancia != null)
                textoDistancia.text = $"Distancia: {ScoreManager.Instance.Distance:F0}m";

            if (textoMonedas != null)
                textoMonedas.text = $"Monedas: {ScoreManager.Instance.Coins}";
        }
    }

    // ── Game Over ───────────────────────────────────

    public void MostrarGameOver()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        if (textoDistanciaFinal != null)
            textoDistanciaFinal.text = $"Distancia: {ScoreManager.Instance.Distance:F0}m";

        if (textoMonedasFinal != null)
            textoMonedasFinal.text = $"Monedas: {ScoreManager.Instance.Coins}";

        if (textoHighScore != null)
            textoHighScore.text = $"Mejor distancia: {ScoreManager.Instance.HighScore:F0}m";
    }

    // ── Botones Game Over ───────────────────────────

    public void OnReintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnVolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}