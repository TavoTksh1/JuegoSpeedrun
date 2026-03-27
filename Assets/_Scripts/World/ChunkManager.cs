using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    [Header("Prefabs del chunk")]
    [SerializeField] private GameObject prefabSuelo;
    [SerializeField] private GameObject prefabPlataforma;
    [SerializeField] private GameObject prefabEspino;
    [SerializeField] private GameObject prefabMoneda;
    [SerializeField] private GameObject prefabAmmoStation;

    [Header("Configuración de chunks")]
    [SerializeField] private float anchoChunk = 20f;
    [SerializeField] private int chunksVisibles = 3;
    [SerializeField] private int chunksGenerados = 0;

    [Header("Probabilidades")]
    [SerializeField] [Range(0, 1)] private float probEspino = 0.3f;
    [SerializeField] [Range(0, 1)] private float probEnemigo = 0.5f;
    [SerializeField] [Range(0, 1)] private float probAmmoStation = 0.15f;

    private Transform jugador;
    private List<GameObject> chunksActivos = new List<GameObject>();
    private float xUltimoChunk = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;

        // Genera los primeros chunks
        for (int i = 0; i < chunksVisibles; i++)
            GenerarChunk();
    }

    private void Update()
    {
        if (jugador == null) return;

        // Genera nuevo chunk cuando el jugador se acerca al final
        if (jugador.position.x + (anchoChunk * (chunksVisibles - 1)) > xUltimoChunk)
            GenerarChunk();

        // Elimina chunks muy lejanos
        EliminarChunksViejos();
    }

    // ── Generación ──────────────────────────────────

    private void GenerarChunk()
    {
        float xInicio = xUltimoChunk;
        GameObject chunk = new GameObject($"Chunk_{chunksGenerados}");
        chunksActivos.Add(chunk);

        // Suelo base del chunk
        GenerarSuelo(chunk, xInicio);

        // Contenido del chunk
        int elementosPorChunk = 4;
        float espaciado = anchoChunk / elementosPorChunk;

        for (int i = 1; i <= elementosPorChunk; i++)
        {
            float x = xInicio + (espaciado * i);
            GenerarElemento(chunk, x);
        }

        xUltimoChunk += anchoChunk;
        chunksGenerados++;
    }

    private void GenerarSuelo(GameObject chunk, float xInicio)
    {
        if (prefabSuelo == null) return;

        GameObject suelo = Instantiate(
            prefabSuelo,
            new Vector2(xInicio + anchoChunk / 2f, -1f),
            Quaternion.identity,
            chunk.transform
        );
        suelo.transform.localScale = new Vector3(anchoChunk, 1f, 1f);
    }

    private void GenerarElemento(GameObject chunk, float x)
    {
        float roll = Random.value;

        if (roll < probEspino)
        {
            // Espino en el suelo
            if (prefabEspino != null)
                Instantiate(prefabEspino, new Vector2(x, 0f), Quaternion.identity, chunk.transform);
        }
        else if (roll < probEspino + probEnemigo)
        {
            // Enemigo sobre el suelo
            if (EnemyManager.Instance != null)
                EnemyManager.Instance.SpawnEnemigo(new Vector2(x, 1f));
        }
        else if (roll < probEspino + probEnemigo + probAmmoStation)
        {
            // AmmoStation
            if (prefabAmmoStation != null)
                Instantiate(prefabAmmoStation, new Vector2(x, 0f), Quaternion.identity, chunk.transform);
        }
        else
        {
            // Moneda
            if (prefabMoneda != null)
                Instantiate(prefabMoneda, new Vector2(x, 1f), Quaternion.identity, chunk.transform);
        }

        // Plataforma aleatoria elevada
        if (Random.value > 0.6f && prefabPlataforma != null)
        {
            float yPlataforma = Random.Range(1.5f, 3f);
            GameObject plataforma = Instantiate(
                prefabPlataforma,
                new Vector2(x, yPlataforma),
                Quaternion.identity,
                chunk.transform
            );
            plataforma.transform.localScale = new Vector3(3f, 0.5f, 1f);
        }
    }

    // ── Limpieza ────────────────────────────────────

    private void EliminarChunksViejos()
    {
        chunksActivos.RemoveAll(c => c == null);

        if (chunksActivos.Count > chunksVisibles + 2)
        {
            GameObject chunkViejo = chunksActivos[0];
            chunksActivos.RemoveAt(0);
            Destroy(chunkViejo);
        }
    }
}