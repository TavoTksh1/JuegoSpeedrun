using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;

    [Header("Toggle")]
    [SerializeField] private Toggle toggleFullscreen;

    [Header("Textos de valor")]
    [SerializeField] private TextMeshProUGUI textoMusica;
    [SerializeField] private TextMeshProUGUI textoSFX;

    private void Start()
    {
        // Cada vez que se abre el panel carga los valores guardados
        CargarValores();
    }

    private void CargarValores()
    {
        if (SettingsManager.Instance == null)
        {
            Debug.LogError("SettingsManager.Instance es NULL");
            return;
        }

        Debug.Log($"Cargando valores - Musica: {SettingsManager.Instance.MusicVolume} | SFX: {SettingsManager.Instance.SFXVolume}");

        if (sliderMusica != null)
        {
            sliderMusica.onValueChanged.RemoveListener(OnCambiarMusica);
            sliderMusica.value = SettingsManager.Instance.MusicVolume;
            sliderMusica.onValueChanged.AddListener(OnCambiarMusica);
            ActualizarTextoMusica(sliderMusica.value);
            Debug.Log($"Slider musica asignado a: {sliderMusica.value}");
        }
        else
        {
            Debug.LogError("sliderMusica es NULL");
        }

        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.RemoveListener(OnCambiarSFX);
            sliderSFX.value = SettingsManager.Instance.SFXVolume;
            sliderSFX.onValueChanged.AddListener(OnCambiarSFX);
            ActualizarTextoSFX(sliderSFX.value);
        }

        if (toggleFullscreen != null)
            toggleFullscreen.isOn = SettingsManager.Instance.IsFullscreen;
    }

    // ── Callbacks de UI ─────────────────────────────

    public void OnCambiarMusica(float valor)
    {
        SettingsManager.Instance.SetMusicVolume(valor);
        ActualizarTextoMusica(valor);
    }

    public void OnCambiarSFX(float valor)
    {
        SettingsManager.Instance.SetSFXVolume(valor);
        ActualizarTextoSFX(valor);
    }

    public void OnCambiarFullscreen(bool valor)
    {
        SettingsManager.Instance.SetFullscreen(valor);
    }

    // ── Textos de valor ─────────────────────────────

    private void ActualizarTextoMusica(float valor)
    {
        if (textoMusica != null)
            textoMusica.text = $"{Mathf.RoundToInt(valor * 100)}%";
    }

    private void ActualizarTextoSFX(float valor)
    {
        if (textoSFX != null)
            textoSFX.text = $"{Mathf.RoundToInt(valor * 100)}%";
    }
}
