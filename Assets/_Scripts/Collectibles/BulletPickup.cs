using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    public enum TipoBala { Normal, Especial }

    [SerializeField] private TipoBala tipo = TipoBala.Normal;
    [SerializeField] private int cantidad = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerCombat combat = other.GetComponent<PlayerCombat>();
        if (combat == null) return;

        if (tipo == TipoBala.Normal)
            combat.AgregarBalasNormales(cantidad);
        else
            combat.AgregarBalasEspeciales(cantidad);

        Debug.Log($"Recogida bala {tipo} x{cantidad}");
        if (AudioManager.instance != null)
            AudioManager.instance.PlayBulletPickup();
        Destroy(gameObject);
    }
}