using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum TipoBala { Normal, Especial }

    [Header("Configuración")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float tiempoVida = 1.5f;
    [SerializeField] private float distanciaMaxima = 20f;

    private TipoBala tipo;
    private float direccion = 1f;
    private Vector2 posicionInicial;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        posicionInicial = transform.position;
        Destroy(gameObject, tiempoVida);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * velocidad * direccion * Time.fixedDeltaTime);

        if (Vector2.Distance(posicionInicial, transform.position) >= distanciaMaxima)
            Destroy(gameObject);
    }

    public void Inicializar(TipoBala tipoBala, float dir)
    {
        tipo = tipoBala;
        direccion = dir;
        posicionInicial = transform.position;

        // Voltea el sprite según la dirección
        if (spriteRenderer != null)
            spriteRenderer.flipX = dir < 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemigo = other.GetComponent<EnemyController>();
            if (enemigo != null)
            {
                if (tipo == TipoBala.Normal)
                    enemigo.RecibirDano(1);
                else if (tipo == TipoBala.Especial)
                    enemigo.SerManipulado();
            }
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Suelo") || other.CompareTag("Obstaculo"))
            Destroy(gameObject);
    }
}