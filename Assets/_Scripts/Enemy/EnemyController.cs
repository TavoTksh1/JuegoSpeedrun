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
    [SerializeField] private Color colorNormal = Color.red;
    [SerializeField] private Color colorManipulado = Color.magenta;

    private EstadoEnemigo estado;
    private Vector2 posicionInicial;
    private float direccion = 1f;
    private Transform espinoCercano;

    private void Start()
    {
        // Busca el SpriteRenderer automáticamente si no está asignado
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        vidaActual = vidaMaxima;
        posicionInicial = transform.position;
        estado = EstadoEnemigo.Patrullando;

        if (spriteRenderer != null)
            spriteRenderer.color = colorNormal;
    }

    private void Update()
    {
        if (estado == EstadoEnemigo.Muerto) return;

        if (estado == EstadoEnemigo.Patrullando)
            Patrullar();
        else if (estado == EstadoEnemigo.Manipulado)
            MoverHaciaEspino();
    }

    // ── Patrulla ────────────────────────────────────

    private void Patrullar()
    {
        if (HayObstaculoEnfrente())
            direccion *= -1f;

        transform.Translate(Vector2.right * direccion * velocidadPatrulla * Time.deltaTime);

        float distancia = transform.position.x - posicionInicial.x;
        if (distancia >= distanciaPatrulla || distancia <= -distanciaPatrulla)
            direccion *= -1f;
    }

    // ── Manipulación ────────────────────────────────

    public void SerManipulado()
    {
        estado = EstadoEnemigo.Manipulado;

        if (spriteRenderer != null)
            spriteRenderer.color = colorManipulado;

        // Busca el espino más cercano en la escena
        espinoCercano = BuscarEspinoCercano();
        Debug.Log("Enemigo manipulado");
    }

    private bool HayEspinoEnfrente()
    {
        // Lanza un raycast corto en la dirección de movimiento
        Vector2 origen = transform.position;
        Vector2 dir = Vector2.right * direccion;
        float distanciaDeteccion = 0.8f;

        RaycastHit2D hit = Physics2D.Raycast(origen, dir, distanciaDeteccion);

        // Dibuja el ray en la vista Scene para depurar
        Debug.DrawRay(origen, dir * distanciaDeteccion, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Espino"))
            return true;

        return false;
    }

    private bool HayObstaculoEnfrente()
    {
        Vector2 origen = transform.position;
        Vector2 dir = Vector2.right * direccion;
        float distanciaDeteccion = 0.8f;

        // Detecta espinos Y obstáculos Y paredes
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
    private void MoverHaciaEspino()
    {
        if (espinoCercano == null)
        {
            // Si no hay espino, sigue moviéndose en una dirección
            transform.Translate(Vector2.right * velocidadManipulado * Time.deltaTime);
            return;
        }

        Vector2 dir = (espinoCercano.position - transform.position).normalized;
        transform.Translate(dir * velocidadManipulado * Time.deltaTime);
    }

    private Transform BuscarEspinoCercano()
    {
        GameObject[] espinos = GameObject.FindGameObjectsWithTag("Espino");
        Transform masCercano = null;
        float menorDistancia = Mathf.Infinity;

        foreach (GameObject espino in espinos)
        {
            float dist = Vector2.Distance(transform.position, espino.transform.position);
            if (dist < menorDistancia)
            {
                menorDistancia = dist;
                masCercano = espino.transform;
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
            Morir();
    }

    public void Morir()
    {
        if (estado == EstadoEnemigo.Muerto) return;

        estado = EstadoEnemigo.Muerto;
        SoltarMonedas();
        Debug.Log("Enemigo muerto");
        Destroy(gameObject, 0.1f);
    }

    // ── Monedas ─────────────────────────────────────

    private void SoltarMonedas()
    {
        if (prefabMoneda == null) return;

        for (int i = 0; i < monedasAlMorir; i++)
        {
            // Solo spawnea hacia arriba y a los lados, nunca hacia abajo
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