using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EstadoEnemigo { Patrullando, Manipulado, Muerto }

    [Header("Movimiento")]
    [SerializeField] private float velocidadPatrulla = 2f;
    [SerializeField] private float velocidadManipulado = 4f;
    [SerializeField] private float distanciaPatrulla = 3f;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 3;
    private int vidaActual;

    [Header("Monedas")]
    [SerializeField] private GameObject prefabMoneda;
    [SerializeField] private int monedasAlMorir = 5;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorManipulado = Color.magenta;

    private EstadoEnemigo estado;
    private Vector2 posicionInicial;
    private float direccion = 1f;
    private Transform espinoCercano;
    private Animator animator;

    // ── Timers de patrulla ──────────────────────────
    private bool estaQuieto = false;
    private float timerQuieto = 0f;
    private float timerMovimiento = 0f;
    private float tiempoHastaQuieto = 0f;

    private float timerCambioDireccion = 0f;
    private float cooldownCambioDireccion = 0.5f;

    // ── Hurt ────────────────────────────────────────
    private bool estaEnHurt = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        vidaActual = vidaMaxima;
        posicionInicial = transform.position;
        estado = EstadoEnemigo.Patrullando;
        tiempoHastaQuieto = Random.Range(3f, 5f);

        if (spriteRenderer != null)
            spriteRenderer.color = colorNormal;

        // Fuerza el estado inicial correcto
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isDead", false);
            animator.SetBool("isManipulated", false);
        }
    }

    private void Update()
    {
        if (estado == EstadoEnemigo.Muerto) return;
        if (estaEnHurt) return;

        if (estado == EstadoEnemigo.Patrullando)
            Patrullar();
        else if (estado == EstadoEnemigo.Manipulado)
            MoverHaciaEspino();
    }

    // ── Patrulla ────────────────────────────────────

    private void Patrullar()
    {
        if (!estaQuieto)
        {
            timerMovimiento += Time.deltaTime;
            timerCambioDireccion += Time.deltaTime;

            if (timerMovimiento >= tiempoHastaQuieto)
            {
                estaQuieto = true;
                timerQuieto = Random.Range(1f, 2.5f);
                timerMovimiento = 0f;
                tiempoHastaQuieto = Random.Range(3f, 5f);

                if (animator != null)
                    animator.SetBool("isWalking", false);
                return;
            }

            if (timerCambioDireccion >= cooldownCambioDireccion && HayObstaculoEnfrente())
            {
                direccion *= -1f;
                timerCambioDireccion = 0f;
            }

            transform.Translate(Vector2.right * direccion * velocidadPatrulla * Time.deltaTime);

            float distancia = transform.position.x - posicionInicial.x;
            if (distancia >= distanciaPatrulla || distancia <= -distanciaPatrulla)
            {
                direccion *= -1f;
                timerCambioDireccion = 0f;
            }

            if (spriteRenderer != null)
                spriteRenderer.flipX = direccion < 0;

            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            timerQuieto -= Time.deltaTime;

            if (timerQuieto <= 0f)
            {
                estaQuieto = false;
                timerMovimiento = 0f;
            }
        }
    }

    // ── Manipulación ────────────────────────────────

    public void SerManipulado()
    {
        estado = EstadoEnemigo.Manipulado;

        if (spriteRenderer != null)
            spriteRenderer.color = colorManipulado;

        if (animator != null)
        {
            animator.SetTrigger("isManipulated");
            animator.SetBool("isWalking", false);
        }

        espinoCercano = BuscarEspinoCercano();
        Debug.Log("Enemigo manipulado");
    }

    private void MoverHaciaEspino()
    {
        // Espera a que termine la animación de Manipulado antes de moverse
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyMani"))
            return;

        // Una vez terminada activa Walk
        if (animator != null)
            animator.SetBool("isWalking", true);

        if (espinoCercano == null)
        {
            transform.Translate(Vector2.right * velocidadManipulado * Time.deltaTime);

            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
            return;
        }

        Vector2 dir = (espinoCercano.position - transform.position).normalized;
        transform.Translate(dir * velocidadManipulado * Time.deltaTime);

        if (spriteRenderer != null)
            spriteRenderer.flipX = dir.x < 0;
    }

    private bool HayObstaculoEnfrente()
    {
        Vector2 origen = transform.position;
        Vector2 dir = Vector2.right * direccion;
        float distanciaDeteccion = 0.8f;

        RaycastHit2D hit = Physics2D.Raycast(origen, dir, distanciaDeteccion);
        Debug.DrawRay(origen, dir * distanciaDeteccion, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Espino")) return true;
            if (hit.collider.CompareTag("Suelo")) return true;
            if (hit.collider.CompareTag("Obstaculo")) return true;
        }

        return false;
    }

    private Transform BuscarEspinoCercano()
    {
        GameObject[] espinos = GameObject.FindGameObjectsWithTag("Espino");
        Transform masCercano = null;
        float menorDistancia = Mathf.Infinity;
        float distanciaMaxima = 15f;
        float alturaEnemigo = transform.position.y;

        foreach (GameObject espino in espinos)
        {
            float dist = Vector2.Distance(transform.position, espino.transform.position);
            float diferenciaAltura = espino.transform.position.y - alturaEnemigo;

            if (dist < menorDistancia && dist < distanciaMaxima && diferenciaAltura <= 0.5f)
            {
                menorDistancia = dist;
                masCercano = espino.transform;
            }
        }

        if (masCercano == null)
        {
            foreach (GameObject espino in espinos)
            {
                float dist = Vector2.Distance(transform.position, espino.transform.position);
                if (dist < menorDistancia && dist < distanciaMaxima)
                {
                    menorDistancia = dist;
                    masCercano = espino.transform;
                }
            }
        }

        return masCercano;
    }

    // ── Daño y muerte ───────────────────────────────

    public void RecibirDano(int cantidad)
    {
        if (estado == EstadoEnemigo.Muerto) return;

        vidaActual -= cantidad;
        Debug.Log($"Enemigo recibió daño. Vida: {vidaActual}/{vidaMaxima}");

        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        StartCoroutine(PausarPorHurt());
    }

    private System.Collections.IEnumerator PausarPorHurt()
    {
        estaEnHurt = true;

        yield return null; // ← espera un frame antes de disparar
        
        if (animator != null)
            animator.SetTrigger("isHurt");

        yield return new WaitForSeconds(0.4f);

        estaEnHurt = false;

        if (estado == EstadoEnemigo.Manipulado && spriteRenderer != null)
            spriteRenderer.color = colorManipulado;
    }

    public void Morir()
    {
        if (estado == EstadoEnemigo.Muerto) return;

        bool eraManipulado = estado == EstadoEnemigo.Manipulado;
        estado = EstadoEnemigo.Muerto;

        if (spriteRenderer != null)
            spriteRenderer.color = eraManipulado ? colorManipulado : colorNormal;

        if (animator != null)
        {
            animator.SetBool("isDead", true);
            animator.SetBool("isWalking", false);
            animator.ResetTrigger("isManipulated");
        }

        SoltarMonedas();
        Debug.Log("Enemigo muerto");
        StartCoroutine(EsperarMuerte());
    }

    private System.Collections.IEnumerator EsperarMuerte()
    {
        // Espera a que el Animator entre al estado Death
        yield return null;
        yield return null;

        // Obtiene la duración real del clip de muerte
        float duracion = 1f; // valor por defecto
        if (animator != null)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("EnemyDeath"))
                duracion = info.length;
        }

        Debug.Log($"Duración animación muerte: {duracion}");
        yield return new WaitForSeconds(duracion);
        Destroy(gameObject);
    }

    // ── Monedas ─────────────────────────────────────

    private void SoltarMonedas()
    {
        if (prefabMoneda == null) return;

        for (int i = 0; i < monedasAlMorir; i++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(0.2f, 1.5f)
            );
            Instantiate(prefabMoneda, (Vector2)transform.position + offset, Quaternion.identity);
        }
    }

    // ── Colisiones ──────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Espino"))
            Morir();
    }
}