using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class HouseScript : MonoBehaviour {

	public float FoodStore;
    public int SafetyTimer = 13;
    [Range(0, 100)]
    public int TargetAutocracyHealth = 50;
    public HousesTypes HouseType;
    public GovernmentBehaviour Government;
    public List<HumanBeingScript> Humans = new List<HumanBeingScript>();

    internal void DistributeFood()
    {
        switch (Government)
        {
            case GovernmentBehaviour.Democracy:
                CureByHealth();
                break;
            case GovernmentBehaviour.Oligarchy:
                CureBySpeed();
                break;
            case GovernmentBehaviour.Autocracy:
                CureByPercentage();
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// Order and cure by heath the people only up to the percentage decided beforehand
    /// </summary>
    public void CureByPercentage()
    {
        Humans = Humans.OrderBy(x => x.Hp).ToList();
        float hpTarget = 0;
        for (int i = 0; i < Humans.Count; i++)
        {
            hpTarget = Humans[i].BaseHp * TargetAutocracyHealth / 100;
            if (FoodStore >= hpTarget - Humans[i].Hp)
            {
                FoodStore -= hpTarget - Humans[i].Hp;
                Humans[i].Hp = hpTarget;
            }
            else
            {
                Humans[i].Hp += FoodStore;
                FoodStore = 0;
            }
        }
    }
    /// <summary>
    /// Order and cure by heath the people 
    /// </summary>
    public void CureByHealth()
    {
        Humans = Humans.OrderBy(x => x.Hp).ToList();
        for (int i = 0; i < Humans.Count; i++)
        {
            if (FoodStore >= Humans[i].BaseHp - Humans[i].Hp)
            {
                FoodStore -= Humans[i].BaseHp - Humans[i].Hp;
                Humans[i].Hp = Humans[i].BaseHp;
            }
            else
            {
                Humans[i].Hp += FoodStore;
                FoodStore = 0;
            }
        }
    }

    /// <summary>
    /// Order and cure by Speed the people, prioritize the faster people
    /// </summary>
    public void CureBySpeed()
    {
        Humans = Humans.OrderBy(x => x.Speed).ToList();
        for (int i = 0; i < Humans.Count; i++)
        {
            if (FoodStore >= Humans[i].BaseHp - Humans[i].Hp)
            {
                FoodStore -= Humans[i].BaseHp - Humans[i].Hp;
                Humans[i].Hp = Humans[i].BaseHp;
            }
            else
            {
                Humans[i].Hp += FoodStore;
                FoodStore = 0;
            }
        }
    }
}


public enum GovernmentBehaviour
{
    Democracy,// Power of people, distributing resources equally
    Oligarchy,// Power of the few, selected people gain more resources
    Autocracy // All for one, absolute monarchy, resources goes to one
}