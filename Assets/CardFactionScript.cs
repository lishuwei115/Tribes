using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFactionScript : MonoBehaviour
{
    public Transform Enemy;
    public Transform Player;
    public bool IsPlayer = false;
    public HousesTypes house;

    public void SetPlayer(bool b)
    {
        IsPlayer = b;
        if (IsPlayer)
        {
            Player.gameObject.SetActive(true);
            Enemy.gameObject.SetActive(false);
        }
        else
        {
            Player.gameObject.SetActive(false);
            Enemy.gameObject.SetActive(true);
        }
    }
}
