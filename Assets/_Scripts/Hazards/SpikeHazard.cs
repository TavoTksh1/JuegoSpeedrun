using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    [Header("Daño al jugador")]
    [SerializeField] private int danioAlJugador = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemigo = other.GetComponent<EnemyController>();
            if (enemigo != null)
                enemigo.Morir();
        }

        if (other.CompareTag("Player"))
        {
            PlayerController jugador = other.GetComponent<PlayerController>();
            if (jugador != null)
                jugador.RecibirDano(danioAlJugador);
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySpike();
        }
    }
}