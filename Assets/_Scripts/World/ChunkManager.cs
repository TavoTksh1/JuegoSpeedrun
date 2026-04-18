using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    [Header("Tilemap")]
    [SerializeField] private Tilemap sueloTilemap;
    [SerializeField] private TileBase sueloTile;
    [SerializeField] private int profundidadRelleno = 5;

    [Header("Prefabs del chunk")]
    [SerializeField] private GameObject prefabPlataforma;
    [SerializeField] private GameObject prefabEspino;
    [SerializeField] private GameObject prefabMoneda;
    [SerializeField] private GameObject prefabAmmoStation;
    [SerializeField] private GameObject prefabObstaculo;
    [SerializeField] private GameObject prefabBalaNormalPickup;
    [SerializeField] private GameObject prefabBalaEspecialPickup;

    [Header("Configuración de chunks")]
    [SerializeField] private float anchoSeccion = 5f;
    [SerializeField] private float anchoHueco = 3f;
    [SerializeField] private int seccionesPorChunk = 4;
    [SerializeField] private int chunksVisibles = 3;
    [SerializeField] private int chunksGenerados = 0;

    [Header("Probabilidades")]
    [SerializeField] [Range(0, 1)] private float probEspino = 0.15f;
    [SerializeField] [Range(0, 1)] private float probEnemigo = 0.25f;
    [SerializeField] [Range(0, 1)] private float probAmmoStation = 0.15f;
    [SerializeField] [Range(0, 1)] private float probObstaculo = 0.15f;
    [SerializeField] [Range(0, 1)] private float probVacio = 0.1f;
    [SerializeField] [Range(0, 1)] private float probBalaNormal = 0.1f;
    [SerializeField] [Range(0, 1)] private float probBalaEspecial = 0.05f;

    private Transform jugador;
    private List<GameObject> chunksActivos = new List<GameObject>();
    private float xUltimoChunk = 0f;

    // Rastrea qué columnas X tienen suelo para poder borrarlas después
    private List<int> columnasConSuelo = new List<int>();

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
        GenerarChunkInicial();
        for (int i = 0; i < chunksVisibles; i++)
            GenerarChunk(false);
    }

    private void Update()
    {
        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) jugador = p.transform;
            return;
        }

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
        int xInicio = Mathf.RoundToInt(-anchoSeccion);
        int xFin = Mathf.RoundToInt(anchoTotal + anchoSeccion);

        PintarSuelo(xInicio, xFin);

        // Pared izquierda invisible
        GameObject pared = new GameObject("ParedInicial");
        pared.transform.parent = chunk.transform;
        pared.transform.position = new Vector2(-anchoSeccion, 2f);
        pared.layer = LayerMask.NameToLayer("Suelo");
        pared.tag = "Suelo";
        BoxCollider2D colPared = pared.AddComponent<BoxCollider2D>();
        colPared.size = new Vector2(1f, 10f);

        for (int i = 1; i <= 3; i++)
            Instantiate(prefabMoneda, new Vector2(anchoSeccion * i, 1f), Quaternion.identity, chunk.transform);

        xUltimoChunk += anchoTotal;
        chunksGenerados++;
    }

    // ── Chunk normal ────────────────────────────────

    private void GenerarChunk(bool forzarSeguro)
    {
        float xInicioFloat = xUltimoChunk;
        GameObject chunk = new GameObject($"Chunk_{chunksGenerados}");
        chunksActivos.Add(chunk);

        bool espinoAnterior = false;
        bool vacioAnterior = false;

        for (int i = 0; i < seccionesPorChunk; i++)
        {
            float x = xInicioFloat + (anchoSeccion * i) + anchoSeccion / 2f;
            float roll = Random.value;

            bool esVacio = !forzarSeguro && roll < probVacio && !vacioAnterior && i > 0;

            if (esVacio)
            {
                vacioAnterior = true;
                espinoAnterior = false;
                xInicioFloat += anchoHueco - anchoSeccion;
                continue;
            }

            vacioAnterior = false;

            // Pinta suelo con tilemap
            int xTileInicio = Mathf.RoundToInt(x - anchoSeccion / 2f);
            int xTileFin = Mathf.RoundToInt(x + anchoSeccion / 2f);
            PintarSuelo(xTileInicio, xTileFin);

            espinoAnterior = GenerarElemento(chunk, x, espinoAnterior);
        }

        xUltimoChunk = xInicioFloat + (seccionesPorChunk * anchoSeccion);
        chunksGenerados++;
    }

    // ── Tilemap ─────────────────────────────────────

    private void PintarSuelo(int xInicio, int xFin)
    {
        if (sueloTilemap == null || sueloTile == null) return;

        int yBase = -1;

        for (int tx = xInicio; tx < xFin; tx++)
        {
            // Fila principal
            sueloTilemap.SetTile(new Vector3Int(tx, yBase, 0), sueloTile);

            // Relleno hacia abajo
            for (int ty = yBase - 1; ty >= yBase - profundidadRelleno; ty--)
                sueloTilemap.SetTile(new Vector3Int(tx, ty, 0), sueloTile);

            columnasConSuelo.Add(tx);
        }
    }

    private void BorrarSuelo(int xInicio, int xFin)
    {
        if (sueloTilemap == null) return;

        int yBase = -1;

        for (int tx = xInicio; tx < xFin; tx++)
        {
            sueloTilemap.SetTile(new Vector3Int(tx, yBase, 0), null);

            for (int ty = yBase - 1; ty >= yBase - profundidadRelleno; ty--)
                sueloTilemap.SetTile(new Vector3Int(tx, ty, 0), null);
        }
    }

    // ── Elementos ───────────────────────────────────

    private bool GenerarElemento(GameObject chunk, float x, bool espinoAnterior)
    {
        float roll = Random.value;

        if (espinoAnterior)
        {
            if (prefabMoneda != null)
                Instantiate(prefabMoneda, new Vector2(x, 1f), Quaternion.identity, chunk.transform);
            TentarPlataforma(chunk, x, false);
            return false;
        }

        if (roll < probObstaculo)
        {
            GenerarObstaculo(chunk, x);
            TentarPlataforma(chunk, x, false);
            return false;
        }

        if (roll < probObstaculo + probEspino)
        {
            if (prefabEspino != null)
                Instantiate(prefabEspino, new Vector2(x, 0f), Quaternion.identity, chunk.transform);
            return true;
        }

        if (roll < probObstaculo + probEspino + probEnemigo)
        {
            if (EnemyManager.Instance != null)
                EnemyManager.Instance.SpawnEnemigo(new Vector2(x, 1f));
            TentarPlataforma(chunk, x, false);
            return false;
        }

        if (roll < probObstaculo + probEspino + probEnemigo + probAmmoStation)
        {
            if (prefabAmmoStation != null)
                Instantiate(prefabAmmoStation, new Vector2(x, 0.5f), Quaternion.identity, chunk.transform);
            TentarPlataforma(chunk, x, false);
            return false;
        }

        if (Random.value < probBalaNormal && prefabBalaNormalPickup != null)
        {
            Instantiate(prefabBalaNormalPickup, new Vector2(x, 1f), Quaternion.identity, chunk.transform);
            TentarPlataforma(chunk, x, false);
            return false;
        }

        if (Random.value < probBalaEspecial && prefabBalaEspecialPickup != null)
        {
            Instantiate(prefabBalaEspecialPickup, new Vector2(x, 1f), Quaternion.identity, chunk.transform);
            TentarPlataforma(chunk, x, false);
            return false;
        }

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

        if (chunksActivos.Count > chunksVisibles + 4)
        {
            GameObject chunkViejo = chunksActivos[0];

            // Borra los tiles del chunk viejo
            if (chunkViejo != null)
            {
                float xChunk = chunkViejo.transform.position.x;
                int xInicio = Mathf.RoundToInt(xChunk);
                int xFin = xInicio + Mathf.RoundToInt(seccionesPorChunk * anchoSeccion);
                BorrarSuelo(xInicio, xFin);
            }

            chunksActivos.RemoveAt(0);
            Destroy(chunkViejo);
        }
    }
}