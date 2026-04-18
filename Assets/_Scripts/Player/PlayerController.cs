using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 16f;

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

    private Animator animator;
    private Rigidbody2D rb;
    private bool estaEnSuelo;
    private bool estaVivo = true;

    private bool estaEnSueloBuffer;
    private float timerSuelo = 0f;
    private const float bufferSuelo = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Crea el material solo si no tiene uno asignado
        if (rb.sharedMaterial == null)
        {
            PhysicsMaterial2D mat = new PhysicsMaterial2D();
            mat.friction = 0f;
            mat.bounciness = 0f;
            rb.sharedMaterial = mat;
        }
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

        if (inputH > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (inputH < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (animator != null)
            animator.SetBool("isWalking", inputH != 0);
    }

    private void ManejarSalto()
    {
        if (estaEnSuelo)
            saltosRestantes = saltoMaximo;

        if (Input.GetButtonDown("Jump") && saltosRestantes > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            saltosRestantes--;

            if (AudioManager.instance != null)
                AudioManager.instance.PlayJump();
        }

        if (animator != null)
            animator.SetBool("isGrounded", estaEnSuelo);
    }

    private void VerificarSuelo()
    {
        // Usa un BoxCast en lugar de OverlapCircle para mejor detección con Tilemaps
        RaycastHit2D hit = Physics2D.BoxCast(
            puntoSuelo.position,
            new Vector2(0.4f, 0.1f),
            0f,
            Vector2.down,
            0.1f,
            capaSuelo
        );

        bool detecto = hit.collider != null;
        Debug.Log($"Detectó suelo: {detecto}");

        if (detecto)
        {
            estaEnSuelo = true;
            timerSuelo = bufferSuelo;
        }
        else
        {
            timerSuelo -= Time.fixedDeltaTime;
            if (timerSuelo <= 0f)
                estaEnSuelo = false;
        }

        if (animator != null)
            animator.SetBool("isGrounded", estaEnSuelo);
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

        if (animator != null)
            animator.SetTrigger("isHurt");

        if (vidaActual <= 0)
            Morir();
    }

    private void Morir()
    {
        estaVivo = false;

        if (animator != null)
            animator.SetBool("isDead", true);

        Debug.Log("Jugador murió");
        StartCoroutine(EsperarMuerte());
    }

    private System.Collections.IEnumerator EsperarMuerte()
    {
        yield return null;
        yield return null;

        float duracion = 1f;
        if (animator != null)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Player_Death"))
                duracion = info.length;
        }

        yield return new WaitForSeconds(duracion);
        GameManager.Instance.GameOver();
    }

    // ── Getters útiles ──────────────────────────────

    public int GetVida() => vidaActual;
    public int GetVidaMaxima() => vidaMaxima;
    public bool EstaVivo() => estaVivo;
}