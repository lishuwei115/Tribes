using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class HouseScript : MonoBehaviour
{
    public bool Defeated = false;
    public float FoodStore;
    public int SafetyTimer = 13;
    [Range(0, 1000)]
    public int SafetyTimerMin = 60;
    [Range(0, 1000)]
    public int SafetyTimerMax = 100;
    [Range(0, 1000)]
    public float TimeAttackMin = 60;
    [Range(0, 1000)]
    public float TimeAttackMax = 180;
    [Range(0, 1000)]
    public float CultivateRadiusMax = 40;
    public Transform CultivateRadiusMaxCircle;
    public bool NewHouse = true;
    [Range(0, 1000)]
    public float CultivateRadiusMin = 7;
    public Transform CultivateCircle;
    public Transform CultivateRadiusMinCircle;
    [Range(0, 1000)]
    public float BuildingRadiusMin = 10;
    [Range(0, 1000)]
    public float BuildingRadiusMax = 80;
    public BuildingCircleScript BuildingCircle;
    public List<HumanBeingScript> Humans = new List<HumanBeingScript>();
    public List<HumanBeingScript> HumansAlive = new List<HumanBeingScript>();
    public bool IsPlayer = false;
    public Transform HouseSkin ;

    [Header("Government parameters")]
    [Range(0, 100)]
    public int TargetAutocracyHealth = 50;
    public HousesTypes HouseType;
    public GovernmentBehaviour Government;

    float TimeAttack;
    private void Awake()
    {
        HouseSkin = transform;
        BuildingCircle = transform.GetComponentInChildren<BuildingCircleScript>();
        BuildingCircle.Initialize(BuildingRadiusMin,BuildingRadiusMax);
    }
    private void Start()
    {
        CloseBuildingCircle(false);
        UIManagerScript.Instance.UpdateFood();
        HouseSkin = Instantiate(SkinManager.Instance.GetSkinInfo(HouseType).HousePrefab,transform);
        TimeAttack = Time.time + UnityEngine.Random.Range(TimeAttackMin, TimeAttackMax);
        SetCultivateCircle(false);
        SafetyTimer = UnityEngine.Random.Range(SafetyTimerMin, SafetyTimerMax);

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
        if (HumansAlive.Count <= 0 && !Defeated)
        {
            gameObject.SetActive(false);
            if (IsPlayer)
            {
                GameManagerScript.Instance.Lost();
            }
            if (GameManagerScript.Instance.StateOfGame == GameState.Playing)
            {
                GameManagerScript.Instance.EnemyDefeated();
            }
            Defeated = true;

        }
    }


    internal void DistributeFood()
    {
        //GovernmentManaging();
        if (NewHouse)
        {
            NewHouse = false;
        }
        else
        {
            List<HumanBeingScript> living = Humans.Where(x => x.Hp > 0).ToList();
            if (FoodStore > living.Count)
            {
                foreach (HumanBeingScript human in living)
                {
                    human.Hp = human.BaseHp;
                    FoodStore--;
                    //human.Reproduce();
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
                        //human.Reproduce();
                    }
                    else
                    {
                        human.Hp = 0;
                        GameManagerScript.Instance.HumanBeingDied();
                        human.gameObject.SetActive(false);
                    }
                }
            }
        }
        
        UIManagerScript.Instance.UpdateFood();

    }

    public void Breed()
    {
        if (NewHouse)
        {
            NewHouse = false;
        }
        else
        {
            List<HumanBeingScript> living = Humans.Where(x => x.Hp > 0).ToList();

            foreach (HumanBeingScript human in living)
            {
                human.Reproduce();
            }

        }
    }

    internal void Cultivate(List<HumanBeingScript> humans)
    {

        foreach (HumanBeingScript human in humans)
        {
            float distance = Vector3.Distance(human.transform.position, transform.position);
            SetCultivateCircle(true);
            if (distance > CultivateRadiusMin && distance < CultivateRadiusMax && human.gameObject.activeInHierarchy && (human.CurrentState != StateType.ComingBackHome && human.CurrentState != StateType.Home))
            {
                human.Cultivate();
                /*human.CurrentState = StateType.FollowInstruction;
                human.TargetFoodDest = null;
                human.TargetHuman = null;
                human.TargetDest = transform;
                human.GoToPosition(transform);*/
            }
        }
    }

    private void SetCultivateCircle(bool a)
    {
        CultivateCircle.gameObject.SetActive(false);

        CultivateCircle.gameObject.SetActive(a);
        CultivateRadiusMaxCircle.localScale = new Vector3(CultivateRadiusMax/3.5f, CultivateRadiusMax / 3.5f, CultivateRadiusMax / 3.5f);
        CultivateRadiusMinCircle.localScale = new Vector3(CultivateRadiusMin / 3.5f, CultivateRadiusMin / 3.5f, CultivateRadiusMin / 3.5f);
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

    internal void OpenBuildingCircle()
    {
        BuildingCircle.OpenBuildingCircle();
    }
    internal void CloseBuildingCircle(bool create)
    {
        if (create)
        {
            BuildingCircle.CreateHouse();

        }
        else
        {
            BuildingCircle.CloseBuildingCircle();

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