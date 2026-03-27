using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelSlots;
    [SerializeField] private GameObject panelConfiguracion;

    [Header("Slots de guardado")]
    [SerializeField] private Button[] botonesSlot;      // 3 botones
    [SerializeField] private TextMeshProUGUI[] textosSlot; // texto de cada slot
    [SerializeField] private bool modoCargar = false;   // true = cargar, false = nueva partida

    private void Start()
    {
        MostrarPanelPrincipal();
    }

    // ── Menú principal ──────────────────────────────

    public void OnNuevaPartida()
    {
        modoCargar = false;
        MostrarPanelSlots();
        ActualizarTextoSlots();
    }

    public void OnCargarPartida()
    {
        modoCargar = true;
        MostrarPanelSlots();
        ActualizarTextoSlots();
    }

    public void OnConfiguracion()
    {
        panelPrincipal.SetActive(false);
        panelConfiguracion.SetActive(true);
    }

    public void OnSalir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego");
    }

    // ── Slots ───────────────────────────────────────

    public void OnSeleccionarSlot(int slot)
    {
        if (modoCargar)
        {
            SaveData data = SaveManager.Instance.CargarPartida(slot);
            if (data.isEmpty)
            {
                Debug.Log("Slot vacío, no se puede cargar");
                return;
            }
            // Aquí después pasaremos los datos al ScoreManager
            Debug.Log($"Cargando slot {slot}");
        }
        else
        {
            SaveManager.Instance.BorrarSlot(slot);
            Debug.Log($"Nueva partida en slot {slot}");
        }

        SceneManager.LoadScene("Game");
    }

    public void OnVolverAlMenu()
    {
        MostrarPanelPrincipal();
    }

    // ── Configuración ───────────────────────────────

    public void OnCambiarMusica(float valor)
    {
        SettingsManager.Instance.SetMusicVolume(valor);
    }

    public void OnCambiarSFX(float valor)
    {
        SettingsManager.Instance.SetSFXVolume(valor);
    }

    public void OnCambiarPantallaCompleta(bool valor)
    {
        SettingsManager.Instance.SetFullscreen(valor);
    }

    public void OnVolverDesdeConfiguracion()
    {
        panelConfiguracion.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    // ── Helpers ─────────────────────────────────────

    private void MostrarPanelPrincipal()
    {
        panelPrincipal.SetActive(true);
        panelSlots.SetActive(false);
        panelConfiguracion.SetActive(false);
    }

    private void MostrarPanelSlots()
    {
        panelPrincipal.SetActive(false);
        panelSlots.SetActive(true);
        panelConfiguracion.SetActive(false);
    }

    private void ActualizarTextoSlots()
    {
        SaveData[] slots = SaveManager.Instance.ObtenerTodosLosSlots();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].isEmpty)
            {
                textosSlot[i].text = "Slot vacío";
            }
            else
            {
                textosSlot[i].text = $"Distancia: {slots[i].distancia:F0}m\n" +
                                     $"Monedas: {slots[i].monedas}\n" +
                                     $"{slots[i].fecha}";
            }
        }
    }
    // Wrappers para los botones de slot (Unity no permite pasar int directo)
    public void OnSeleccionarSlot0() { OnSeleccionarSlot(0); }
    public void OnSeleccionarSlot1() { OnSeleccionarSlot(1); }
    public void OnSeleccionarSlot2() { OnSeleccionarSlot(2); }
}
