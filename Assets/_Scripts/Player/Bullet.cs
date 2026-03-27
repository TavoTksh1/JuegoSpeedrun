using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum TipoBala { Normal, Especial }

    [Header("Configuración")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float tiempoVida = 1.5f; // ← límite de tiempo
    [SerializeField] private float distanciaMaxima = 20f; // ← límite de distancia

    private TipoBala tipo;
    private float direccion = 1f;
    private Vector2 posicionInicial;

    private void Start()
    {
        posicionInicial = transform.position;
        Destroy(gameObject, tiempoVida);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * velocidad * direccion * Time.deltaTime);

        // Se destruye si supera la distancia máxima
        if (Vector2.Distance(posicionInicial, transform.position) >= distanciaMaxima)
            Destroy(gameObject);
    }

    public void Inicializar(TipoBala tipoBala, float dir)
    {
        tipo = tipoBala;
        direccion = dir;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Colisión física con: {other.gameObject.name} | Tag: {other.tag}");
        Destroy(gameObject);

        if (other.CompareTag("Enemy"))
        {
            EnemyController enemigo = other.GetComponent<EnemyController>();
            if (enemigo == null) return;

            if (tipo == TipoBala.Normal)
                enemigo.RecibirDano(1);
            else if (tipo == TipoBala.Especial)
                enemigo.SerManipulado();

            Destroy(gameObject);
        }

        if (other.CompareTag("Suelo")) Destroy(gameObject);
        if (other.CompareTag("Obstaculo")) Destroy(gameObject);
    }

   
}