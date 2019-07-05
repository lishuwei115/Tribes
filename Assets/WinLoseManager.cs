using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;
    public Animator controller;

    private void Awake()
    {
        controller = GetComponent<Animator>();
    }

    public void WinLoseState(int state)
    {
        controller.SetInteger("UIState", state);
    }



}
