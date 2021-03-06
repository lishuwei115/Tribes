﻿using System;
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
    public float TimeAttackRangeIncreaseMin = 40;
    [Range(0, 1000)]
    public float TimeAttackRangeIncreaseMax = 60;
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
    public Transform GuardianCircle;
    public List<HumanBeingScript> Humans = new List<HumanBeingScript>();
    public List<HumanBeingScript> HumansAlive = new List<HumanBeingScript>();
    public bool IsPlayer = false;
    public Transform HouseSkin;
    public float TimetoReachInstruction = 0;
    [Header("Government parameters")]
    [Range(0, 100)]
    public int TargetAutocracyHealth = 50;
    public HousesTypes HouseType;
    public GovernmentBehaviour Government;
    int AttackIterated = 0;
    float TimeAttack;
    int AttackRangeIncreaseIterated = 0;
    float TimeAttackRange;
    [HideInInspector]
    public AnimationCurve PopulationDistribution;

    public float BuffTime = 20;

    public float InitialHumanRadius;

    public bool FiredUp { get; private set; }

    private void Awake()
    {
        HouseSkin = transform;
        BuildingCircle = transform.GetComponentInChildren<BuildingCircleScript>();
        BuildingCircle.Initialize(BuildingRadiusMin, BuildingRadiusMax);
    }
    private void Start()
    {

        CloseBuildingCircle(false);
        CloseGuardianCircle(false);
        UIManagerScript.Instance.UpdateFood();
        HouseSkin = Instantiate(SkinManager.Instance.GetSkinInfo(HouseType).HousePrefab, transform);
        TimeAttack = (UnityEngine.Random.Range(TimeAttackMin, TimeAttackMax)/(float)GameManagerScript.Instance.DayTime);
        TimeAttackRange = (UnityEngine.Random.Range(TimeAttackRangeIncreaseMin, TimeAttackRangeIncreaseMax) / (float)GameManagerScript.Instance.DayTime);
        SetCultivateCircle(false);
        SafetyTimer = UnityEngine.Random.Range(SafetyTimerMin, SafetyTimerMax);
        PopulationDistribution = GameManagerScript.Instance.PopulationDistribution;
    }

    private void Update()
    {
        if (!GameManagerScript.Instance.Pause)
        {


            if (!IsPlayer)
            {
                if (((float)UIManagerScript.Instance.DayNumIterator ) + (1f-((float)GameManagerScript.Instance.currentDayTime / (float)GameManagerScript.Instance.DayTime)) >= (float)TimeAttack )
                {
                    AttackIterated = UIManagerScript.Instance.DayNumIterator;
                    TimeAttack = AttackIterated+(UnityEngine.Random.Range(TimeAttackMin, TimeAttackMax) / (float)GameManagerScript.Instance.DayTime);
                    List<HouseScript> TargetHouse = GameManagerScript.Instance.Houses.Where(r => r.FoodStore > 0 && r.HouseType != HouseType).ToList();
                    int i = UnityEngine.Random.Range(0, TargetHouse.Count * 100) / 100; // /100*100 is to increase randomness
                    Transform target = TargetHouse[i].transform;
                    MoveTribeTo(target.position, GameManagerScript.Instance.FollowOrderRandomness);
                }
                if (((float)UIManagerScript.Instance.DayNumIterator )+(1-((float)GameManagerScript.Instance.currentDayTime/ (float)GameManagerScript.Instance.DayTime)) >= (float)TimeAttackRange  && !FiredUp)
                {
                    AttackRangeIncreaseIterated = UIManagerScript.Instance.DayNumIterator;
                    TimeAttackRange = AttackRangeIncreaseIterated+(UnityEngine.Random.Range(TimeAttackRangeIncreaseMin, TimeAttackRangeIncreaseMax) / (float)GameManagerScript.Instance.DayTime);
                    foreach (HumanBeingScript human in HumansAlive.Where(r=> r.HumanJob == HumanClass.Warrior))
                    {
                        InitialHumanRadius = human.Radius;
                        human.Radius = 1000;
                        human.FiredUpState(true);
                    }
                    FiredUp = true;
                    Invoke("ResetHumanRadius", BuffTime);
                }
            }
            HumansAlive = Humans.Where(x => x.Hp > 0).ToList();
            if (HumansAlive.Count <= 0 && !Defeated && FoodStore <= 0)
            {
                //gameObject.SetActive(false);
                HouseSkin.GetComponent<Animator>().SetBool("UIState", true);
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
        if(UnityEngine.Random.Range(0f,1.01f)>0.9f)
        AddPeople();
    }

    public void ResetHumanRadius()
    {
        AttackRangeIncreaseIterated = UIManagerScript.Instance.DayNumIterator;
        TimeAttackRange =  (UnityEngine.Random.Range(TimeAttackRangeIncreaseMin, TimeAttackRangeIncreaseMax) / (float)GameManagerScript.Instance.DayTime);
        foreach (HumanBeingScript human in HumansAlive)
        {
            human.Radius = InitialHumanRadius;
            human.FiredUpState(false);

        }
        FiredUp = false;

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
            List<HumanBeingScript> Warrior = living.Where(x => x.HumanJob == HumanClass.Warrior).ToList();

            if (FoodStore > (living.Count - Warrior.Count) + (Warrior.Count * GameManagerScript.Instance.FoodRequiredWarrior))
            {
                foreach (HumanBeingScript human in living)
                {
                    human.Hp = human.BaseHp;
                    FoodStore -= human.HumanJob == HumanClass.Harvester ? GameManagerScript.Instance.FoodRequiredHarvester : GameManagerScript.Instance.FoodRequiredWarrior;
                    //human.Reproduce();
                }
            }
            else
            {
                float percentage = FoodStore / ((living.Count - Warrior.Count) + (Warrior.Count * GameManagerScript.Instance.FoodRequiredWarrior));
                if (percentage < 0.1f)
                    percentage = 0.1f;

                //Distribute equally
                foreach (HumanBeingScript human in living)
                {
                    human.Hp = human.BaseHp * percentage;
                    human.HPBar.gameObject.SetActive(true);

                    human.HPBar.UpdateHP(human.Hp, human.BaseHp, HouseType);
                    human.HPBar.gameObject.SetActive(false);
                }
                FoodStore = 0;
                //distribute untill finishing resources
                /*for (int i = living.Count - 1; i >= 0; i--)
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
                }*/
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
            HumansAlive = Humans.Where(x => x.Hp > 0).ToList();

        }
    }

    internal void Cultivate(List<HumanBeingScript> humans)
    {

        foreach (HumanBeingScript human in humans)
        {
            float distance = Vector3.Distance(human.transform.position, transform.position);
            //SetCultivateCircle(true);
            if (/*distance < CultivateRadiusMax && */human.gameObject.activeInHierarchy && (human.CurrentState != StateType.ComingBackHome && human.CurrentState != StateType.Home))
            {
                human.Cultivate();
            }
        }
    }
    public void AddPeople()
    {
        if (!IsPlayer)
        {
            HumansAlive = Humans.Where(r => r.Hp > 0).ToList();
            List<HumanBeingScript> harvester = HumansAlive.Where(r => r.Hp > 0 && r.HumanJob == HumanClass.Harvester).ToList();
            List<HumanBeingScript> warrior = HumansAlive.Where(r => r.Hp > 0 && r.HumanJob == HumanClass.Warrior).ToList();
            float foodRequiredHarvester = (harvester.Count * GameManagerScript.Instance.FoodRequiredHarvester) + (warrior.Count * GameManagerScript.Instance.FoodRequiredWarrior) + GameManagerScript.Instance.FoodRequiredHarvester;
            float foodRequiredWarrior = (harvester.Count * GameManagerScript.Instance.FoodRequiredHarvester) + (warrior.Count * GameManagerScript.Instance.FoodRequiredWarrior) + GameManagerScript.Instance.FoodRequiredWarrior;
            float populationState = PopulationDistribution.Evaluate(((float)Humans.Where(r => r.Hp > 0).ToList().Count ) / (float)GameManagerScript.Instance.MaxHumansForTribe);
            if(harvester.Count == 0 && FoodStore > 0)
            {
                GameManagerScript.Instance.AddHumanUsingFood(this, HumanClass.Harvester);
                return;
            }
            if (HumansAlive.Count < GameManagerScript.Instance.MaxHumansForTribe)
                if ((float)harvester.Count / (float)HumansAlive.Count < 1f - populationState && (float)warrior.Count / (float)HumansAlive.Count >= populationState)
                {
                    if (FoodStore > foodRequiredHarvester)
                    {
                        GameManagerScript.Instance.AddHumanUsingFood(this, HumanClass.Harvester);
                    }
                }
                else if (FoodStore > foodRequiredWarrior)
                {
                    GameManagerScript.Instance.AddHumanUsingFood(this, HumanClass.Warrior);
                }
        }
    }
    private void SetCultivateCircle(bool a)
    {
        CultivateCircle.gameObject.SetActive(false);

        CultivateCircle.gameObject.SetActive(a);
        CultivateRadiusMaxCircle.localScale = new Vector3(CultivateRadiusMax / 3.5f, CultivateRadiusMax / 3.5f, CultivateRadiusMax / 3.5f);
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

    internal void MoveTribeTo(Vector3 t, float precision)
    {
        Vector3 randomPos = t;
        float timeHelper = 0;

        List<HumanBeingScript> humans;
        if (GameManagerScript.Instance.HumanSelected == HumanClass.Harvester && IsPlayer)
            humans = Humans.Where(r => r.Hp > 0 && r.HumanJob == HumanClass.Harvester).ToList();
        else
            humans = Humans.Where(r => r.Hp > 0 && r.HumanJob == HumanClass.Warrior).ToList();
        foreach (HumanBeingScript human in humans)
        {
            if (human.gameObject.activeInHierarchy && (human.CurrentState != StateType.ComingBackHome && human.CurrentState != StateType.Home))
            {
                randomPos = t + new Vector3(UnityEngine.Random.Range(-precision, precision), 0, UnityEngine.Random.Range(-precision, precision));
                human.CurrentState = StateType.FollowInstruction;
                human.TargetFoodDest = null;
                human.TargetHuman = null;
                human.TargetDest = new Vector3(t.x, 0, t.z);
                human.GoToPosition(randomPos);
                float distance = Vector3.Distance(human.transform.position, randomPos);
                float time = 1 / (Time.deltaTime * human.Speed / distance * 10) / 60;
                if (time > timeHelper)
                {
                    timeHelper = time;
                }
                human.TimetoReachInstruction = time;
            }
        }
        TimetoReachInstruction = timeHelper;

    }

    internal void OpenBuildingCircle()
    {
        BuildingCircle.OpenBuildingCircle();
        GameManagerScript.Instance.MapBorder.gameObject.SetActive(true);

    }
    internal void OpenGuardianCircle()
    {
        GuardianCircle.gameObject.SetActive(true);
        AddGuardianManager.Instance.gameObject.SetActive(true);
        WorldmapCamera.Instance.IsBuilding = true;
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
        GameManagerScript.Instance.MapBorder.gameObject.SetActive(false);

    }
    internal void CloseGuardianCircle(bool create)
    {
        if (create)
        {
            GuardianCircle.gameObject.SetActive(false);
            AddGuardianManager.Instance.gameObject.SetActive(false);

        }
        else
        {
            GuardianCircle.gameObject.SetActive(false);
            AddGuardianManager.Instance.gameObject.SetActive(false);
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