using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool isEmpty = true;
    public float distancia;
    public int monedas;
    public string fecha;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const int TOTAL_SLOTS = 3;
    private const string SLOT_KEY = "SaveSlot_";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Guarda la partida en el slot indicado (0, 1 o 2)
    public void GuardarPartida(int slot)
    {
        SaveData data = new SaveData();
        data.isEmpty = false;
        data.distancia = ScoreManager.Instance.Distance;
        data.monedas = ScoreManager.Instance.Coins;
        data.fecha = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SLOT_KEY + slot, json);
        PlayerPrefs.Save();

        Debug.Log($"Partida guardada en slot {slot}: {json}");
    }

    // Carga la partida del slot indicado
    public SaveData CargarPartida(int slot)
    {
        string key = SLOT_KEY + slot;

        if (!PlayerPrefs.HasKey(key))
            return new SaveData(); // slot vacío

        string json = PlayerPrefs.GetString(key);
        return JsonUtility.FromJson<SaveData>(json);
    }

    // Borra el slot indicado
    public void BorrarSlot(int slot)
    {
        PlayerPrefs.DeleteKey(SLOT_KEY + slot);
        PlayerPrefs.Save();
        Debug.Log($"Slot {slot} borrado");
    }

    // Devuelve todos los slots para mostrarlos en el menú
    public SaveData[] ObtenerTodosLosSlots()
    {
        SaveData[] slots = new SaveData[TOTAL_SLOTS];
        for (int i = 0; i < TOTAL_SLOTS; i++)
            slots[i] = CargarPartida(i);
        return slots;
    }
}