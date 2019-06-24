using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;

public class HumanBeingScript : MonoBehaviour
{
    public float Radius = 2;
    public delegate void BackHome();
    public event BackHome FinallyBackHome;

    public delegate void Reproduced();
    public event Reproduced ReproducedEvent;


    [Range(0, 1000)]
    public float HpMax;
    [Range(0, 1000)]
    public float HpMin;

    public float Hp;
    public float BaseHp;

    [Range(0, 1000)]
    public float AttackMax;
    [Range(0, 1000)]
    public float AttackMin;

    public float Attack;

    [Range(0, 1000)]
    public float SpeedMax;
    [Range(0, 1000)]
    public float SpeedMin;

    public float Speed;

    [Range(0, 1000)]
    public float Charity;

    [Range(0, 1000)]
    public float Gratitude;

    [Range(0, 1000)]
    public float Hate;
    [Range(1, 100)]
    public float Hunger = 1;

    [Range(1, 100)]
    public float ReproductionPerc = 5;
    [Range(1, 100)]
    public float GivingPerc = 20;

    public float Food;

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
    [HideInInspector]
    public HouseScript TargetHouse;
    [HideInInspector]
    public Vector3 TargetDest;
    [HideInInspector]
    public Transform TargetFoodDest;
    [HideInInspector]
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
    public bool AmIActing = false;
    private IEnumerator FollowCo;
    private IEnumerator MoveCo;
    private MeshRenderer MR;
    private float OffsetTimer = 0;
    private void Awake()
    {
        MR = GetComponent<MeshRenderer>();
    }


    // Use this for initialization
    void Start()
    {
        //StartCoroutine(Live());
        Speed = UnityEngine.Random.Range(SpeedMin, SpeedMax) / 10;
        Hp = UnityEngine.Random.Range(HpMin, HpMax);
        BaseHp = Hp;
        Attack = UnityEngine.Random.Range(AttackMin, AttackMax);
        GameManagerScript.Instance.DayStarted += Instance_DayStarted;
    }

    // Update is called once per frame
    void Update()
    {
        //eat when starving
        if (Hp < 0)
        {
            if (Food == 0)
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
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            GoToRandomPos();
        }
        HType = Charity > Hate && Charity > Gratitude ? HumanType.Charity :
                 Hate > Charity && Hate > Gratitude ? HumanType.Hate :
                 Gratitude > Hate && Charity < Gratitude ? HumanType.Gratitude : HumanType.None;
        //Debug.Log(Vector3.Distance(transform.position, TargetDest) + "   " + name);



        //going back home if the safety time is finished
        if (Time.time - OffsetTimer ==TargetHouse.SafetyTimer && (CurrentState != StateType.Home && CurrentAction != ActionState.Fight && CurrentState != StateType.ComingBackHome))
        {
            GoToPosition(TargetHouse.transform.position);
            CurrentState = StateType.ComingBackHome;
            CanIgetFood = false;
        }
        //lose health when outside the house
        if( CurrentState != StateType.Home)
        {
            Hp -= Hunger*Time.deltaTime;
        }

    }


    public void GoToRandomPos()
    {
        if (gameObject.activeInHierarchy)
        {

            IsStarted = true;
            if (MoveCo != null)
            {
                StopCoroutine(MoveCo);
            }
            //set destination
            TargetDest = GameManagerScript.Instance.GetFreeSpaceOnGround(transform.position.y);
            MoveCo = Move(TargetDest);
            //move in a fixed amount of time 
            StartCoroutine(MoveCo);
        }
    }

    public void GoToPosition(Vector3 nextPos)
    {
        if (gameObject.activeInHierarchy)
        {
            IsStarted = true;
            if (MoveCo != null)
            {
                StopCoroutine(MoveCo);
            }

            MoveCo = Move(nextPos);
            StartCoroutine(MoveCo);
        }
    }

    public void HomeSweetHome()
    {
        /*if (Hp < BaseHp)
        {
            if (Food > 0)
            {
                Hp += Food;
                if (Hp > BaseHp)
                {
                    TargetHouse.FoodStore += Hp - BaseHp;
                    Hp = BaseHp;
                }
                else
                {
                    Hp += TargetHouse.FoodStore;
                    if (Hp > BaseHp)
                    {
                        TargetHouse.FoodStore = Hp - BaseHp;
                        Hp = BaseHp;
                    }
                }
            }
            else
            {
                Hp += TargetHouse.FoodStore;
                if (Hp > BaseHp)
                {
                    TargetHouse.FoodStore = Hp - BaseHp;
                    Hp = BaseHp;
                }
            }
        }
        else
        {
                    TargetHouse.FoodStore += Food;

        }*/
        TargetHouse.FoodStore += Food;
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


    private void OnTriggerEnter(Collider other)
    {
        /*if(GameManagerScript.Instance.GameStatus == GameStateType.DayStarted)
		{
			if (other.tag == "Food" && CurrentState != StateType.FoodFound && CanIgetFood)
            {
                //Debug.Log("Food");
                TargetFoodDest = other.transform.position;
                GoToPosition(other.transform.position);
                TargetFood = other.GetComponent<FoodScript>();
                CurrentState = StateType.FoodFound;
            }
			else if (other.tag == "Human" && CurrentState != StateType.FoodFound && CurrentState != StateType.Home)
            {
				MeetOthers(other);
            }
            else if (other.tag == "House")
            {
                //Debug.Log("House");
            }
		}*/

    }

    private void OnTriggerStay(Collider other)
    {
        /*if (GameManagerScript.Instance.GameStatus == GameStateType.DayStarted)
		{
			if (other.tag == "Food" && CurrentState != StateType.FoodFound && CanIgetFood)
			{
				//Debug.Log("Food");
				TargetFoodDest = other.transform.position;
				GoToPosition(other.transform.position);
				TargetFood = other.GetComponent<FoodScript>();
				CurrentState = StateType.FoodFound;
			}
			else if (other.tag == "Human" && CurrentState != StateType.FoodFound && CurrentState != StateType.Home)
            {
                MeetOthers(other);
            }
		}*/
    }

    private List<RaycastHit> LookAround(string layer)
    {
        LayerMask layerMask = 1 << LayerMask.NameToLayer(layer);
        List<RaycastHit> ElementHitted = new List<RaycastHit>();
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, Radius, transform.forward, Radius, layerMask);

        ElementHitted.AddRange(hits.ToList());

        return ElementHitted;
    }





    void Instance_DayStarted()
    {
        OffsetTimer = Time.time;
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
    public IEnumerator Move(Vector3 dest)
    {
        Vector3 offset = transform.position;
        float timeCount = 0;
        while (timeCount < 1)
        {
            List<RaycastHit> Humancollisions = LookAround("Enemy");
            List<RaycastHit> Foodcollisions = LookAround("Food");
            //Debug.Log(Foodcollisions.Count);
            /*if (Humancollisions.Count > 0 && TargetFoodDest == null && TargetHuman == null)
            {
                TargetHuman = Humancollisions[0].collider.transform;
            }*/
            if (Foodcollisions.Count > 0 && TargetFoodDest == null && TargetHuman == null)
            {
                TargetFoodDest = Foodcollisions[0].collider.transform;
                CurrentState = StateType.FoodFound;
                GoToPosition(TargetFoodDest.position);
            }

            yield return new WaitForFixedUpdate();
            transform.position = Vector3.Lerp(offset, dest, timeCount);
            timeCount = timeCount + Time.deltaTime * Speed;
            //timeCount = timeCount + Time.deltaTime * Speed * .1f * ((math.abs(.5f - math.abs(timeCount - .5f)) + 0.5f) / 2);

        }
        if (CurrentState == StateType.ComingBackHome)
        {
            CurrentState = StateType.Home;
            HomeSweetHome();
            //Instance_DayStarted();
        }
        else if (CurrentState == StateType.LookingForFood)
        {
            CurrentState = StateType.ComingBackHome;
            GoToPosition(TargetHouse.transform.position);
        }
        else if (CurrentState == StateType.FoodFound)
        {
            if (TargetFoodDest.gameObject.activeInHierarchy)
            {
                Food += TargetFoodDest.GetComponent<FoodScript>().Food;
                TargetFoodDest.gameObject.SetActive(false);
                DidIFindFood = true;
                CurrentState = StateType.ComingBackHome;
                TargetFoodDest = null;
                GoToPosition(TargetHouse.transform.position);
            }
            else
            {
                CurrentState = StateType.LookingForFood;
                TargetFoodDest = null;
                GoToPosition(TargetDest);
            }
        }
        //CurrentState = StateType.ComingBackHome;
        //GoToPosition(TargetHouse.transform.position);

        MoveCo = null;
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

    public void AttackEnemy(Transform humanT)
    {
        if (FollowCo == null)
        {
            FollowCo = FollowEnemy(humanT);
            StartCoroutine(FollowCo);
        }
    }

    public void UnderAttack(float damage)
    {
        Hp -= damage;
    }

    private IEnumerator FollowEnemy(Transform humanT)
    {
        bool EnemyAlive = true;
        HumanBeingScript Enemy = humanT.GetComponent<HumanBeingScript>();
        while (EnemyAlive)
        {
            GoToPosition(humanT.position);
            float Dist = Vector3.Distance(transform.position, humanT.position);
            if (Dist < 1f && CanIAttack)
            {
                CanIAttack = false;
                Invoke("AttackAction", 1);
                Enemy.UnderAttack(Attack);
            }

            if (!Enemy.gameObject.activeInHierarchy || Enemy.CurrentState == StateType.Home)
            {
                EnemyAlive = false;
            }
            yield return new WaitForEndOfFrame();
        }

        CurrentState = StateType.ComingBackHome;
        GoToPosition(TargetHouse.transform.position);
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
    ComingBackHome = 3
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