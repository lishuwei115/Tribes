using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class HouseScript : MonoBehaviour
{

    public float FoodStore;
    public int SafetyTimer = 13;
    [Range(0, 1000)]
    public float TimeAttackMin = 60;
    [Range(0, 1000)]
    public float TimeAttackMax = 180;

    public List<HumanBeingScript> Humans = new List<HumanBeingScript>();
    public List<HumanBeingScript> HumansAlive = new List<HumanBeingScript>();
    [HideInInspector]
    public bool IsPlayer = false;

    [Header("Government parameters")]
    [Range(0, 100)]
    public int TargetAutocracyHealth = 50;
    public HousesTypes HouseType;
    public GovernmentBehaviour Government;

    float TimeAttack;
    private void Start()
    {
        UIManagerScript.Instance.UpdateFood();

        TimeAttack = Time.time + UnityEngine.Random.Range(TimeAttackMin, TimeAttackMax);

    }

    private void Update()
    {
        if (!IsPlayer)
        {
            if (Time.time >= TimeAttack && GameManagerScript.Instance.currentDayTime >45)
            {
                TimeAttack = Time.time + UnityEngine.Random.Range(TimeAttackMin, TimeAttackMax);
                Transform target = null;
                while(target == null)
                {
                    int i = UnityEngine.Random.Range(0,GameManagerScript.Instance.Houses.Count*100)/100; // /100*100 is to increase randomness
                    if(GameManagerScript.Instance.Houses[i].HouseType != HouseType)
                    {
                        target = GameManagerScript.Instance.Houses[i].transform;
                    }
                }
                MoveTribeTo(target.position);
            }
        }
        HumansAlive = Humans.Where(x => x.Hp > 0).ToList();

    }


    internal void DistributeFood()
    {
        //GovernmentManaging();
        List<HumanBeingScript> living = Humans.Where(x => x.Hp > 0).ToList();
        if (FoodStore > living.Count)
        {
            foreach (HumanBeingScript human in living)
            {
                human.Hp = human.BaseHp;
                FoodStore--;
                human.Reproduce();
            }
        }
        else
        {
            for (int i = living.Count - 1; i >= 0; i--)
            {
                HumanBeingScript human = living[i];
                if (FoodStore > 0)
                {
                    FoodStore--;
                    human.Hp = human.BaseHp;
                    human.Reproduce();
                }
                else
                {
                    human.Hp = 0;
                    GameManagerScript.Instance.HumanBeingDied();
                    human.gameObject.SetActive(false);
                }
            }
        }
        UIManagerScript.Instance.UpdateFood();

    }

    private void GovernmentManaging()
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

    internal void MoveTribeTo(Vector3 transform)
    {
        foreach (HumanBeingScript human in Humans)
        {
            if (human.gameObject.activeInHierarchy&& (human.CurrentState != StateType.ComingBackHome && human.CurrentState != StateType.Home))
            {
                human.CurrentState = StateType.FollowInstruction;
                human.TargetFoodDest = null;
                human.TargetHuman = null;
                human.TargetDest = transform;
                human.GoToPosition(transform);
            }
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
            if (hpTarget - Humans[i].Hp > 0 && Humans[i].Hp > 0)
            {
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
    }
    /// <summary>
    /// Order and cure by heath the people 
    /// </summary>
    public void CureByHealth()
    {
        Humans = Humans.OrderBy(x => x.Hp).ToList();
        for (int i = 0; i < Humans.Count; i++)
        {
            if (Humans[i].Hp > 0)
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

    /// <summary>
    /// Order and cure by Speed the people, prioritize the faster people
    /// </summary>
    public void CureBySpeed()
    {
        Humans = Humans.OrderByDescending(x => x.Speed).ToList();
        for (int i = 0; i < Humans.Count; i++)
        {
            if (Humans[i].Hp > 0)
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
}


public enum GovernmentBehaviour
{
    Democracy,// Power of people, distributing resources equally
    Oligarchy,// Power of the few, selected people gain more resources
    Autocracy // All for one, absolute monarchy, resources goes to one
}