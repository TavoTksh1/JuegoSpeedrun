using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource bgAudioSource;
    public AudioSource sfxAudioSource;

    [Header("Música")]
    public AudioClip musicMenu;
    public AudioClip musicGame;

    [Header("SFX - Jugador")]
    public AudioClip sfxJump;
    public AudioClip sfxShootNormal;
    public AudioClip sfxShootSpecial;
    public AudioClip sfxPlayerHurt;
    public AudioClip sfxPlayerDeath;

    [Header("SFX - Enemigo")]
    public AudioClip sfxEnemyHurt;
    public AudioClip sfxEnemyDeath;
    public AudioClip sfxEnemyManipulated;

    [Header("SFX - Coleccionables")]
    public AudioClip sfxCoin;
    public AudioClip sfxAmmoReload;
    public AudioClip sfxBulletPickup;

    [Header("SFX - Hazards")]
    public AudioClip sfxSpike;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Crea los AudioSource por código si no están asignados
            if (bgAudioSource == null)
            {
                bgAudioSource = gameObject.AddComponent<AudioSource>();
                bgAudioSource.loop = true;
                bgAudioSource.playOnAwake = false;
                bgAudioSource.volume = 1f;
            }

            if (sfxAudioSource == null)
            {
                sfxAudioSource = gameObject.AddComponent<AudioSource>();
                sfxAudioSource.loop = false;
                sfxAudioSource.playOnAwake = false;
                sfxAudioSource.volume = 1f;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // ── Música ──────────────────────────────────────

    public void PlayMusicMenu()
    {
        PlayMusic(musicMenu);
    }

    public void PlayMusicGame()
    {
        PlayMusic(musicGame);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (bgAudioSource == null) return; // ← verifica que no esté destruido
        if (bgAudioSource.clip == clip && bgAudioSource.isPlaying) return;
        bgAudioSource.clip = clip;
        bgAudioSource.loop = true;
        bgAudioSource.Play();
    }

    public void StopMusic()
    {
        bgAudioSource.Stop();
    }

    // ── Volumen ─────────────────────────────────────

    public void SetMusicVolume(float valor)
    {
        bgAudioSource.volume = valor;
    }

    public void SetSFXVolume(float valor)
    {
        sfxAudioSource.volume = valor;
    }

    // ── SFX Jugador ─────────────────────────────────

    public void PlayJump() => PlaySFX(sfxJump);
    public void PlayShootNormal() => PlaySFX(sfxShootNormal);
    public void PlayShootSpecial() => PlaySFX(sfxShootSpecial);
    public void PlayPlayerHurt() => PlaySFX(sfxPlayerHurt);
    public void PlayPlayerDeath() => PlaySFX(sfxPlayerDeath);

    // ── SFX Enemigo ─────────────────────────────────

    public void PlayEnemyHurt() => PlaySFX(sfxEnemyHurt);
    public void PlayEnemyDeath() => PlaySFX(sfxEnemyDeath);
    public void PlayEnemyManipulated() => PlaySFX(sfxEnemyManipulated);

    // ── SFX Coleccionables ──────────────────────────

    public void PlayCoin() => PlaySFX(sfxCoin);
    public void PlayAmmoReload() => PlaySFX(sfxAmmoReload);
    public void PlayBulletPickup() => PlaySFX(sfxBulletPickup);

    // ── SFX Hazards ─────────────────────────────────

    public void PlaySpike() => PlaySFX(sfxSpike);

    // ── Helper ──────────────────────────────────────

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        if (sfxAudioSource == null) return; // ← verifica que no esté destruido
        sfxAudioSource.PlayOneShot(clip);
    }
}