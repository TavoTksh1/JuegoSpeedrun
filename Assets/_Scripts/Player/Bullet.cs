using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum TipoBala { Normal, Especial }

    [Header("Configuración")]
    [SerializeField] private float velocidad = 15f;
    [SerializeField] private float tiempoVida = 3f;
    private TipoBala tipo;
    private float direccion = 1f;

    private void Start()
    {
        // Se autodestruye si no golpea nada
        Destroy(gameObject, tiempoVida);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * velocidad * direccion * Time.deltaTime);
    }

    // Llamado desde PlayerCombat al instanciar la bala
    public void Inicializar(TipoBala tipoBala, float dir)
    {
        tipo = tipoBala;
        direccion = dir;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemigo = other.GetComponent<EnemyController>();
            if (enemigo == null) return;

            if (tipo == TipoBala.Normal)
            {
                enemigo.RecibirDano(1);
            }
            else if (tipo == TipoBala.Especial)
            {
                enemigo.SerManipulado();
            }

            Destroy(gameObject);
        }

        // Destruirse al tocar el suelo o paredes
        if (other.CompareTag("Suelo"))
        {
            Destroy(gameObject);
        }
    }
}