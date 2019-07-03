﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;
using System;

public class HumanBeingScript : MonoBehaviour
{
    public float Radius = 2;
    public delegate void BackHome();
    public event BackHome FinallyBackHome;

    public delegate void Reproduced();
    public event Reproduced ReproducedEvent;

    [Header("Controll parameters")]
    [Range(.1f, .9f)]
    [Tooltip("If 0.9 is Warrior, if 0.1 is Farmer, it will distribute the attack accordingly")]
    public float Specialization = 0.5f;
    public float InitialHP = 0;
    public float HPBonus = 0;
    public float PhisicalAttack;
    public float HarvestAttack;
    public float Attack;
    public float Speed;
    public float Food;
    [Tooltip("The life of the food attacked")]
    public float FoodLife = 0;

    [Header("Twickable parameters")]
    [Range(0, 1000)]
    public float HpMax;
    [Range(0, 1000)]
    public float HpMin;

    public float Hp;
    public float BaseHp;

    [Range(0, 1000)]
    public float SpeedMax;
    [Range(0, 1000)]
    public float SpeedMin;
    [Range(0, 1000)]
    public float AttackMax;
    [Range(0, 1000)]
    public float AttackMin;
    [Range(1, 100)]
    public float ReproductionPerc = 5;

    public float AttackFrequency = 1;
    [Header("Harvest/warrior increments")]
    [Range(0.001f,0.1f)]
    public float HarvestIncrement = 0.01f;
    [Range(0.001f, 0.1f)]
    public float WarriorIncrement = 0.01f;



    [Header("AdvancedParameters")]

    [Range(0, 1000)]
    public float Charity;

    [Range(0, 1000)]
    public float Gratitude;

    [Range(0, 1000)]
    public float Hate;
    [Range(1, 100)]
    public float Hunger = 1;

    
    [Range(1, 100)]
    public float GivingPerc = 20;


    
    public float GratitiudeFoodPerc = 10;
    public float CharityFoodPerc = 10;
    public float HateGratitutePerc = 10;
    public float HateHatePerc = 15;


    [Header("Charity")]
    public float CModifier = 1;
    public float CGModifier = 1;
    public float CHModifier = 1;
    public float CModifierHealth = 1;
    public float CModifierSpeed = 0;
    public float CModifierAttack = -0.5f;

    [Header("Gratitude")]
    public float GModifier = 1;
    public float GCModifier = 1;
    public float GHModifier = 1;
    public float GModifierHealth = -0.5f;
    public float GModifierSpeed = 1;
    public float GModifierAttack = 0;

    [Header("Hate")]
    public float HModifier = 1;
    public float HCModifier = 1;
    public float HGModifier = 1;
    public float HModifierHealth = 0;
    public float HModifierSpeed = -0.5f;
    public float HModifierAttack = 1;


    public HumanType HType = HumanType.None;
    public HousesTypes HouseType = HousesTypes.Center;
    public StateType CurrentState = StateType.Home;
    public ActionState CurrentAction = ActionState.None;

    public Material CharityM;
    public Material AttackM;
    public Material BegM;
    public Material BaseM;
    //Not visible in Inspector
    public HouseScript TargetHouse;
    public Vector3 TargetDest;
    public Transform TargetFoodDest;
    public Transform TargetHuman;
    [HideInInspector]
    public bool IsStarted = false;
    [HideInInspector]
    public FoodScript TargetFood;
    [HideInInspector]
    public bool DidIFindFood = false;
    [HideInInspector]
    public bool CanIgetFood = true;
    [HideInInspector]
    public bool CanIAttack = true;
    [HideInInspector]
    public bool FoodStealed = false;
    [HideInInspector]
    public bool AmIActing = false;
    private IEnumerator FollowCo;
    private IEnumerator MoveCo;
    private MeshRenderer MR;
    private float OffsetTimer = 0;
    
    bool AttackDecision = false;
    Animator AnimController = null;
    private void Awake()
    {
        MR = GetComponent<MeshRenderer>();
    }

    internal void WearSkin()
    {
        Transform gO = Instantiate(SkinManager.Instance.GetSkinFromHouse(HouseType), transform);
        if (gO.GetComponentInChildren<Animator>())
        {
            AnimController = gO.GetComponentInChildren<Animator>();
            AnimController.SetInteger("UIState", 0);
        }
    }


    // Use this for initialization
    void Start()
    {
        //StartCoroutine(Live());
        Speed = UnityEngine.Random.Range(SpeedMin, SpeedMax) / 10;
        Hp = UnityEngine.Random.Range(HpMin, HpMax);
        InitialHP = Hp;
        BaseHp = Hp;
        Attack = UnityEngine.Random.Range(AttackMin, AttackMax);
        GameManagerScript.Instance.DayStarted += Instance_DayStarted;
    }

    // Update is called once per frame
    void Update()
    {
        CheckBonusHealth();

        //eat when starving
        if (Hp < 0)
        {
            GameManagerScript.Instance.HumanBeingDied();
            gameObject.SetActive(false);
            /*if (Food == 0)
            {
                GameManagerScript.Instance.HumanBeingDied();
                gameObject.SetActive(false);
            }
            else
            {
                Hp += Food;
                Food = 0;
                if (Hp > BaseHp)
                {
                    Food = Hp - BaseHp;
                    Hp = BaseHp;
                }
            }*/
        }

     
        /*HType = Charity > Hate && Charity > Gratitude ? HumanType.Charity :
                 Hate > Charity && Hate > Gratitude ? HumanType.Hate :
                 Gratitude > Hate && Charity < Gratitude ? HumanType.Gratitude : HumanType.None;*/
        //Debug.Log(Vector3.Distance(transform.position, TargetDest) + "   " + name);



        //going back home if the safety time is finished
        if (Time.time - OffsetTimer >= TargetHouse.SafetyTimer && (CurrentState != StateType.Home /*&& CurrentAction != ActionState.Fight*/ && CurrentState != StateType.ComingBackHome))
        {
            FollowCo = null;
            CanIgetFood = false;
            CurrentState = StateType.ComingBackHome;
            GoToPosition(TargetHouse.transform.position);
        }





        //lose health when outside the house
        /*if (CurrentState != StateType.Home)
        {
            //Hp -= Hunger*Time.deltaTime;
        }*/

    }


    public void GoToRandomPos()
    {
        if (gameObject.activeInHierarchy)
        {
            TargetDest = GameManagerScript.Instance.GetFreeSpaceOnGround(transform.position.y);
            GoToPosition(TargetDest);
        }
    }
    public void MineFood(FoodScript food)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(Harvest(food));
        }
    }
    public void GoToPosition(Vector3 nextPos)
    {
        if (gameObject.activeInHierarchy)
        {
            if (FoodLife > 0)
            {
                FoodLife = 0;
                if (Specialization > 0.1f)
                {
                    //Specialization -= 0.005f;
                }
                else
                {
                    Specialization = 0.1f;
                }
                CheckBonusHealth();

            }

            //to do implement a proper coroutine managing
            StopAllCoroutines();
            IsStarted = true;
            //currentID++;
            /*if (MoveCo != null)
            {
                StopCoroutine(MoveCo);
            }*/
            //MoveCo = StartCoroutine(PublicMove(nextPos));
            MoveCo = Move(nextPos);
            StartCoroutine(MoveCo);
            if (AnimController != null)
            {
                //Walk animation if there is an animator
                AnimController.SetInteger("UIState", 0);
            }
        }
    }
    /// <summary>
    /// create a stoppable Coroutine, NOT WORKING
    /// </summary>
    /// <param name="dest"></param>
    /// <returns></returns>
    /*IEnumerator<int> PublicMove(Vector3 dest)
    {
        return new ResetableEnumerator<int>
        {
            Enumerator = Move(dest),
            ResetFunc = () =>
            {
                //Cleanup();
                return Move(dest);
            }
        };
    }*/

    public void HomeSweetHome()
    {

        TargetHouse.FoodStore += Food;
        UIManagerScript.Instance.UpdateFood();

        Food = 0;
        FinallyBackHome();
        //Reproduce();
        CanIgetFood = true;
        ResetAction();
    }

    public void Reproduce()
    {
        if (UnityEngine.Random.Range(0, 100) < ReproductionPerc)
        {

            GameManagerScript.Instance.Reproduction(TargetHouse);
        }
    }

    private List<RaycastHit> LookAround(string layer)
    {
        LayerMask layerMask = 1 << LayerMask.NameToLayer(layer);
        List<RaycastHit> ElementHitted = new List<RaycastHit>();
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, Radius, transform.forward, Radius, layerMask);

        ElementHitted.AddRange(hits.ToList());

        return ElementHitted;
    }
    private List<RaycastHit> LookAroundForEnemies()
    {
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Enemy");
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, Radius, transform.forward, Radius, layerMask);
        List<RaycastHit> Enemy = hits.Where(a => a.collider.tag == "Human" && a.collider.GetComponent<HumanBeingScript>().HouseType != HouseType && a.collider.gameObject.activeInHierarchy).ToList();

        return Enemy;
    }

    private void CheckSurroundings()
    {
        //checking collisions and grouping them by category
        List<RaycastHit> collisions = LookAround("Food");
        List<RaycastHit> Food = collisions.Where(a => a.collider.tag == "Food").ToList();
        List<RaycastHit> Enemy = collisions.Where(a => a.collider.tag == "Human" && a.collider.GetComponent<HumanBeingScript>().HouseType != HouseType).ToList();
        List<RaycastHit> Allies = collisions.Where(a => a.collider.tag == "Human" && a.collider.GetComponent<HumanBeingScript>().HouseType == HouseType).ToList();

        //check the nearest food resource



    }



    void Instance_DayStarted()
    {
        OffsetTimer = Time.time;
        FoodStealed = false;
        GoToRandomPos();

        CurrentState = StateType.LookingForFood;
    }


    private void MeetOthers(Collider other)
    {
        if (CurrentAction == ActionState.None && CurrentState != StateType.Home)
        {

            HumanBeingScript human = other.GetComponent<HumanBeingScript>();
            if (HouseType != human.HouseType)
            {
                ActionState CurrentEnemyAction = human.GetCurrentAction(this);
                GetCurrentAction(human);

                switch (CurrentAction)
                {
                    case ActionState.None:
                        break;
                    case ActionState.Charity:
                        switch (CurrentEnemyAction)
                        {
                            case ActionState.Begging:
                                human.Food += (Food / 100) * GivingPerc + (Food / 100) * GratitiudeFoodPerc;
                                Food -= (Food / 100) * GivingPerc - (Food / 100) * CharityFoodPerc;
                                break;
                        }
                        Invoke("ResetAction", 5);
                        break;
                    case ActionState.Begging:
                        Invoke("ResetAction", 5);
                        break;
                    case ActionState.Fight:
                        switch (CurrentEnemyAction)
                        {
                            case ActionState.Charity:
                                AttackEnemy(human.transform);
                                break;
                            case ActionState.Begging:
                                Attack += (Attack / 100) * HateGratitutePerc;
                                AttackEnemy(human.transform);
                                break;
                            case ActionState.Fight:
                                Attack += (Attack / 100) * HateHatePerc;
                                AttackEnemy(human.transform);
                                break;
                        }
                        break;
                }
            }
        }
    }

    public void ResetAction()
    {
        CurrentAction = ActionState.None;
        MR.material = BaseM;
    }


    public ActionState GetCurrentAction(HumanBeingScript enemy)
    {
        if (CurrentAction == ActionState.None && CurrentState != StateType.Home)
        {

            if (DidIFindFood)
            {
                if (enemy.DidIFindFood)
                {
                    float AttackPerc = (Hate * 100) / (Charity + Hate);
                    CurrentAction = UnityEngine.Random.Range(0, 99) < AttackPerc ? ActionState.Fight : ActionState.Charity;
                }
                else
                {
                    float CharityPerc = (Hate * 100) / (Charity + Hate);
                    CurrentAction = UnityEngine.Random.Range(0, 99) < CharityPerc ? ActionState.Charity : ActionState.Fight;
                }
            }
            else
            {
                if (enemy.DidIFindFood)
                {
                    float AttackPerc = (Hate * 100) / (Gratitude + Hate);
                    CurrentAction = UnityEngine.Random.Range(0, 99) < AttackPerc ? ActionState.Fight : ActionState.Begging;
                }
                else
                {
                    CurrentAction = ActionState.None;
                }
            }

            switch (CurrentAction)
            {
                case ActionState.Charity:
                    Charity += CModifier;
                    Gratitude += CGModifier;
                    Hate += CHModifier;
                    BaseHp += CModifierHealth;
                    Speed += CModifierSpeed;
                    Attack += CModifierAttack;
                    MR.material = CharityM;
                    break;
                case ActionState.Begging:
                    Gratitude += GModifier;
                    Charity += GCModifier;
                    Hate += GHModifier;
                    BaseHp += GModifierHealth;
                    Speed += GModifierSpeed;
                    Attack += GModifierAttack;
                    MR.material = BegM;
                    break;
                case ActionState.Fight:
                    Hate += HModifier;
                    Gratitude += HGModifier;
                    Charity += HCModifier;
                    BaseHp += HModifierHealth;
                    Speed += HModifierSpeed;
                    Attack += HModifierAttack;
                    MR.material = AttackM;
                    break;
            }
        }

        return CurrentAction;

    }

    /// <summary>
    /// Move to a specified dest.
    /// using the new JobSystem
    /// </summary>
    /// <returns>The move.</returns>
    /// <param name="dest">Destination.</param>
    /*public IEnumerator Move(Vector3 dest)
	{
		Vector3 offset = transform.position;
		float timeCount = 0;
		while (timeCount < 1)
        {
			NativeArray<float3> result = new NativeArray<float3>(1, Allocator.TempJob);
            NativeArray<float> timer = new NativeArray<float>(1, Allocator.TempJob);
			MoveJob jobData = new MoveJob();

			jobData.HumanPos = offset;
            jobData.Dest = dest;
            jobData.result = result;
            jobData.timerRes = timer;
			jobData.Timer = timeCount;
			// Schedule the job
            JobHandle handle = jobData.Schedule();
            // Wait for the job to complete
            handle.Complete();
            transform.position = result[0];
            timeCount = jobData.timerRes[0];
			result.Dispose();
            timer.Dispose();
			yield return new WaitForFixedUpdate();

        }
		MoveCo = null;
	}*/


    /// <summary>
    /// Move to a specified dest.
    /// using the old system
    /// </summary>
    /// <returns>The move.</returns>
    /// <param name="dest">Destination.</param>
    //private int currentID = 0;
    public IEnumerator Move(Vector3 dest)
    {
        Vector3 offset = transform.position;
        float distance = Vector3.Distance(offset, dest);
        //int iD = currentID;
        if (CurrentState != StateType.FoodFound)
        {
            TargetFoodDest = null;
        }

        TargetHuman = null;

        float timeCount = 0;
        while (timeCount < 1)
        {
            List<RaycastHit> EnemiesCollision = LookAroundForEnemies();
            List<RaycastHit> Foodcollisions = LookAround("Food");
            List<RaycastHit> Housecollisions = LookAround("House");
            //Debug.Log(Foodcollisions.Count);
            //Debug.Log(iD);
            AttackOrFood(EnemiesCollision, Foodcollisions);

            if (CurrentState != StateType.FollowInstruction && !FoodStealed && CurrentState == StateType.LookingForFood && TargetFoodDest == null && TargetHuman == null && Housecollisions.Count > 0 && Housecollisions[0].collider.GetComponent<HouseScript>().FoodStore > 0 && Housecollisions[0].collider.GetComponent<HouseScript>().HouseType != HouseType)
            {
                TargetFoodDest = Housecollisions[0].collider.transform;
                CurrentState = StateType.FoodFound;
                GoToPosition(TargetFoodDest.position);
            }

            if (CurrentState != StateType.FollowInstruction && CurrentState == StateType.LookingForFood && Foodcollisions.Count > 0 && TargetFoodDest == null && TargetHuman == null)
            {
                TargetFoodDest = Foodcollisions[0].collider.transform;
                CurrentState = StateType.FoodFound;

                GoToPosition(TargetFoodDest.position);
            }
            if (CurrentState != StateType.FollowInstruction && CurrentState != StateType.ComingBackHome && EnemiesCollision.Count > 0 /*&& TargetFoodDest == null*/ && TargetHuman == null)
            {
                TargetHuman = EnemiesCollision[0].collider.transform;
                AttackEnemy(TargetHuman);
            }
            yield return new WaitForFixedUpdate();
            Vector3 nextpos = Vector3.Lerp(offset, dest, timeCount);
            transform.position = nextpos;

            timeCount = timeCount + Time.deltaTime * Speed / distance * 10;
            //timeCount = timeCount + Time.deltaTime * Speed/ distance*10 * ((math.abs(.5f - math.abs(timeCount - .5f)/3) + 1f) ); 
            //timeCount = timeCount + Time.deltaTime * Speed * .1f * ((math.abs(.5f - math.abs(timeCount - .5f)) + 0.5f) / 2);

        }
        if (CurrentState == StateType.FollowInstruction)
        {
            Debug.Log("arrived destination");
        }
            if (CurrentState == StateType.ComingBackHome)
        {
            CurrentState = StateType.Home;
            HomeSweetHome();
        }
        else if (CurrentState == StateType.LookingForFood || CurrentState == StateType.FollowInstruction )
        {
            //CurrentState = StateType.ComingBackHome;
            //GoToPosition(TargetHouse.transform.position);
            CurrentState = StateType.LookingForFood;
            GoToRandomPos();
        }
        else if (CurrentState == StateType.FoodFound)
        {
            if (TargetFoodDest.gameObject.layer == LayerMask.NameToLayer("House"))
            {
                if (TargetFoodDest.GetComponent<HouseScript>().FoodStore > 0)
                {
                    Food += 1;
                    FoodStealed = true;
                    TargetFoodDest.GetComponent<HouseScript>().FoodStore--;
                    DidIFindFood = true;
                    UIManagerScript.Instance.UpdateFood();
                }
                
                CurrentState = StateType.LookingForFood;
                GoToRandomPos();
            }
            else
            {
                if (TargetFoodDest.gameObject.activeInHierarchy)
                {
                    /*Food += TargetFoodDest.GetComponent<FoodScript>().Food;
                    TargetFoodDest.gameObject.SetActive(false);*/
                    DidIFindFood = true;
                    MineFood(TargetFoodDest.GetComponent<FoodScript>());
                    /*CurrentState = StateType.LookingForFood;
                    GoToRandomPos();*/
                }
                else
                {
                    CurrentState = StateType.LookingForFood;
                    GoToRandomPos();
                }
            }

        }
        //CurrentState = StateType.ComingBackHome;
        //GoToPosition(TargetHouse.transform.position);


        MoveCo = null;
    }

    private void AttackOrFood(List<RaycastHit> EnemiesCollision, List<RaycastHit> Foodcollisions)
    {
        if (EnemiesCollision.Count > 0 && Foodcollisions.Count > 0 && TargetFoodDest == null && TargetHuman == null)
        {

            int chance = UnityEngine.Random.Range(0, 100);
            if (chance > Specialization * 100)
            {
                //attack
                AttackDecision = true;
            }
            else
            {
                AttackDecision = false;
                //farm
            }
            if (CurrentState != StateType.FollowInstruction && CurrentState == StateType.LookingForFood && !AttackDecision)
            {
                TargetFoodDest = Foodcollisions[0].collider.transform;
                CurrentState = StateType.FoodFound;

                GoToPosition(TargetFoodDest.position);
            }
            if (CurrentState != StateType.FollowInstruction && CurrentState != StateType.ComingBackHome && AttackDecision)
            {
                TargetHuman = EnemiesCollision[0].collider.transform;
                AttackEnemy(TargetHuman);
            }
        }
    }

    public void AttackEnemy(Transform humanT)
    {
        if (FollowCo == null)
        {
            StopAllCoroutines();
            MoveCo = null;

            FollowCo = FollowEnemy(humanT);
            StartCoroutine(FollowCo);
        }
    }

    public void UnderAttack(float damage)
    {
        Hp -= damage;
        List<RaycastHit> EnemiesCollision = LookAroundForEnemies();
        //Counterattack
        if (Time.time - OffsetTimer < TargetHouse.SafetyTimer && EnemiesCollision.Count >0 )
        {
            AttackEnemy(EnemiesCollision[0].collider.transform);
        }
    }
    private IEnumerator Harvest(FoodScript food)
    {
        bool ResourceAvaiable = true;
        if (FoodLife <= 0)
        {
            FoodLife = food.Hardness;
        }

        while (ResourceAvaiable)
        {
            //move towars target
            while (FoodLife > 0)
            {
                FoodLife -= HarvestAttack;
                yield return new WaitForSeconds(AttackFrequency);
            }
            if (Specialization > 0.1f)
            {
                Specialization-=HarvestIncrement;

            }
            else
            {
                Specialization = 0.1f;
            }
            CheckBonusHealth();

            //Attack
            if (FoodLife <= 0 && food.Food > 0)
            {
                FoodLife = food.Hardness;
                food.Food -= 1;
                Food += 1;
                ResourceAvaiable = food.Food > 0;
                if (!ResourceAvaiable)
                {
                    DidIFindFood = true;
                    food.gameObject.SetActive(false);
                    CurrentState = StateType.LookingForFood;
                    GoToRandomPos();
                }
            }
            ResourceAvaiable = food.Food > 0;
            
            yield return new WaitForEndOfFrame();
        }

        CurrentState = StateType.LookingForFood;
        GoToRandomPos();
        FollowCo = null;
    }
    private IEnumerator FollowEnemy(Transform humanT)
    {
        bool EnemyAlive = true;
        Vector3 offset = transform.position;
        float distance = Vector3.Distance(offset, humanT.position);
        float timeCount = 0;
        HumanBeingScript Enemy = humanT.GetComponent<HumanBeingScript>();
        while (EnemyAlive)
        {
            //move towars target
            while (distance >= 2f)
            {
                distance = Vector3.Distance(transform.position, humanT.position);
                //offset = transform.position;
                timeCount = 0;
                yield return new WaitForFixedUpdate();
                timeCount = timeCount + Time.deltaTime * Speed / distance * 10;
                Vector3 nextpos = Vector3.Lerp(transform.position, humanT.position, timeCount);
                transform.position = nextpos;
            }
            //start another coroutine, not compatible with current system
            //GoToPosition(humanT.position);
            float Dist = Vector3.Distance(transform.position, humanT.position);
            //Attack
            if (Dist < 2f && CanIAttack)
            {
                CanIAttack = false;
                Invoke("AttackAction", AttackFrequency);
                Enemy.UnderAttack(PhisicalAttack);
                if (AnimController != null)
                {
                    //AttackAnimation if there is an animator
                    AnimController.SetInteger("UIState", 2);
                }
                if (Enemy.Hp <= 0)
                {
                    Food += Enemy.Food;
                    Enemy.Food = 0;
                }
            }
            /*if(Dist>2 && Enemy.gameObject.activeInHierarchy)
            {
                FollowCo = null;
                AttackEnemy(Enemy.transform);
            }
            else*/
            if (Dist > 2)
            {
                FollowCo = null;

                CurrentState = StateType.LookingForFood;
                GoToRandomPos();
            }
            //won?
            if (!Enemy.gameObject.activeInHierarchy || Enemy.CurrentState == StateType.Home)
            {
                if (Specialization < 0.9f)
                {
                    Specialization += WarriorIncrement;

                }
                else
                {
                    Specialization = 0.9f;
                }
                CheckBonusHealth();
                EnemyAlive = false;

            }
            yield return new WaitForSeconds(AttackFrequency);
        }

        CurrentState = StateType.LookingForFood;
        GoToRandomPos();
        FollowCo = null;
    }

    private void CheckBonusHealth()
    {
        if(Math.Abs( (Specialization*100-50)/2)+InitialHP > BaseHp)
        {
            BaseHp = Math.Abs((Specialization * 100 - 50) / 2) + InitialHP;
            HPBonus = (Specialization * 100 - 50) / 2;
        }
        PhisicalAttack = Specialization * Attack;
        HarvestAttack = (1 - Specialization) * Attack;
    }

    private void AttackAction()
    {
        CanIAttack = true;
    }
}



public enum StateType
{
    Home = 0,
    LookingForFood = 1,
    FoodFound = 2,
    ComingBackHome = 3,
    FollowInstruction = 4
}


public enum ActionState
{
    None,
    Fight,
    Charity,
    Begging
}

public enum HumanType
{
    None,
    Charity,
    Gratitude,
    Hate
}