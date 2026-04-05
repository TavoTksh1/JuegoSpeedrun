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
    [SerializeField] private GameObject prefabObstaculo;

    [Header("Configuración de chunks")]
    [SerializeField] private float anchoSeccion = 5f;    // ancho de cada sección de suelo
    [SerializeField] private float anchoHueco = 3f;      // ancho exacto del hueco
    [SerializeField] private int seccionesPorChunk = 4;
    [SerializeField] private int chunksVisibles = 3;
    [SerializeField] private int chunksGenerados = 0;

    [Header("Probabilidades")]
    [SerializeField] [Range(0, 1)] private float probEspino = 0.15f;
    [SerializeField] [Range(0, 1)] private float probEnemigo = 0.25f;
    [SerializeField] [Range(0, 1)] private float probAmmoStation = 0.15f;
    [SerializeField] [Range(0, 1)] private float probObstaculo = 0.15f;
    [SerializeField] [Range(0, 1)] private float probVacio = 0.1f;

    [Header("Prefabs del chunk")]
    [SerializeField] private GameObject prefabBalaNormalPickup;
    [SerializeField] private GameObject prefabBalaEspecialPickup;
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

        // Chunk inicial seguro bajo el jugador
        GenerarChunkInicial();

        // Chunks normales
        for (int i = 0; i < chunksVisibles; i++)
            GenerarChunk(false);
    }

    private void Update()
    {
        if (jugador == null) return;

        float anchoTotal = seccionesPorChunk * anchoSeccion;
        if (jugador.position.x + (anchoTotal * (chunksVisibles - 1)) > xUltimoChunk)
            GenerarChunk(false);

        EliminarChunksViejos();
    }

    // ── Chunk inicial seguro ────────────────────────

    private void GenerarChunkInicial()
    {
        GameObject chunk = new GameObject($"Chunk_{chunksGenerados}");
        chunksActivos.Add(chunk);

        float anchoTotal = seccionesPorChunk * anchoSeccion;

        // Suelo inicial con tiles individuales
        for (float tx = -anchoSeccion; tx < anchoTotal + anchoSeccion; tx += 1f)
        {
            Instantiate(
                prefabSuelo,
                new Vector2(tx, -1f),
                Quaternion.identity,
                chunk.transform
            );
        }

        // Pared izquierda invisible — solo collider
        GameObject pared = new GameObject("ParedInicial");
        pared.transform.parent = chunk.transform;
        pared.transform.position = new Vector2(-anchoSeccion, 2f);
        pared.layer = LayerMask.NameToLayer("Suelo");
        pared.tag = "Suelo";
        BoxCollider2D colPared = pared.AddComponent<BoxCollider2D>();
        colPared.size = new Vector2(1f, 10f);

        // Monedas guía hacia la derecha
        for (int i = 1; i <= 3; i++)
            Instantiate(prefabMoneda, new Vector2(anchoSeccion * i, 1f), Quaternion.identity, chunk.transform);

        xUltimoChunk += anchoTotal;
        chunksGenerados++;
    }

    // ── Chunk normal ────────────────────────────────

    private void GenerarChunk(bool forzarSeguro)
    {
        float xInicio = xUltimoChunk;
        GameObject chunk = new GameObject($"Chunk_{chunksGenerados}");
        chunksActivos.Add(chunk);

        bool espinoAnterior = false;
        bool vacioAnterior = false;

        for (int i = 0; i < seccionesPorChunk; i++)
        {
            float x = xInicio + (anchoSeccion * i) + anchoSeccion / 2f;
            float roll = Random.value;

            // Hueco — nunca dos seguidos, nunca el primero del chunk
            bool esVacio = !forzarSeguro && roll < probVacio && !vacioAnterior && i > 0;

            if (esVacio)
            {
                // Genera hueco de ancho exacto
                GenerarHueco(chunk, x);
                vacioAnterior = true;
                espinoAnterior = false;
                // Avanza el xUltimoChunk por el hueco
                xInicio += anchoHueco - anchoSeccion;
                continue;
            }

            vacioAnterior = false;

            // Sección de suelo
            GenerarSeccionSuelo(chunk, x, anchoSeccion);

            // Elementos encima
            espinoAnterior = GenerarElemento(chunk, x, espinoAnterior);
        }

        xUltimoChunk = xInicio + (seccionesPorChunk * anchoSeccion);
        chunksGenerados++;
    }

    // ── Suelo y huecos ──────────────────────────────
    private void GenerarSeccionSuelo(GameObject chunk, float x, float ancho)
    {
        if (prefabSuelo == null) return;

        // Genera tiles individuales de tamaño 1x1
        for (float tx = x - ancho / 2f; tx < x + ancho / 2f; tx += 1f)
        {
            Instantiate(
                prefabSuelo,
                new Vector2(tx, -1f),
                Quaternion.identity,
                chunk.transform
            );
        }
    }

    private void GenerarHueco(GameObject chunk, float x)
    {
        // El hueco no genera suelo — solo marca visual opcional
        // El DeathZone detecta si el jugador cae aquí
        Debug.Log($"Hueco generado en X: {x}");
    }

    // ── Elementos ───────────────────────────────────

    private bool GenerarElemento(GameObject chunk, float x, bool espinoAnterior)
    {
        float roll = Random.value;

        // Zona segura después de espino
        if (espinoAnterior)
        {
            if (prefabMoneda != null)
                Instantiate(prefabMoneda, new Vector2(x, 1f), Quaternion.identity, chunk.transform);
            TentarPlataforma(chunk, x, false);
            return false;
        }

        // Obstáculo
        if (roll < probObstaculo)
        {
            GenerarObstaculo(chunk, x);
            TentarPlataforma(chunk, x, false);
            return false;
        }

        // Espino en suelo
        if (roll < probObstaculo + probEspino)
        {
            if (prefabEspino != null)
                Instantiate(prefabEspino, new Vector2(x, 0f), Quaternion.identity, chunk.transform);
            return true; // no genera plataforma arriba
        }

        // Enemigo
        if (roll < probObstaculo + probEspino + probEnemigo)
        {
            if (EnemyManager.Instance != null)
                EnemyManager.Instance.SpawnEnemigo(new Vector2(x, 1f));
            TentarPlataforma(chunk, x, false);
            return false;
        }

        // AmmoStation
        if (roll < probObstaculo + probEspino + probEnemigo + probAmmoStation)
        {
            if (prefabAmmoStation != null)
                Instantiate(prefabAmmoStation, new Vector2(x, 0.5f), Quaternion.identity, chunk.transform);
            TentarPlataforma(chunk, x, false);
            return false;
        }

        // Bala suelta aleatoria
        if (Random.value < 0.1f && prefabBalaNormalPickup != null)
            Instantiate(prefabBalaNormalPickup, new Vector2(x, 1f), Quaternion.identity, chunk.transform);
        else if (Random.value < 0.05f && prefabBalaEspecialPickup != null)
            Instantiate(prefabBalaEspecialPickup, new Vector2(x, 1f), Quaternion.identity, chunk.transform);

        // Moneda por defecto
        if (prefabMoneda != null)
            Instantiate(prefabMoneda, new Vector2(x, 1f), Quaternion.identity, chunk.transform);

        TentarPlataforma(chunk, x, true);
        return false;
    }



    private void GenerarObstaculo(GameObject chunk, float x)
    {
        if (prefabObstaculo == null) return;

        GameObject obs = Instantiate(
            prefabObstaculo,
            new Vector2(x, 0.25f),
            Quaternion.identity,
            chunk.transform
        );
        obs.transform.localScale = new Vector3(0.8f, 1.5f, 1f);

        // Puede salir enemigo encima
        if (Random.value > 0.5f && EnemyManager.Instance != null)
            EnemyManager.Instance.SpawnEnemigo(new Vector2(x, 1.8f));
    }

    private void TentarPlataforma(GameObject chunk, float x, bool puedeEspino)
    {
        if (Random.value > 0.5f && prefabPlataforma != null)
        {
            float yPlataforma = Random.Range(1.2f, 2.2f);
            GameObject plataforma = Instantiate(
                prefabPlataforma,
                new Vector2(x, yPlataforma),
                Quaternion.identity,
                chunk.transform
            );
            plataforma.transform.localScale = new Vector3(3f, 0.5f, 1f);
            plataforma.layer = LayerMask.NameToLayer("Suelo");
            plataforma.tag = "Suelo";

            // Espino encima solo si no hay espino abajo
            if (puedeEspino && Random.value < 0.4f && prefabEspino != null)
            {
                Instantiate(
                    prefabEspino,
                    new Vector2(x, yPlataforma + 0.5f),
                    Quaternion.identity,
                    chunk.transform
                );
            }
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