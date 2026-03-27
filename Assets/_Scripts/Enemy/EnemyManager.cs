using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("Configuración")]
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private int maxEnemigos = 10;

    private List<GameObject> enemigosActivos = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnEnemigo(Vector2 posicion)
    {
        // Limpia enemigos destruidos de la lista
        enemigosActivos.RemoveAll(e => e == null);

        if (enemigosActivos.Count >= maxEnemigos) return;

        GameObject enemigo = Instantiate(prefabEnemigo, posicion, Quaternion.identity);
        enemigosActivos.Add(enemigo);
    }

    public void EliminarTodosLosEnemigos()
    {
        foreach (GameObject enemigo in enemigosActivos)
        {
            if (enemigo != null)
                Destroy(enemigo);
        }
        enemigosActivos.Clear();
    }
}