using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Prefabs de balas")]
    [SerializeField] private GameObject prefabBalaNormal;
    [SerializeField] private GameObject prefabBalaEspecial;

    [Header("Punto de disparo")]
    [SerializeField] private Transform puntoDisparo;

    [Header("Munición")]
    [SerializeField] private int maxBalasNormales = 12;
    [SerializeField] private int maxBalasEspeciales = 6;

    private int balasNormales;
    private int balasEspeciales;

    // Dirección actual del jugador (1 = derecha, -1 = izquierda)
    private float direccion = 1f;

    private void Start()
    {
        balasNormales = maxBalasNormales;
        balasEspeciales = maxBalasEspeciales;
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.IsPlaying()) return;

        ActualizarDireccion();
        ManejarDisparo();
    }

    private void ActualizarDireccion()
    {
        // Sincroniza la dirección con el PlayerController
        direccion = transform.localScale.x > 0 ? 1f : -1f;
    }

    public void AgregarBalasNormales(int cantidad)
    {
        balasNormales = Mathf.Min(balasNormales + cantidad, maxBalasNormales);
        Debug.Log($"Balas normales: {balasNormales}");
    }

    public void AgregarBalasEspeciales(int cantidad)
    {
        balasEspeciales = Mathf.Min(balasEspeciales + cantidad, maxBalasEspeciales);
        Debug.Log($"Balas especiales: {balasEspeciales}");
    }
    private void ManejarDisparo()
    {
        // Bala normal — clic izquierdo
        if (Input.GetMouseButtonDown(0))
            Disparar(Bullet.TipoBala.Normal);

        // Bala especial — clic derecho
        if (Input.GetMouseButtonDown(1))
            Disparar(Bullet.TipoBala.Especial);
    }

    private void Disparar(Bullet.TipoBala tipo)
    {
        if (tipo == Bullet.TipoBala.Normal)
        {
            if (balasNormales <= 0)
            {
                Debug.Log("Sin balas normales");
                return;
            }
            balasNormales--;
            InstanciarBala(prefabBalaNormal, tipo);
        }
        else
        {
            if (balasEspeciales <= 0)
            {
                Debug.Log("Sin balas especiales");
                return;
            }
            balasEspeciales--;
            InstanciarBala(prefabBalaEspecial, tipo);
        }

        Debug.Log($"Balas normales: {balasNormales} | Especiales: {balasEspeciales}");
    }

    private void InstanciarBala(GameObject prefab, Bullet.TipoBala tipo)
    {
        GameObject bala = Instantiate(prefab, puntoDisparo.position, Quaternion.identity);
        Bullet bulletScript = bala.GetComponent<Bullet>();
        bulletScript.Inicializar(tipo, direccion);
    }

    // Llamado desde AmmoStation
    public void Recargar()
    {
        balasNormales = maxBalasNormales;
        balasEspeciales = maxBalasEspeciales;
        Debug.Log("Munición recargada");
    }

    // Getters para el UIManager
    public int GetBalasNormales() => balasNormales;
    public int GetBalasEspeciales() => balasEspeciales;
    public int GetMaxBalasNormales() => maxBalasNormales;
    public int GetMaxBalasEspeciales() => maxBalasEspeciales;
}