using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource BGSource;
    public AudioSource WorshipSource;
    public AudioClip BGMusic;
    public AudioClip Worship;
    public void Awake()
    {
        Instance = this;
        //BGSource.Stop();
        WorshipSource.Stop();

    }
    public void StartDay()
    {
        BGSource.clip = BGMusic;
        BGSource.Play();
    }
    public void StartWorship()
    {
        WorshipSource.clip = Worship;
        WorshipSource.Play();
    }
    public void StopWorship()
    {
        WorshipSource.Stop();
    }
}
