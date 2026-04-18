using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 120f;

    [Header("Detección de suelo")]
    [SerializeField] private Transform puntoSuelo;
    [SerializeField] private float radioSuelo = 0.2f;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Salto")]
    [SerializeField] private int saltoMaximo = 2;
    private int saltosRestantes;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 3;
    private int vidaActual;

    // Componentes
    private Rigidbody2D rb;
    private bool estaEnSuelo;
    private bool estaVivo = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0f, bounciness = 0f };
    }

    private void Start()
    {
        vidaActual = vidaMaxima;
    }

    private void Update()
    {
        if (!estaVivo) return;
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.IsPlaying()) return;

        ManejarMovimiento();
        ManejarSalto();
        ActualizarDistancia();
    }

    private void FixedUpdate()
    {
        VerificarSuelo();
    }

    // ── Movimiento ──────────────────────────────────

    private void ManejarMovimiento()
    {
        float inputH = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(inputH * velocidad, rb.velocity.y);

        // Voltear el sprite según dirección
        if (inputH > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (inputH < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void ManejarSalto()
    {
        if (estaEnSuelo)
            saltosRestantes = saltoMaximo;

        if (Input.GetButtonDown("Jump") && saltosRestantes > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            saltosRestantes--;
            AudioManager.instance.PlayJump();
        }
    }

    private void VerificarSuelo()
    {
        estaEnSuelo = Physics2D.OverlapCircle(
            puntoSuelo.position,
            radioSuelo,
            capaSuelo
        );
    }

    // ── Distancia ───────────────────────────────────

    private void ActualizarDistancia()
    {
        ScoreManager.Instance.UpdateDistance(transform.position.x);
    }

    // ── Vida y daño ─────────────────────────────────

    public void RecibirDano(int cantidad = 1)
    {
        if (!estaVivo) return;

        vidaActual -= cantidad;
        Debug.Log($"Jugador recibió daño. Vida: {vidaActual}/{vidaMaxima}");

        if (vidaActual <= 0)
            Morir();
    }

    private void Morir()
    {
        estaVivo = false;
        Debug.Log("Jugador murió");
        GameManager.Instance.GameOver();
    }

    // ── Getters útiles ──────────────────────────────

    public int GetVida() => vidaActual;
    public int GetVidaMaxima() => vidaMaxima;
    public bool EstaVivo() => estaVivo;
}