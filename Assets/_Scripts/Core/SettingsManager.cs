using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";

    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public bool IsFullscreen { get; private set; }

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

    private void Start()
    {
        CargarConfiguracion();
    }

    public void SetMusicVolume(float valor)
    {
        MusicVolume = valor;
        PlayerPrefs.SetFloat(MUSIC_KEY, valor);

        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(valor);
    }

    public void SetSFXVolume(float valor)
    {
        SFXVolume = valor;
        PlayerPrefs.SetFloat(SFX_KEY, valor);

        if (AudioManager.instance != null)
            AudioManager.instance.SetSFXVolume(valor);
    }

    public void SetFullscreen(bool valor)
    {
        IsFullscreen = valor;
        Screen.fullScreen = valor;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, valor ? 1 : 0);
        Debug.Log($"Pantalla completa: {valor}");
    }

    private void CargarConfiguracion()
    {
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1.0f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1.0f);
        IsFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;

        Screen.fullScreen = IsFullscreen;
        Debug.Log("Configuración cargada");

        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(MusicVolume);
            AudioManager.instance.SetSFXVolume(SFXVolume);
        }
    }
}