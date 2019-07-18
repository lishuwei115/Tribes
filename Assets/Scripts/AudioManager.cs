using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource BGSource;
    public AudioClip BGMusic;
    public void Awake()
    {
        Instance = this;
        BGSource.Stop();
    }
    public void StartDay()
    {
        BGSource.clip = BGMusic;
        BGSource.Play();
    }

}
