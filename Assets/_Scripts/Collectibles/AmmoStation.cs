using UnityEngine;

public class AmmoStation : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float tiempoRecarga = 10f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color colorDisponible = Color.green;
    [SerializeField] private Color colorAgotado = Color.gray;

    private bool disponible = true;
    private float timerRecarga = 0f;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ActualizarColor();
    }

    private void Update()
    {
        if (!disponible)
        {
            timerRecarga -= Time.deltaTime;
            if (timerRecarga <= 0f)
            {
                disponible = true;
                ActualizarColor();
                Debug.Log("AmmoStation disponible de nuevo");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!disponible) return;

        if (other.CompareTag("Player"))
        {
            PlayerCombat combat = other.GetComponent<PlayerCombat>();
            if (combat != null)
            {
                combat.Recargar();
                disponible = false;
                timerRecarga = tiempoRecarga;
                ActualizarColor();
                Debug.Log("Munición recargada en AmmoStation");
            }
        }
    }

    private void ActualizarColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = disponible ? colorDisponible : colorAgotado;
    }
}