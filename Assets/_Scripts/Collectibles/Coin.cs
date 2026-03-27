using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int valor = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.Instance.AddCoins(valor);
            Destroy(gameObject);
        }
    }

    // Recoger con colisión normal también
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ScoreManager.Instance.AddCoins(valor);
            Destroy(gameObject);
        }
    }
}