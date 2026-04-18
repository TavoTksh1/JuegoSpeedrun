using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource bgAudioSource;
    public AudioSource sfxAudioSource;

    public AudioClip jump;
    public AudioClip coin;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance=this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    public void PlayJump()
    {
        PlaySound(jump);   
    }

    public void PlayCoin()
    {
        PlaySound(coin);   
    }

    private void PlaySound(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);
    }
}
