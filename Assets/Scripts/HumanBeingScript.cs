using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class HumanBeingScript : MonoBehaviour
{
    public HousesTypes HouseType = HousesTypes.Red;
    public StateType CurrentState = StateType.Home;
    [Tooltip("radius of research")]
    public float Radius = 20;
    float RadiusFarmer = 20;
    public float RadiusWarrior = 20;
    public float AttackDistance = 5f;
    public delegate void BackHome();
    public event BackHome FinallyBackHome;

    public delegate void Reproduced();
    public event Reproduced ReproducedEvent;

    [Header("Info parameters - not twickable")]
    [Range(.1f, .9f)]
    [Tooltip("If 0.9 is Warrior, if 0.1 is Farmer, it will distribute the attack accordingly")]
    public float Specialization = 0.5f;
    public HumanClass HumanJob = HumanClass.Standard;
    public float InitialHP = 0;
    public float HPBonus = 0;
    public float PhisicalAttack;
    public float HarvestAttack;
    public float CultivationProgress = 0;
    public float Hp;
    public float BaseHp;
    public float Attack;
    public float Speed;
    public float TimeToGoBack;
    public float SafetyTime = 0;
    public float Food;
    public bool DeliverFood;
    [Tooltip("The life of the food attacked")]
    public float FoodLife = 0;
    public float TransparentDistanceFromHome;

    [Header("Twickable parameters")]
    [Range(0, 1000)]
    public float RayMaxDistance = 5;
    
    [Range(0, 1000)]
    public float RayMaxDistanceWarrior = 5;
    public float FoodRandomPointDistance = 4;
    [Range(0, 1000)]
    public float CultivationTarget = 300;
    public List<float> CultivationPercentage = new List<float> { 70, 90, 100 };
    float RandomFoodGained;
    float HpMax;
    float HpMin;
    float StorageCapacity = 1;

    [Header("Farmers parameters")]
    [Range(0, 1000)]
    private float RayMaxDistanceFarmer = 5;
    [Range(0, 30)]
    public float StorageCapacityFarmer = 1;
    [Range(0, 1000)]
    public float HpMaxFarmer;
    [Range(0, 1000)]
    public float HpMinFarmer;
    [Range(1, 100)]
    public float ReproductionPercFarmer = 5;
    [Range(0, 1000)]
    public float SpeedMaxFarmer;
    [Range(0, 1000)]
    public float SpeedMinFarmer;
    [Range(0, 1000)]
    public float AttackMaxFarmer;
    [Range(0, 1000)]
    public float AttackMinFarmer;
    public float AttackFrequencyFarmer = 1;


    [Header("Warriors parameters")]
    [Range(0, 30)]
    public float StorageCapacityWarrior = 1;
    [Range(0, 1000)]
    public float HpMaxWarrior;
    [Range(0, 1000)]
    public float HpMinWarrior;

    float SpeedMax;
    float SpeedMin;
    float AttackMax;
    float AttackMin;
    [Range(0, 1000)]
    public float SpeedMaxWarrior;
    [Range(0, 1000)]
    public float SpeedMinWarrior;
    [Range(0, 1000)]
    public float AttackMaxWarrior;
    [Range(0, 1000)]
    public float AttackMinWarrior;
    [Range(1, 100)]
    public float ReproductionPercWarrior = 5;
    float ReproductionPerc = 5;

    public float AttackFrequencyWarrior = 1;
    float AttackFrequency = 1;

    [Header("Harvest/warrior increments")]


    [Range(0.00f, 0.1f)]
    public float HarvestIncrement = 0.01f;
    [Range(0.00f, 0.1f)]
    public float WarriorIncrement = 0.01f;
    [Range(.1f, .9f)]
    public float WarriorValue = 0.6f;
    [Range(.1f, .9f)]
    public float HarvesterValue = 0.4f;


    [Header("AdvancedParameters")]
    [HideInInspector]
    [Range(0, 1000)]
    public float Charity;
    [HideInInspector]
    [Range(0, 1000)]
    public float Gratitude;

    [HideInInspector]
    [Range(0, 1000)]
    public float Hate;
    [HideInInspector]
    [Range(1, 100)]
    public float Hunger = 1;


    [HideInInspector]
    [Range(1, 100)]
    public float GivingPerc = 20;



    [HideInInspector]
    public float GratitiudeFoodPerc = 10;
    [HideInInspector]
    public float CharityFoodPerc = 10;
    [HideInInspector]
    public float HateGratitutePerc = 10;
    [HideInInspector]
    public float HateHatePerc = 15;


    [Header("Charity")]
    [HideInInspector]
    public float CModifier = 1;
    [HideInInspector]
    public float CGModifier = 1;
    [HideInInspector]
    public float CHModifier = 1;
    [HideInInspector]
    public float CModifierHealth = 1;
    [HideInInspector]
    public float CModifierSpeed = 0;
    [HideInInspector]
    public float CModifierAttack = -0.5f;

    [Header("Gratitude")]
    [HideInInspector]
    public float GModifier = 1;
    [HideInInspector]
    public float GCModifier = 1;
    [HideInInspector]
    public float GHModifier = 1;
    [HideInInspector]
    public float GModifierHealth = -0.5f;
    [HideInInspector]
    public float GModifierSpeed = 1;
    [HideInInspector]
    public float GModifierAttack = 0;

    [Header("Hate")]
    [HideInInspector]
    public float HModifier = 1;
    [HideInInspector]
    public float HCModifier = 1;
    [HideInInspector]
    public float HGModifier = 1;
    [HideInInspector]
    public float HModifierHealth = 0;
    [HideInInspector]
    public float HModifierSpeed = -0.5f;
    [HideInInspector]
    public float HModifierAttack = 1;


    public HumanType HType = HumanType.None;

    public ActionState CurrentAction = ActionState.None;

    [HideInInspector]
    public Material CharityM;
    [HideInInspector]
    public Material AttackM;
    [HideInInspector]
    public Material BegM;
    [HideInInspector]
    public Material BaseM;
    [HideInInspector]
    public HealthBarSprite HPBar;
    public HarvestingBarSprite HarvestBar;
    public Transform CultivationField;
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
    int Id = 0;
    bool AttackDecision = false;
    Animator AnimController = null;

    LayerMask EnemyLayer;
    Transform StandardSkin = null;
    Transform WarriorSkin = null;
    Transform HarvesterSkin = null;
    bool initialized = false;
    Vector3 RandomVector = new Vector3();
    public float TimetoReachInstruction;
    public List<Vector3> PathToHome;
    public int PathIndex = 0;

    public bool WaitingForOthers { get; private set; }

    private void Awake()
    {
        MR = GetComponent<MeshRenderer>();
    }




    // Use this for initialization
    void Start()
    {
        if (!initialized)
            Initialize();
    }
    public void Initialize()
    {
        HarvestBar = GetComponentInChildren<HarvestingBarSprite>();
        HarvestBar.gameObject.SetActive(false);

        HPBar = GetComponentInChildren<HealthBarSprite>();
        HPBar.gameObject.SetActive(false);
        InitializeWarriorFarmerParameters();
        InitializeRandomParameters();
        GameManagerScript.Instance.DayStarted += Instance_DayStarted;
        //Set Layer of allies and enemies
        SetLayers();
        List<RaycastHit> Housecollisions = LookAround("House");
        //AnimController.gameObject.SetActive(false);
        InvisibilityIfHouse(Housecollisions);
        initialized = true;
    }

    private void InitializeWarriorFarmerParameters()
    {
        if(HumanJob == HumanClass.Warrior)
        {
            Radius = RadiusWarrior;
            RayMaxDistance = RayMaxDistanceWarrior;
            StorageCapacity = StorageCapacityWarrior;
            HpMin = HpMinWarrior;
            HpMax = HpMaxWarrior;
            SpeedMin = SpeedMinWarrior;
            SpeedMax = SpeedMaxWarrior;
            AttackMin = AttackMinWarrior;
            AttackMax = AttackMaxWarrior;
            ReproductionPerc = ReproductionPercWarrior;
            AttackFrequency = AttackFrequencyWarrior;
        }
        else
        {
            Radius = RadiusFarmer;
            RayMaxDistance = RayMaxDistanceFarmer;
            StorageCapacity = StorageCapacityFarmer;
            HpMin = HpMinFarmer;
            HpMax = HpMaxFarmer;
            SpeedMin = SpeedMinFarmer;
            SpeedMax = SpeedMaxFarmer;
            AttackMin = AttackMinFarmer;
            AttackMax = AttackMaxFarmer;
            ReproductionPerc = ReproductionPercFarmer;
            AttackFrequency = AttackFrequencyFarmer;
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManagerScript.Instance.Pause)
        {
            CheckBonusHealth();
            if (AnimController != null)
            {
                AnimController.speed = 1;
            }

            //eat when starving
            if (Hp <= 0)
            {
                if (TargetFood != null)
                {
                    if (TargetFood.Food > 0)
                    {
                        TargetFood.ReleaseSlot();
                    }
                    TargetFood = null;
                }
                if (AnimController != null)
                {
                    AnimController.speed = 1;
                    //AttackAnimation if there is an animator
                    AnimController.GetComponent<TribeColorScript>().TribeColor = SkinManager.Instance.GetSkinInfo(HouseType).TribeColor;
                    AnimController.transform.SetParent(GameManagerScript.Instance.DeadContainer);
                    GameManagerScript.Instance.DeadList.Add(AnimController);
                    AnimController.SetInteger("UIState", 3);
                }
                GameManagerScript.Instance.HumanBeingDied();
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
            //hate management
            /*HType = Charity > Hate && Charity > Gratitude ? HumanType.Charity :
                     Hate > Charity && Hate > Gratitude ? HumanType.Hate :
                     Gratitude > Hate && Charity < Gratitude ? HumanType.Gratitude : HumanType.None;*/
            //Debug.Log(Vector3.Distance(transform.position, TargetDest) + "   " + name);


            //get the time required to go home
            //TimeToGoBack = (PathFinderManager.Instance.DistanceToKeyPoint(transform.position, PathFinderManager.Instance.GetHousePos(HouseType))) / (Speed) / 10;
            //linear distance
            TimeToGoBack = (Vector3.Distance(transform.position, TargetHouse.transform.position)) / (Speed) / 10;
            //going back home if the safety time is finished
            //BreakPoint
            if (!TargetHouse.IsPlayer && GameManagerScript.Instance.DayTime - GameManagerScript.Instance.currentDayTime >= GameManagerScript.Instance.DayTime - (TimeToGoBack + TargetHouse.SafetyTimer) && (CurrentState != StateType.Home /*&& CurrentAction != ActionState.Fight*/ && CurrentState != StateType.ComingBackHome))
            {
                FollowCo = null;
                CanIgetFood = false;
                CurrentState = StateType.ComingBackHome;
                //PathToHome = PathFinderManager.Instance.GetPathToDestination(transform.position, PathFinderManager.Instance.GetHousePos(HouseType));
                //GoBackHome(true);
                GoToPosition(TargetHouse.transform.position);
                //update HP Bar
                HPBar.gameObject.SetActive(false);
                HarvestBar.gameObject.SetActive(false);
                CultivationField.gameObject.SetActive(false);

            }
            //BreakPoint
            if (TargetHouse.IsPlayer && GameManagerScript.Instance.DayTime - GameManagerScript.Instance.currentDayTime >= GameManagerScript.Instance.DayTime - (TimeToGoBack + SafetyTime) && (CurrentState != StateType.Home /*&& CurrentAction != ActionState.Fight*/ && CurrentState != StateType.ComingBackHome))
            {
                FollowCo = null;
                CanIgetFood = false;
                CurrentState = StateType.ComingBackHome;
                //PathToHome = PathFinderManager.Instance.GetPathToDestination(transform.position, PathFinderManager.Instance.GetHousePos(HouseType));

                //GoBackHome(true);
                GoToPosition(TargetHouse.transform.position);
                //update HP Bar
                HPBar.gameObject.SetActive(false);
                HarvestBar.gameObject.SetActive(false);
                CultivationField.gameObject.SetActive(false);

            }

            //lose health when outside the house
            //HealthDecreasingOutside();

        }
        else
        {
            if (AnimController != null)
            {
                AnimController.speed = 0;
            }
        }

    }


    private void InitializeRandomParameters()
    {


        SafetyTime = UnityEngine.Random.Range(0f, 10f);
        Speed = UnityEngine.Random.Range(SpeedMin, SpeedMax) / 10;
        Hp = UnityEngine.Random.Range(HpMin, HpMax);
        Attack = UnityEngine.Random.Range(Attack, Attack);
        InitialHP = Hp;
        BaseHp = Hp;
    }
    internal void WearSkin()
    {
        //set class and instantiate the standard skin
        //HumanJob = HumanClass.Standard;
        StandardSkin = Instantiate(SkinManager.Instance.GetSkinFromHouse(HouseType), transform);
        if (StandardSkin.GetComponentInChildren<Animator>())
        {
            AnimController = StandardSkin.GetComponentInChildren<Animator>();
            AnimController.SetInteger("UIState", 0);
        }
        ChangeClass(HumanJob);

    }
    public void ChangeClass(HumanClass h)
    {
        //set class and deactivate skins
        HumanJob = h;
        StandardSkin.gameObject.SetActive(false);
        if (HarvesterSkin != null)
            HarvesterSkin.gameObject.SetActive(false);
        if (WarriorSkin != null)

            WarriorSkin.gameObject.SetActive(false);
        //wear skin selected
        switch (h)
        {
            case HumanClass.Standard:
                StandardSkin.gameObject.SetActive(true);
                if (StandardSkin.GetComponentInChildren<Animator>())
                {
                    AnimController = StandardSkin.GetComponentInChildren<Animator>();
                    AnimController.SetInteger("UIState", 0);
                }
                break;
            case HumanClass.Harvester:
                if (HarvesterSkin == null)
                { HarvesterSkin = Instantiate(SkinManager.Instance.GetSkinFromHouse(HouseType, h), transform); }
                HarvesterSkin.gameObject.SetActive(true);
                if (HarvesterSkin.GetComponentInChildren<Animator>())
                {
                    AnimController = HarvesterSkin.GetComponentInChildren<Animator>();
                    AnimController.SetInteger("UIState", 0);
                }
                break;
            case HumanClass.Warrior:
                if (WarriorSkin == null)
                { WarriorSkin = Instantiate(SkinManager.Instance.GetSkinFromHouse(HouseType, h), transform); }
                WarriorSkin.gameObject.SetActive(true);
                if (WarriorSkin.GetComponentInChildren<Animator>())
                {
                    AnimController = WarriorSkin.GetComponentInChildren<Animator>();
                    AnimController.SetInteger("UIState", 0);
                }
                break;
        }
    }
    private void SetLayers()
    {
        switch (HouseType)
        {
            case HousesTypes.Green:
                gameObject.layer = LayerMask.NameToLayer("North");
                EnemyLayer = LayerMask.GetMask("West", "South", "East", "Monster");
                break;
            case HousesTypes.Yellow:
                gameObject.layer = LayerMask.NameToLayer("South");
                EnemyLayer = LayerMask.GetMask("West", "North", "East", "Monster");
                break;
            case HousesTypes.Red:
                gameObject.layer = LayerMask.NameToLayer("East");
                EnemyLayer = LayerMask.GetMask("West", "South", "North", "Monster");
                break;
            case HousesTypes.Blue:
                gameObject.layer = LayerMask.NameToLayer("West");
                EnemyLayer = LayerMask.GetMask("North", "South", "East", "Monster");
                break;
        }
    }



    private void HealthDecreasingOutside()
    {
        if (CurrentState != StateType.Home)
        {
            Hp -= Hunger * Time.deltaTime;
        }
    }

    public void GoToRandomPos()
    {
        if (gameObject.activeInHierarchy)
        {
            //all map
            TargetDest = GameManagerScript.Instance.GetFreeSpaceOnGround(0);
            TargetDest = GetFreeSpaceOnGround();
            GoToPosition(TargetDest);
        }
    }
    public void GoBackHome(bool Initialize)
    {
        if (gameObject.activeInHierarchy)
        {
            if (Initialize)
            {
                PathIndex = 0;
            }
            else
            {
                PathIndex++;
            }
            TargetDest = PathToHome[PathIndex];
            GoToPosition(TargetDest);
        }
    }
    public Vector3 GetFreeSpaceOnGround()
    {
        Vector3 res = new Vector3(UnityEngine.Random.Range(transform.position.x - RayMaxDistance, transform.position.x + RayMaxDistance), transform.position.y, UnityEngine.Random.Range(transform.position.z - RayMaxDistance, transform.position.z + RayMaxDistance));
        /*
                LayerMask layerMask = LayerMask.GetMask("ObstacleItem");
                List<RaycastHit> ElementHitted = new List<RaycastHit>();
                ElementHitted = Physics.RaycastAll(transform.position, res-transform.position,Vector3.Distance(transform.position, res),layerMask).ToList<RaycastHit>();
                Debug.DrawLine(transform.position, res, Color.white, 1);
                for (int i = 0; i<1000; i++)
                {
                    if(ElementHitted.Count > 0)
                    {
                        res = new Vector3(UnityEngine.Random.Range(transform.position.x - RayMaxDistance, transform.position.x + RayMaxDistance), transform.position.y, UnityEngine.Random.Range(transform.position.z - RayMaxDistance, transform.position.z + RayMaxDistance));
                        ElementHitted = Physics.RaycastAll(transform.position, res - transform.position, Vector3.Distance(transform.position, res), layerMask).ToList<RaycastHit>();
                        Debug.DrawLine(transform.position, res, Color.white, 1);
                    }
                    else
                    {
                        break;
                    }
                }
                */
        if (res.x < -GameManagerScript.Instance.GroundSizeWidth)
        {
            res.x = -GameManagerScript.Instance.GroundSizeWidth;
        }
        else if (res.x > GameManagerScript.Instance.GroundSizeWidth)
        {
            res.x = GameManagerScript.Instance.GroundSizeWidth;
        }
        if (res.z < -GameManagerScript.Instance.GroundSizeWidth)
        {
            res.z = -GameManagerScript.Instance.GroundSizeWidth;
        }
        else if (res.z > GameManagerScript.Instance.GroundSizeWidth)
        {
            res.z = GameManagerScript.Instance.GroundSizeWidth;
        }
        return res;
    }
    public void MineFood(FoodScript food)
    {
        if (gameObject.activeInHierarchy && food.AskSlot())
        {
            StopAllCoroutines();
            StartCoroutine(Harvest(food));
        }
        else
        {
            CurrentState = StateType.LookingForFood;
            GoToRandomPos();
        }
    }

    internal void Cultivate()
    {
        if (gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(CultivateCo());
        }

        /*human.CurrentState = StateType.FollowInstruction;
                        human.TargetFoodDest = null;
                        human.TargetHuman = null;
                        human.TargetDest = transform;
                        human.GoToPosition(transform);*/
    }
    public void GoToPosition(Vector3 nextPos)
    {

        if (gameObject.activeInHierarchy)
        {
            HarvestBar.gameObject.SetActive(false);

            if (FoodLife > 0)
            {

                FoodLife = 0;
                if (Specialization > 0.1f)
                {
                }
                else
                {
                    Specialization = 0.1f;
                }
                CheckBonusHealth();
                CheckHumanJob();
            }

            //to do implement a proper coroutine managing
            IsStarted = true;
            CultivationField.gameObject.SetActive(false);
            MoveCo = Move(new Vector3(nextPos.x, 0, nextPos.z));
            StopAllCoroutines();
            StartCoroutine(Move(new Vector3(nextPos.x, 0, nextPos.z)));
            if (AnimController != null)
            {
                //Walk animation if there is an animator
                AnimController.SetInteger("UIState", 0);
            }
        }
    }


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
        //if breeding is abilitated for the player
        if (GameManagerScript.Instance.Breeding == true || TargetHouse.IsPlayer == false)
        {
            if (UnityEngine.Random.Range(0, 100) < ReproductionPerc)
            {
                GameManagerScript.Instance.Reproduction(TargetHouse, HumanJob);
            }
        }

    }

    private List<RaycastHit> LookAround(string layer)
    {
        LayerMask layerMask = LayerMask.GetMask(layer);
        List<RaycastHit> ElementHitted = new List<RaycastHit>();


        ElementHitted = Physics.SphereCastAll(transform.position, Radius, transform.forward, Radius, layerMask).ToList<RaycastHit>();
        if (layerMask == LayerMask.GetMask("Food"))
        {
            ElementHitted = ElementHitted.Where(r => r.collider.GetComponent<FoodScript>().Slots > 0).ToList();
        }
        return ElementHitted;
    }


    private List<RaycastHit> LookAroundForEnemies()
    {

        List<RaycastHit> Enemy = new List<RaycastHit>();

        Enemy = Physics.SphereCastAll(transform.position, Radius, transform.forward, Radius, EnemyLayer).ToList<RaycastHit>();
        //List < RaycastHit > humans = Enemy.ToArray< RaycastHit>().Where(a => (a.collider.GetComponent<HumanBeingScript>()&&)).ToList();
        List<RaycastHit> EnemyNotInHome = new List<RaycastHit>();
        foreach (RaycastHit r in Enemy)
        {
            if (r.collider.GetComponent<HumanBeingScript>())
            {
                if (r.collider.GetComponent<HumanBeingScript>().CurrentState != StateType.Home)
                {
                    EnemyNotInHome.Add(r);
                }
            }
            else
            {
                EnemyNotInHome.Add(r);
            }
        }
        Enemy = EnemyNotInHome;

        return Enemy;
    }

    private void CheckSurroundings()
    {
        /*
        //checking collisions and grouping them by category
        List<RaycastHit> collisions = LookAround("Food");
        List<RaycastHit> Food = collisions.Where(a => a.collider.tag == "Food").ToList();
        List<RaycastHit> Enemy = collisions.Where(a => a.collider.tag == "Human" && a.collider.GetComponent<HumanBeingScript>().HouseType != HouseType).ToList();
        List<RaycastHit> Allies = collisions.Where(a => a.collider.tag == "Human" && a.collider.GetComponent<HumanBeingScript>().HouseType == HouseType).ToList();

        //check the nearest food resource
        */


    }



    void Instance_DayStarted()
    {
        OffsetTimer = Time.time;
        FoodStealed = false;
        CurrentState = StateType.LookingForFood;

        GoToRandomPos();

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





    private void InvisibilityIfHouse(List<RaycastHit> Housecollisions)
    {
        if (Housecollisions.Count > 0)
        {
            if (Vector3.Distance(Housecollisions[0].collider.ClosestPoint(transform.position), transform.position) < TransparentDistanceFromHome)
            {
                if (AnimController != null)
                {
                    AnimController.gameObject.SetActive(false);
                }
            }
            else
            {
                if (AnimController != null)
                {
                    AnimController.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (AnimController != null)
            {
                AnimController.gameObject.SetActive(true);
            }
        }
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
                RandomVector = new Vector3(Random.Range(-AttackDistance, AttackDistance), 0, Random.Range(-AttackDistance, AttackDistance));
                AttackEnemy(TargetHuman);
            }
        }
    }

    public void AttackEnemy(Transform humanT)
    {
        if (/*FollowCo == null &&*/ gameObject.activeInHierarchy)
        {
            CurrentState = StateType.LookingForFood;
            StopAllCoroutines();
            MoveCo = null;

            FollowCo = FollowEnemy(humanT);
            StartCoroutine(FollowCo);
        }
        else
        {
            FollowCo = null;
            MoveCo = null;
            GoToRandomPos();
        }
    }

    public void UnderAttack(float damage)
    {            //BreakPoint
        //update HP Bar
        HPBar.gameObject.SetActive(true);
        HPBar.UpdateHP(Hp, BaseHp, HouseType);
        RandomVector = new Vector3();
        Hp -= damage;
        List<RaycastHit> EnemiesCollision = LookAroundForEnemies();
        HarvestBar.gameObject.SetActive(false);
        //Counterattack
        if (TargetFood != null)
        {
            if (TargetFood.Food > 0)
            {
                TargetFood.ReleaseSlot();
            }
            TargetFood = null;
        }
        if ((CurrentState != StateType.FollowInstruction || WaitingForOthers) && CurrentState != StateType.ComingBackHome && HumanJob == HumanClass.Warrior)
        {
            if (Food >= StorageCapacity)
            {
                CurrentState = StateType.ComingBackHome;
                DeliverFood = true;
                GoToPosition(TargetHouse.transform.position);
                //PathToHome = PathFinderManager.Instance.GetPathToDestination(transform.position, PathFinderManager.Instance.GetHousePos(HouseType));
                //GoBackHome(true);
            }
            else
            if (!TargetHouse.IsPlayer && GameManagerScript.Instance.DayTime - GameManagerScript.Instance.currentDayTime < GameManagerScript.Instance.DayTime - (TimeToGoBack + TargetHouse.SafetyTimer) && EnemiesCollision.Count > 0)
            {
                AttackEnemy(EnemiesCollision[0].collider.transform);
            }
            else if (TargetHouse.IsPlayer && GameManagerScript.Instance.DayTime - GameManagerScript.Instance.currentDayTime < GameManagerScript.Instance.DayTime - (TimeToGoBack + SafetyTime) && EnemiesCollision.Count > 0)
            {
                AttackEnemy(EnemiesCollision[0].collider.transform);
            }

        }


    }
    /// <summary>
    /// Move to a specified dest.
    /// using the old system
    /// </summary>
    /// <returns>The move.</returns>
    /// <param name="dest">Destination.</param>
    //private int currentID = 0;
    public IEnumerator Move(Vector3 dest)
    {
        yield return new WaitForEndOfFrame();
        Vector3 offset = transform.position;
        float distance = Vector3.Distance(offset, dest);
        //int iD = currentID;
        if (CurrentState != StateType.FoodFound)
        {
            TargetFoodDest = null;
        }

        TargetHuman = null;

        float timeCount = 0;
        List<RaycastHit> EnemiesCollision = new List<RaycastHit>();
        List<RaycastHit> Foodcollisions = new List<RaycastHit>();
        if (HumanJob == HumanClass.Warrior)
            EnemiesCollision = LookAroundForEnemies();
        else
            Foodcollisions = LookAround("Food");
        List<RaycastHit> Housecollisions = LookAround("House");
        while (timeCount < 1)
        {
            if (HumanJob == HumanClass.Warrior)
                EnemiesCollision = LookAroundForEnemies();
            else
                Foodcollisions = LookAround("Food");
            Housecollisions = LookAround("House");
            Foodcollisions = Foodcollisions.Where(r => r.collider.GetComponent<FoodScript>().Slots > 0).ToList();
            InvisibilityIfHouse(Housecollisions);
            if (CurrentState != StateType.FollowInstruction && CurrentState != StateType.ComingBackHome)
            {
                if (CurrentState == StateType.LookingForFood && (GameManagerScript.Instance.AttackIsEnable || !TargetHouse.IsPlayer) && EnemiesCollision.Count > 0 && Foodcollisions.Count > 0)
                {
                    break;
                }
                //found enemy house
                if (!FoodStealed && CurrentState == StateType.LookingForFood && TargetFoodDest == null && TargetHuman == null && Housecollisions.Count > 0 && Housecollisions[0].collider.GetComponent<HouseScript>().FoodStore > 0 && Housecollisions[0].collider.GetComponent<HouseScript>().HouseType != HouseType)
                {
                    break;
                }
                //found enemy

                if ((GameManagerScript.Instance.AttackIsEnable || !TargetHouse.IsPlayer) && EnemiesCollision.Count > 0)
                {
                    break;
                }
                //found food
                if (CurrentState == StateType.LookingForFood && Foodcollisions.Count > 0)
                {
                    if (Foodcollisions[0].collider.GetComponent<FoodScript>().Slots > 0)
                    {
                        break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
            Vector3 nextpos = Vector3.Lerp(offset, dest, timeCount);
            transform.position = nextpos;
            if (!GameManagerScript.Instance.Pause)
                timeCount = timeCount + Time.deltaTime * Speed / distance * 10;
            if (CurrentState == StateType.FollowInstruction && timeCount >= 1)
            {
                WaitingForOthers = true;
                yield return new WaitForSeconds(TargetHouse.TimetoReachInstruction - TimetoReachInstruction);
            }
            WaitingForOthers = false;
        }
        if (HumanJob == HumanClass.Warrior)
            EnemiesCollision = LookAroundForEnemies();
        else
            Foodcollisions = LookAround("Food");
        Housecollisions = LookAround("House");
        InvisibilityIfHouse(Housecollisions);
        if (timeCount < 1)
        {

            if (CurrentState != StateType.FollowInstruction && CurrentState == StateType.LookingForFood && (GameManagerScript.Instance.AttackIsEnable || !TargetHouse.IsPlayer) && EnemiesCollision.Count > 0 && Foodcollisions.Count > 0)
            {
                AttackOrFood(EnemiesCollision, Foodcollisions);
            }
            else
            //found enemy house
            if (CurrentState != StateType.FollowInstruction && !FoodStealed && CurrentState == StateType.LookingForFood && TargetFoodDest == null && TargetHuman == null && Housecollisions.Count > 0 && Housecollisions[0].collider.GetComponent<HouseScript>().FoodStore > 0 && Housecollisions[0].collider.GetComponent<HouseScript>().HouseType != HouseType)
            {
                TargetFoodDest = Housecollisions[0].collider.transform;
                CurrentState = StateType.FoodFound;
                GoToPosition(TargetFoodDest.position);
                AttackAction();
            }
            else
            //found enemy

            if ((GameManagerScript.Instance.AttackIsEnable || !TargetHouse.IsPlayer) && CurrentState != StateType.FollowInstruction && CurrentState != StateType.ComingBackHome && EnemiesCollision.Count > 0)
            {
                TargetHuman = EnemiesCollision[0].collider.transform;
                FollowCo = null;
                CurrentState = StateType.LookingForFood;
                RandomVector = new Vector3(Random.Range(-AttackDistance, AttackDistance), 0, Random.Range(-AttackDistance, AttackDistance));
                AttackEnemy(TargetHuman);
            }
            else
            //found food
            if (CurrentState != StateType.FollowInstruction && CurrentState == StateType.LookingForFood && Foodcollisions.Count > 0 /*&& /*TargetFoodDest == null /*&& TargetHuman == null*/)
            {
                TargetFoodDest = Foodcollisions[0].collider.transform;
                CurrentState = StateType.FoodFound;
                Vector3 randomPos = TargetFoodDest.position + new Vector3(UnityEngine.Random.Range(-FoodRandomPointDistance, FoodRandomPointDistance), 0, UnityEngine.Random.Range(-FoodRandomPointDistance, FoodRandomPointDistance));
                GoToPosition(randomPos);
            }
            else
            {
                CurrentState = StateType.LookingForFood;
                GoToRandomPos();
            }
        }
        else
        {
            if (CurrentState != StateType.FollowInstruction && CurrentState == StateType.LookingForFood && (GameManagerScript.Instance.AttackIsEnable || !TargetHouse.IsPlayer) && EnemiesCollision.Count > 0 && Foodcollisions.Count > 0)
            {
                AttackOrFood(EnemiesCollision, Foodcollisions);
            }
            else
            if (CurrentState == StateType.FollowInstruction)
            {
                if (EnemiesCollision.Count > 0 && (GameManagerScript.Instance.AttackIsEnable || !TargetHouse.IsPlayer))
                {
                    TargetHuman = EnemiesCollision[0].collider.transform;
                    FollowCo = null;
                    CurrentState = StateType.LookingForFood;
                    RandomVector = new Vector3(Random.Range(-AttackDistance, AttackDistance), 0, Random.Range(-AttackDistance, AttackDistance));
                    AttackEnemy(TargetHuman);
                }
                else if (Foodcollisions.Count > 0)
                {
                    TargetFoodDest = Foodcollisions[0].collider.transform;
                    CurrentState = StateType.FoodFound;
                    Vector3 randomPos = TargetFoodDest.position + new Vector3(UnityEngine.Random.Range(-FoodRandomPointDistance, FoodRandomPointDistance), 0, UnityEngine.Random.Range(-FoodRandomPointDistance, FoodRandomPointDistance));
                    GoToPosition(randomPos);
                }
                else
                {
                    CurrentState = StateType.LookingForFood;
                    GoToRandomPos();
                }

            }
            else
            if (PathIndex < PathToHome.Count - 1 && CurrentState == StateType.ComingBackHome)
            {
                GoBackHome(false);
            }
            else
            if (/*PathIndex == PathToHome.Count - 1 &&*/ CurrentState == StateType.ComingBackHome && !DeliverFood && GameManagerScript.Instance.DayTime - GameManagerScript.Instance.currentDayTime >= GameManagerScript.Instance.DayTime - (TimeToGoBack + SafetyTime))
            {
                TargetHouse.FoodStore += Food;
                UIManagerScript.Instance.UpdateFood();
                Food = 0;
                CanIgetFood = true;
                CurrentState = StateType.Home;
                HomeSweetHome();
            }
            else if (/*PathIndex == PathToHome.Count - 1 &&*/ CurrentState == StateType.ComingBackHome && DeliverFood)
            {
                TargetHouse.FoodStore += Food;
                UIManagerScript.Instance.UpdateFood();
                Food = 0;
                CanIgetFood = true;
                CurrentState = StateType.LookingForFood;
                DeliverFood = false;
                GoToRandomPos();
            }
            else if (CurrentState == StateType.LookingForFood || CurrentState == StateType.FollowInstruction)
            {
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
                        CurrentState = StateType.ComingBackHome;
                        DeliverFood = true;
                        GoToPosition(TargetHouse.transform.position);
                        //PathToHome = PathFinderManager.Instance.GetPathToDestination(transform.position, PathFinderManager.Instance.GetHousePos(HouseType));
                        //GoBackHome(true);
                    }

                    CurrentState = StateType.LookingForFood;
                    GoToRandomPos();
                }
                else
                {
                    if (TargetFoodDest.gameObject.activeInHierarchy)
                    {
                        DidIFindFood = true;
                        MineFood(TargetFoodDest.GetComponent<FoodScript>());
                    }
                    else
                    {
                        CurrentState = StateType.LookingForFood;
                        GoToRandomPos();
                    }
                }
            }
            else
            {
                CurrentState = StateType.LookingForFood;
                GoToRandomPos();
                MoveCo = null;
            }
        }


    }
    private IEnumerator Harvest(FoodScript food)
    {
        yield return new WaitForEndOfFrame();
        bool ResourceAvaiable = true;
        if (FoodLife <= 0)
        {
            FoodLife = food.Hardness;
        }
        TargetFood = food;

        while (ResourceAvaiable)
        {
            yield return new WaitForEndOfFrame();
            //move towars target
            List<RaycastHit> Housecollisions = LookAround("House");
            InvisibilityIfHouse(Housecollisions);
            while (FoodLife > 0)
            {
                HarvestBar.gameObject.SetActive(true);
                HarvestBar.UpdateHarvest(FoodLife, food.Hardness, HouseType);
                if (!GameManagerScript.Instance.Pause)
                    FoodLife -= HarvestAttack;
                if (AnimController != null)
                {
                    //AttackAnimation if there is an animator
                    AnimController.SetInteger("UIState", 1);
                }
                if (!food.isActiveAndEnabled)
                {
                    break;
                }
                yield return new WaitForSeconds(AttackFrequency);
            }
            if (Specialization > 0.1f)
            {
                Specialization -= HarvestIncrement;
            }
            else
            {
                Specialization = 0.1f;
            }
            CheckBonusHealth();
            CheckHumanJob();
            //Attack
            if (FoodLife <= 0 && food.Food > 0)
            {
                FoodLife = food.Hardness;
                Food += food.TakeFood(1);
                ResourceAvaiable = food.Food > 0;

            }
            if (Food >= StorageCapacity)
            {
                break;
            }
            if (!ResourceAvaiable)
            {
                break;
            }
            if (TargetFood != null)
            {
                food.ReleaseSlot();

            }
            ResourceAvaiable = food.Food > 0;
            yield return new WaitForEndOfFrame();
        }
        if (CurrentState == StateType.FollowInstruction || CurrentState == StateType.FollowInstruction)
        {
            food.ReleaseSlot();
        }
        else
        if (Food >= StorageCapacity)
        {
            CurrentState = StateType.ComingBackHome;
            DeliverFood = true;
            //PathToHome = PathFinderManager.Instance.GetPathToDestination(transform.position, PathFinderManager.Instance.GetHousePos(HouseType));
            //GoBackHome(true);
            GoToPosition(TargetHouse.transform.position);
        }
        else
        if (!ResourceAvaiable)
        {
            DidIFindFood = true;
            food.Deactivate(); ;
            CurrentState = StateType.LookingForFood;
            GoToRandomPos();
        }
        else
        {
            HarvestBar.gameObject.SetActive(false);
            CurrentState = StateType.LookingForFood;
            GoToRandomPos();
            FollowCo = null;
        }
    }
    private IEnumerator CultivateCo()
    {
        yield return new WaitForEndOfFrame();
        List<RaycastHit> Housecollisions = LookAround("House");
        InvisibilityIfHouse(Housecollisions);
        CurrentState = StateType.Cultivating;
        CultivationField.gameObject.SetActive(true);
        CultivationProgress = 0;
        while (CurrentState == StateType.Cultivating)
        {
            yield return new WaitForEndOfFrame();
            if (Food >= StorageCapacity)
            {
                break;
            }
            //move towars target
            while (CultivationProgress < CultivationTarget * Specialization * 2)
            {
                HarvestBar.gameObject.SetActive(true);
                HarvestBar.UpdateHarvest(CultivationProgress, CultivationTarget * Specialization * 2, HouseType);
                if (!GameManagerScript.Instance.Pause)
                    CultivationProgress += HarvestAttack;
                if (AnimController != null)
                {
                    //AttackAnimation if there is an animator
                    AnimController.SetInteger("UIState", 1);
                }
                yield return new WaitForSeconds(AttackFrequency);
            }
            if (Specialization > 0.1f)
            {
                Specialization -= HarvestIncrement;

            }
            else
            {
                Specialization = 0.1f;
            }
            CheckBonusHealth();
            CheckHumanJob();
            CultivationProgress = 0;
            RandomFoodGained = UnityEngine.Random.Range(0f, CultivationPercentage[CultivationPercentage.Count - 1] + 1);
            for (int i = 0; i < CultivationPercentage.Count; i++)
            {
                if (RandomFoodGained < CultivationPercentage[i])
                {
                    Food += i;
                    break;
                }

            }
            yield return new WaitForEndOfFrame();
        }
        if (Food >= StorageCapacity)
        {
            TargetHouse.FoodStore += Food;
            UIManagerScript.Instance.UpdateFood();
            Food = 0;/*
            CurrentState = StateType.ComingBackHome;
            DeliverFood = true;
            GoToPosition(TargetHouse.transform.position);*/
        }

    }
    private IEnumerator FollowEnemy(Transform humanT)
    {
        yield return new WaitForEndOfFrame();
        Vector3 RandomAttackPos = humanT.position + RandomVector;
        if (RandomVector == new Vector3())
        {
            RandomAttackPos = transform.position;
        }
        bool EnemyAlive = true;
        Vector3 offset = transform.position;
        float distance = Vector3.Distance(offset, RandomAttackPos);
        float timeCount = 0;
        bool humanEnemy = true;

        HumanBeingScript Enemy = null;
        MonsterScript EnemyMonster = null;
        if (humanT.GetComponent<HumanBeingScript>())
        {
            humanEnemy = true;
            Enemy = humanT.GetComponent<HumanBeingScript>();
        }
        else if (humanT.GetComponent<MonsterScript>())
        {
            humanEnemy = false;
            EnemyMonster = humanT.GetComponent<MonsterScript>();
        }
        float Dist = 0;
        while (EnemyAlive)
        {
            // yield return new WaitUntil(() => !GameManagerScript.Instance.Pause);

            List<RaycastHit> Housecollisions = LookAround("House");
            InvisibilityIfHouse(Housecollisions);


            //stop game
            //move towars target
            RandomAttackPos = humanT.position + RandomVector;
            if (RandomVector == new Vector3())
            {
                RandomAttackPos = transform.position;
            }
            if (humanEnemy)
            {
                Enemy.UnderAttack(0);
                if (Enemy.CurrentState == StateType.Home)
                {
                    break;
                }
            }
            else
            {
                EnemyMonster.UnderAttack(0);

            }
            distance = Vector3.Distance(transform.position, RandomAttackPos);
            while (distance >= Time.deltaTime * Speed / distance * 10)
            {
                if (Enemy == null && EnemyMonster == null)
                {
                    break;
                }
                //offset = transform.position;
                timeCount = 0;
                yield return new WaitForEndOfFrame();
                timeCount = timeCount + Time.deltaTime * Speed / distance * 10;
                RandomAttackPos = humanT.position + RandomVector;
                if (RandomVector == new Vector3())
                {
                    RandomAttackPos = transform.position;
                }
                Vector3 nextpos = Vector3.Lerp(transform.position, RandomAttackPos, timeCount);
                if (!GameManagerScript.Instance.Pause)
                    transform.position = nextpos;
                distance = Vector3.Distance(transform.position, RandomAttackPos);
                RandomAttackPos = GameManagerScript.Instance.IsInsidePlayground(nextpos);

            }
            if (Enemy == null && EnemyMonster == null)
            {
                break;
            }
            bool pause = GameManagerScript.Instance.Pause;
            //start another coroutine, not compatible with current system
            //GoToPosition(humanT.position);
            if (humanT != null)
            {
                RandomAttackPos = humanT.position + RandomVector;
                if (RandomVector == new Vector3())
                {
                    RandomAttackPos = transform.position;
                }
                Dist = Vector3.Distance(transform.position, RandomAttackPos);
                //Attack
                if (Dist <= Time.deltaTime * Speed / distance * 10 && Dist <= AttackDistance && CanIAttack)
                {
                    //update HP Bar
                    HPBar.gameObject.SetActive(true);
                    HPBar.UpdateHP(Hp, BaseHp, HouseType);

                    CanIAttack = false;
                    Invoke("AttackAction", AttackFrequency);
                    if (humanEnemy)
                    {
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
                            EnemyAlive = false;
                            HPBar.gameObject.SetActive(false);
                            if (Specialization < 0.9f)
                            {
                                Specialization += WarriorIncrement;

                            }
                            else
                            {
                                Specialization = 0.9f;
                            }
                            CheckHumanJob();
                            CheckBonusHealth();
                            //update HP Bar
                            break;
                        }
                    }
                    else
                    {
                        EnemyMonster.UnderAttack(PhisicalAttack);
                        if (AnimController != null)
                        {
                            //AttackAnimation if there is an animator
                            AnimController.SetInteger("UIState", 2);
                        }
                        if (EnemyMonster.Hp <= 0)
                        {
                            //Food += EnemyMonster.Food;
                            //EnemyMonster.Food = 0;
                            HPBar.gameObject.SetActive(false);
                            if (EnemyMonster.HouseHuman == null && EnemyMonster.Alive)
                            {
                                GameManagerScript.Instance.AddGuardian(HouseType);
                            }
                            EnemyMonster.Alive = false;
                            EnemyAlive = false;
                            if (Specialization < 0.9f)
                            {
                                Specialization += WarriorIncrement;

                            }
                            else
                            {
                                Specialization = 0.9f;
                            }
                            CheckHumanJob();
                            CheckBonusHealth();
                            //update HP Bar

                        }
                    }
                }
                RandomAttackPos = humanT.position + RandomVector;
                if (RandomVector == new Vector3())
                {
                    RandomAttackPos = transform.position;
                }
                Dist = Vector3.Distance(transform.position, RandomAttackPos);
                if (Dist > AttackDistance && humanT != null)
                {
                    break;
                }
                else
                if (humanT == null)
                {
                    break;
                }
                //won?
                /*if (!EnemyAlive)
                {
                    if (Specialization < 0.9f)
                    {
                        Specialization += WarriorIncrement;

                    }
                    else
                    {
                        Specialization = 0.9f;
                    }
                    CheckHumanJob();
                    CheckBonusHealth();
                    //update HP Bar

                }*/
                yield return new WaitForEndOfFrame();
            }
        }
        if (Food >= StorageCapacity)
        {
            CurrentState = StateType.ComingBackHome;
            DeliverFood = true;
            GoToPosition(TargetHouse.transform.position);
        }
        else
        if (Enemy == null && EnemyMonster == null)
        {
            CurrentState = StateType.LookingForFood;
            FollowCo = null;
            GoToRandomPos();
            EnemyAlive = false;
        }
        else
            if (Dist > AttackDistance && humanT != null)
        {
            FollowCo = null;
            RandomAttackPos = humanT.position + RandomVector;
            if (RandomVector == new Vector3())
            {
                RandomAttackPos = transform.position;
            }
            RandomAttackPos = humanT.position + new Vector3(Random.Range(-AttackDistance, AttackDistance), 0, Random.Range(-AttackDistance, AttackDistance));
            AttackEnemy(humanT.transform);
        }
        else
                if (humanT == null)
        {
            FollowCo = null;
            CurrentState = StateType.LookingForFood;
            GoToRandomPos();
        }
        else
        {
            CurrentState = StateType.LookingForFood;
            FollowCo = null;
            GoToRandomPos();
        }

    }

    private void CheckHumanJob()
    {
        if (Specialization <= HarvesterValue)
        {
            ChangeClass(HumanClass.Harvester);
        }
        else if (Specialization >= WarriorValue)
        {
            ChangeClass(HumanClass.Warrior);
        }
        else
        {
            ChangeClass(HumanClass.Standard);
        }
    }

    private void CheckBonusHealth()
    {
        if (Math.Abs((Specialization * 100 - 50) / 2) + InitialHP > BaseHp)
        {
            BaseHp = Math.Abs((Specialization * 100 - 50) / 2) + InitialHP;
            HPBonus = Mathf.Abs(Specialization * 100 - 50) / 2;
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
    FollowInstruction = 4,
    Cultivating = 5
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
public enum HumanClass
{
    Standard,
    Harvester,
    Warrior
}
