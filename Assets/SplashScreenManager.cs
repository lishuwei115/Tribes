using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenManager : MonoBehaviour
{
    public static SplashScreenManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public bool SplashActive = true;
    public void CloseSplash()
    {
        SplashActive = false;
        GetComponent<Animator>().SetBool("UIState", true);
    }

}
