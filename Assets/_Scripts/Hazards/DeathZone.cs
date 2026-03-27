using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController jugador = other.GetComponent<PlayerController>();
            if (jugador != null)
                jugador.RecibirDano(999); // mata instantáneamente
        }

        // Destruye enemigos y monedas que caigan
        if (other.CompareTag("Enemy"))
            Destroy(other.gameObject);

        if (other.CompareTag("Coin"))
            Destroy(other.gameObject);
    }
}