﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActionCaller : MonoBehaviour
{
    public Color BreedOn = Color.white;
    public Color BreedOff = Color.white;
    public TextMeshProUGUI BreedOnOffText;
    public TextMeshProUGUI AttackOnOffText;
    public Color AttackOn;
    public Color AttackOff;
    public TextMeshProUGUI MonsterNumber = null;

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if(MonsterNumber != null)
        {
            if (GameManagerScript.Instance.GuardiansSummonable > 0)
            {
                GetComponent<Button>().interactable = true;
            }
            else
            {
                GameManagerScript.Instance.CloseGuardianMenu();
                GetComponent<Button>().interactable = false;

            }
            MonsterNumber.text = GameManagerScript.Instance.GuardiansSummonable.ToString();
        }
        
    }

    public void SelectWarriors()
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)
            if (!GameManagerScript.Instance.Pause)
            {
                GameManagerScript.Instance.HumanSelected = HumanClass.Warrior;
                WorldmapCamera.Instance.PressFakeButton("SelectWarrior", 0.1f);
                WorldmapCamera.Instance.DeactiveFakeButton("SelectFarmer");
            }
    }

    public void SelectFarmers()
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)
            if (!GameManagerScript.Instance.Pause)
            {
                GameManagerScript.Instance.HumanSelected = HumanClass.Harvester;
                WorldmapCamera.Instance.PressFakeButton("SelectFarmer", 0.1f);

                WorldmapCamera.Instance.DeactiveFakeButton("SelectWarrior");
            }
    }

    public void GoTo()
    {
        if (!GameManagerScript.Instance.Pause)
        {
            WorldmapCamera.Instance.PressFakeButton("GoToPosition", 0.1f);
            WorldmapCamera.Instance.TribeToPoint(new Vector2(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2));
        }
    }
    public void CultivatePlayer()
    {
        GameManagerScript.Instance.Cultivate();
    }
    public void AddFarmer()
    {
        GameManagerScript.Instance.AddPlayerHarvester();
    }
    public void AddWarrior()
    {
        GameManagerScript.Instance.AddPlayerWarrior();
    }
    public void AddHouse()
    {
        GameManagerScript.Instance.AddHouse(GameManagerScript.Instance.PlayerHouse);
    }
    public void AddGuardian()
    {
        GameManagerScript.Instance.AddMonsterMenu(GameManagerScript.Instance.PlayerHouse);
    }
    public void ToogleBreeding()
    {
        bool breeding = GameManagerScript.Instance.ToogleBreeding();
        GetComponent<Image>().color = breeding ? BreedOn : BreedOff;
        BreedOnOffText.text = breeding ? "is ON" : "is OFF";
    }
    public void ToogleAttack()
    {
        bool attacking = GameManagerScript.Instance.ToogleAttack();
        GetComponent<Image>().color = attacking ? AttackOn : AttackOff;
        AttackOnOffText.text = attacking ? "is ON" : "is OFF";
    }

}
