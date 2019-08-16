using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;
    public Animator controller;
    public Image WinSprite;
    public Image LoseSprite;
    public Sprite[] WinImages;
    public Sprite[] LoseImages;
    public HousesTypes[] houses;

    private void Awake()
    {
        Instance = this;
        controller = GetComponent<Animator>();
    }

    public void WinLoseState(int state)
    {
        int i = GetHouseNumber(GameManagerScript.Instance.PlayerHouse);
        WinSprite.sprite = WinImages[i];
        LoseSprite.sprite = LoseImages[i];
        controller.SetInteger("UIState", state);
    }

    private int GetHouseNumber(HousesTypes playerHouse)
    {
        for (int i = 0; i < houses.Length; i++)
        {
            if(playerHouse == houses[i])
            {
                return i;
            }
        }
        return 0;
    }

    public void CloseWinLoseMenu()
    {

        controller.SetInteger("UIState", 0);
    }

}
