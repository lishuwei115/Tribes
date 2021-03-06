﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public bool Pause = false;
    public Transform HousePrefab;
    public Transform HousesHolder;
    public Transform MapBorder;
    public HumanClass HumanSelected = HumanClass.Harvester;
    public GameState StateOfGame = GameState.Playing;
    public delegate void StartDay();
    public event StartDay DayStarted;
    public HousesTypes PlayerHouse = HousesTypes.Red;
    HousesTypes PlayerHouseChake = HousesTypes.Red;
    public int enemyDefeated = 0;
    public float FoodPlayer = 0;
    public Mesh HumanMesh;
    //public Mesh FoodMesh;
    public Material FoodMaterial;
    public BlockInput[] UIButtons;
    public int GuardiansSummonable = 0;

    public bool UIButtonOver;
    public static GameManagerScript Instance;
    public GameStateType GameStatus = GameStateType.Intro;
    [Header("Harvest/warrior increments")]
    public int FoodRequiredHarvester = 1;
    public int FoodRequiredWarrior = 5;
    [Tooltip("Set the random max value when directiong the tribe in one point")]
    [Range(0, 300)]
    public float FollowOrderRandomness = 10;
    [Range(0, 300)]
    public int DayTime = 30;

    [Range(0, 300)]
    public int DayLightTime = 60;

    [Range(1, 500)]
    public int Humans = 10;
    public AnimationCurve PopulationDistribution;

    //[Range(0, 500)]
    //public int NumberOfMonsters = 6;

    [Range(1, 500)]
    public int FoodPerDay = 10;

    public AnimationCurve FoodDistanceFromOrigin;
    public float FoodMinDistance = 1;
    public float FoodRaySpawn = 60;

    [Range(0, 10)]
    public int MaxNumChildren = 3;
    [Range(0, 1000)]
    public int MaxHumansForTribe = 100;
    [Header("Special Attack Parameters")]
    [Range(0, 1000)]
    public float RangeAttackDistance = 10;
    [Range(0, 1000)]
    public float RangeAttackDamage = 40;
    [Header("GameElements")]
    public List<HouseScript> Houses = new List<HouseScript>();
    public List<MonsterHouse> MonsterHouses;
    public MonsterScript MonsterPrefab;
    public List<MonsterScript> Monsters = new List<MonsterScript>();
    public List<MonsterScript> Guardians = new List<MonsterScript>();
    //public Transform MonsterContainer; TO DO
    public GameObject Human;

    public GameObject Food;
    public GameObject Flower;

    public Transform HumansContainer;



    public Transform FoodContainer;

    public Transform DeadContainer;

    [Header("info")]
    public int HumansAtHome = 0;

    //Not visible in Inspector
    [HideInInspector]
    public List<HumanBeingScript> HumansList = new List<HumanBeingScript>();



    public List<Animator> DeadList = new List<Animator>();
    public List<Animator> FlowerList = new List<Animator>();


    [HideInInspector]
    public List<FoodScript> FoodsList = new List<FoodScript>();

    private IEnumerator DayTimeCoroutine;

    [Range(0, 1000)]
    public float GroundSizeWidth = 40;
    [Range(0, 1000)]
    public float GroundSizeHeight = 40;

    [HideInInspector]
    private int ReproducedLastDay;
    [HideInInspector]
    private int DiedLastDay;
    [HideInInspector]
    public int currentDayTime = 0;
    public float CurrentTimeMS = 0;
    public bool AddingPlayerHouse = false;
    public bool Breeding = true;
    public bool AttackIsEnable = true;
    public DestroyOverTime Pointer;
    public DestroyOverTime PointerBlue;
    private bool FoodRandomized;

    Vector3 RangeAttackPosition;

    public bool Worship = false;

    private void Awake()
    {
        Instance = this;
        MonsterHouses = UnityEngine.Object.FindObjectsOfType<MonsterHouse>().ToList<MonsterHouse>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;


    }

    // Use this for initialization
    void Start()
    {
        //StartGame();

        UIButtons = UnityEngine.Object.FindObjectsOfType<BlockInput>();

    }
    internal void Won()
    {
        UIManagerScript.Instance.WinLoseState(1);
        StateOfGame = GameState.Won;
    }
    internal void SpawnNewHouse(HousesTypes h, Vector3 position)
    {
        HouseScript house = Instantiate(HousePrefab, position, new Quaternion(0, 0, 0, 0), HousesHolder).GetComponent<HouseScript>();
        house.HouseType = h;
        house.IsPlayer = h == PlayerHouse ? true : false;
        Houses.Add(house);
        for (int i = 0; i < 2; i++)
        {
            GameObject human = Instantiate(Human, house.transform.position, Quaternion.identity, HumansContainer);
            HumanBeingScript hbs = human.GetComponent<HumanBeingScript>();
            human.GetComponent<MeshFilter>().sharedMesh = HumanMesh;
            HumansList.Add(hbs);
            hbs.Specialization = 0.1f;
            hbs.HumanJob = HumanClass.Harvester;
            hbs.HouseType = house.HouseType;
            hbs.TargetHouse = house;
            hbs.FinallyBackHome += Hbs_FinallyBackHome;
            hbs.TargetHouse = house.GetComponent<HouseScript>();
            hbs.WearSkin();
            //hbs.gameObject.tag = "" + house.tag;
            house.Humans.Add(hbs);
        }
        foreach (HouseScript ho in Houses)
        {
            if (ho.HouseType == PlayerHouse)
            {
                ho.CloseBuildingCircle(true);
                WorldmapCamera.Instance.FinishBuilding();

                AddingPlayerHouse = true;
            }
        }
    }
    public void CloseBuildMenu()
    {
        foreach (HouseScript ho in Houses)
        {
            if (ho.HouseType == PlayerHouse)
            {
                ho.CloseBuildingCircle(false);
            }
        }
    }
    internal bool ToogleBreeding()
    {
        Breeding = !Breeding;
        return Breeding;
    }

    internal bool ToogleAttack()
    {
        AttackIsEnable = !AttackIsEnable;
        return AttackIsEnable;
    }

    internal void EnemyDefeated()
    {
        if (StateOfGame != GameState.Lost && StateOfGame != GameState.Won)
        {
            enemyDefeated++;
            List<HouseScript> houses = Houses.Where(r => !r.IsPlayer && r.HumansAlive.Count > 0).ToList();
            if (houses.Count == 0)
            {
                Won();

            }


        }

    }
    /// <summary>
    /// make all human of the player to cultivate
    /// </summary>
    public void Cultivate()
    {
        List<HumanBeingScript> playerHumans = HumansList.Where(r => r.HouseType == PlayerHouse && r.isActiveAndEnabled).ToList();
        Worship = true;
        AudioManager.Instance.StartWorship();
        foreach (HouseScript house in Houses)
        {
            if (house.HouseType == PlayerHouse)
            {
                house.Cultivate(playerHumans);
            }
        }

    }
    public bool IsInBoundary(Vector3 mPos)
    {
        if (mPos.x < -GroundSizeWidth || mPos.x > GroundSizeWidth || mPos.z < -GroundSizeHeight || mPos.z > GroundSizeHeight)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    internal void Lost()
    {
        List<HouseScript> houses = Houses.Where(r => r.IsPlayer && r.HumansAlive.Count > 0).ToList();
        if (houses.Count == 0)
        {
            UIManagerScript.Instance.WinLoseState(2);
            StateOfGame = GameState.Lost;
        }

    }

    internal void MoveTribeTo(Vector3 pos, HousesTypes tribe)
    {
        foreach (HouseScript house in Houses)
        {
            if (house.HouseType == tribe)
            {
                house.MoveTribeTo(pos, FollowOrderRandomness);
            }
        }
        OpenMapBorder(.2f);
    }
    // Update is called once per frame
    void Update()
    {
        if (Worship)
        {
            if (HumansList.Where(h => h.CurrentState == StateType.Cultivating).ToList().Count <= 0)
            {
                AudioManager.Instance.StopWorship();
                Worship = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            SceneManager.LoadScene(0);
        }
        //UIManagerScript.Instance.InfoUpdate(HumansList.Where(r => r.gameObject.activeInHierarchy && r.HType == HumanType.Charity).ToList().Count.ToString(),
        //                                    HumansList.Where(r => r.gameObject.activeInHierarchy && r.HType == HumanType.Gratitude).ToList().Count.ToString(),
        //                                    HumansList.Where(r => r.gameObject.activeInHierarchy && r.HType == HumanType.Hate).ToList().Count.ToString());

        UIManagerScript.Instance.NumberOfEntity.text = "POPULATION: " + HumansList.Where(r => r.gameObject.activeInHierarchy).ToList().Count.ToString();
        UIButtonOver = false;

        foreach (BlockInput item in UIButtons.Where(r => r.isActiveAndEnabled))
        {
            if (item.UIButtonOver)
            {
                UIButtonOver = true;
                return;
            }
        }
        //Check if the player has changed
        if (PlayerHouseChake != PlayerHouse)
        {
            PlayerHouseChake = PlayerHouse;
            foreach (HouseScript house in Houses)
            {
                house.NewHouse = false;
                if (house.HouseType == PlayerHouse)
                {
                    house.IsPlayer = true;
                    //UIManagerScript.Instance.ChangePlayer(house.HouseType);
                }
                else
                {
                    house.IsPlayer = false;

                }

            }
        }

    }
    public void OpenMapBorder(float seconds)
    {
        MapBorder.gameObject.SetActive(true);
        Invoke("CloseMapBorder", seconds);
    }

    public void CloseMapBorder()
    {
        MapBorder.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        UIButtonOver = false;
        //UIManagerScript.Instance.ChangePlayer(PlayerHouse);
        //Initialize all the humans in all houses
        foreach (HouseScript house in Houses)
        {
            house.NewHouse = false;
            if (house.HouseType == PlayerHouse)
            {
                house.IsPlayer = true;

            }
            else
            {
                house.IsPlayer = false;

            }
            //assign all the humans of one house
            for (int i = 0; i < Humans; i++)
            {
                GameObject human = Instantiate(Human, house.transform.position, Quaternion.identity, HumansContainer);

                HumanBeingScript hbs = human.GetComponent<HumanBeingScript>();
                hbs.HouseType = house.HouseType;
                hbs.Specialization = 0.1f;
                hbs.HumanJob = HumanClass.Harvester;
                hbs.TargetHouse = house;
                hbs.Initialize();
                //human.GetComponent<MeshFilter>().sharedMesh = HumanMesh;
                HumansList.Add(hbs);
                hbs.FinallyBackHome += Hbs_FinallyBackHome;
                hbs.WearSkin();
                //hbs.gameObject.tag = "" + house.tag;
                house.Humans.Add(hbs);
            }

        }

        //creating the food resources based on the editable FoodPerDay parameter
        for (int i = 0; i < FoodPerDay; i++)
        {
            GameObject food = Instantiate(Food, FoodContainer);
            //food.GetComponent<MeshFilter>().sharedMesh = FoodMesh;
            //food.GetComponent<MeshRenderer>().sharedMaterial = FoodMaterial;
            food.SetActive(false);
            FoodsList.Add(food.GetComponent<FoodScript>());
        }
        Invoke("DayStarting", 1);
    }


    public void AddHouse(HousesTypes house)
    {
        if (house == PlayerHouse)
        {
            foreach (HouseScript h in Houses)
            {
                if (h.HouseType == PlayerHouse && h.HumansAlive.Count > 0)
                {
                    h.OpenBuildingCircle();
                    AddingPlayerHouse = true;
                }
            }
        }
    }
    public void AddMonsterMenu(HousesTypes house)
    {
        if (house == PlayerHouse)
        {
            foreach (HouseScript h in Houses)
            {
                if (h.HouseType == PlayerHouse && h.HumansAlive.Count > 0)
                {
                    h.OpenGuardianCircle();
                    AddingPlayerHouse = true;
                }
            }
        }
    }
    public void SetFood()
    {
        for (int i = 0; i < FoodPerDay; i++)
        {
            FoodsList[i].Food = 6;
            FoodsList[i].Slots = 6;

            FoodsList[i].GetComponent<Animator>().SetBool("UIState", false);
            //Invoke("RandomizeFoodPosition", 0.3f);
        }
        RandomizeFoodPosition();


        FlowerizeDead();
    }

    private void FlowerizeDead()
    {
        foreach (Animator dead in DeadList)
        {
            GameObject flower = Instantiate(SkinManager.Instance.GetSkinInfo(dead.GetComponent<TribeColorScript>().Tribe).Flower.gameObject, DeadContainer);
            flower.SetActive(true);
            flower.transform.position = new Vector3(dead.transform.position.x, 0, dead.transform.position.z) + new Vector3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(-3, 3));
            FlowerList.Add(flower.GetComponent<Animator>());
            //flower.GetComponent<SpriteRenderer>().color = dead.GetComponent<TribeColorScript>().TribeColor;
            /*GameObject food = Instantiate(Food, FoodContainer);
            food.SetActive(true);
            food.transform.position = dead.transform.position;
            //FoodsList.Add(food.GetComponent<FoodScript>());*/
            Destroy(dead.gameObject);
        }
        DeadList = new List<Animator>();
    }

    public void RandomizeFoodPosition()
    {
        for (int i = 0; i < FoodPerDay; i++)
        {
            if (FoodRandomized == false)
            {
                FoodsList[i].gameObject.SetActive(false);
                float radiusFromOrigin = FoodDistanceFromOrigin.Evaluate((float)i / (float)FoodPerDay);
                float f = UnityEngine.Random.Range(0f, 6f);
                //get the circular position from the random value
                Vector2 v = new Vector2(Mathf.Cos(f) * (radiusFromOrigin * FoodRaySpawn), Mathf.Sin(f) * (radiusFromOrigin * FoodRaySpawn));
                LayerMask layerMask = LayerMask.GetMask("ObstacleItem", "Food");
                List<RaycastHit> ElementHitted = Physics.SphereCastAll(new Vector3(v.x, 0, v.y), 10, new Vector3(0, 1, 0), 0, layerMask).ToList<RaycastHit>();
                //Debug.DrawLine(transform.position, new Vector3(v.x, 0, v.y), Color.yellow, 1);
                for (int i2 = 0; i2 < 1000; i2++)
                {
                    if (ElementHitted.Count > 0)
                    {
                        f = UnityEngine.Random.Range(0f, 6f);
                        v = new Vector2(Mathf.Cos(f) * (radiusFromOrigin * FoodRaySpawn), Mathf.Sin(f) * (radiusFromOrigin * FoodRaySpawn));
                        ElementHitted = Physics.SphereCastAll(new Vector3(v.x, 0, v.y), FoodMinDistance * f, new Vector3(0, 1, 0), 0, layerMask).ToList<RaycastHit>();
                        //Debug.DrawLine(transform.position, new Vector3(v.x, 0, v.y), Color.yellow, 1);
                    }
                    else
                    {
                        break;
                    }
                }
                FoodsList[i].transform.position = new Vector3(v.x, 0, v.y);


            }

            FoodsList[i].gameObject.SetActive(true);
            FoodsList[i].GetComponent<Animator>().SetBool("UIState", true);
        }
        FoodRandomized = true;
    }
    public Vector3 IsInsidePlayground(Vector3 pos)
    {
        if (pos.x < -GroundSizeWidth)
        {
            pos.x = -GroundSizeWidth;
        }
        if (pos.z < -GroundSizeWidth)
        {
            pos.z = -GroundSizeWidth;
        }
        if (pos.x > GroundSizeWidth)
        {
            pos.x = GroundSizeWidth;
        }
        if (pos.z > GroundSizeWidth)
        {
            pos.z = GroundSizeWidth;
        }
        return pos;
    }
    public Vector3 GetFreeSpaceOnGround(float y)
    {
        Vector3 res = new Vector3(UnityEngine.Random.Range(-GroundSizeWidth, GroundSizeWidth), y, UnityEngine.Random.Range(-GroundSizeWidth, GroundSizeWidth));
        return res;
    }

    public void Reproduction(HouseScript home, HumanClass humanJob)
    {
        int childNumber = UnityEngine.Random.Range(1, MaxNumChildren + 1);
        for (int i = 0; i < childNumber; i++)
        {
            if (home.HumansAlive.Count < MaxHumansForTribe)
            {
                SpawnHuman(home, humanJob);
            }
        }
    }
    public void AddPlayerHarvester()
    {
        AddHumanUsingFood(Houses.Where(r => r.IsPlayer).ToList()[0], HumanClass.Harvester);
    }
    public void AddPlayerWarrior()
    {
        AddHumanUsingFood(Houses.Where(r => r.IsPlayer).ToList()[0], HumanClass.Warrior);
    }
    public void AddHumanUsingFood(HouseScript home, HumanClass humanJob)
    {
        int foodRequired = humanJob == HumanClass.Harvester ? FoodRequiredHarvester : FoodRequiredWarrior;
        if (home.FoodStore >= foodRequired && home.HumansAlive.Count < MaxHumansForTribe)
        {
            SpawnHuman(home, humanJob);
            home.FoodStore -= foodRequired;
            if (home.HouseType == PlayerHouse)
                UIFollowSprite.Instance.ViewFoodConsumed(-foodRequired);
        }
    }
    public void SpawnHuman(HouseScript home, HumanClass humanJob)
    {
        GameObject human = Instantiate(Human, home.transform.position, Quaternion.identity, HumansContainer);
        HumanBeingScript hbs = human.GetComponent<HumanBeingScript>();
        hbs.Specialization = humanJob == HumanClass.Harvester ? 0.1f : 0.9f;
        hbs.HumanJob = humanJob;
        hbs.HouseType = home.HouseType;
        hbs.TargetHouse = home;
        hbs.Initialize();
        HumansList.Add(hbs);
        human.GetComponent<HumanBeingScript>().Hp = 60;
        hbs.HouseType = home.HouseType;
        hbs.FinallyBackHome += Hbs_FinallyBackHome;
        home.Humans.Add(hbs);
        hbs.WearSkin();
        home.HumansAlive = home.Humans.Where(x => x.Hp > 0).ToList();
        ReproducedLastDay++;
        //HumansAtHome++;
        hbs.CurrentState = StateType.LookingForFood;
        hbs.GoToRandomPos();
    }
    public void HumanBeingDied()
    {
        UIManagerScript.Instance.UpdatePeople();
        DiedLastDay++;
    }
    public void DayStarting()
    {
        UIManagerScript.Instance.InfoDailyUpdate();
        ReproducedLastDay = 0;
        DiedLastDay = 0;
        HumansAtHome = 0;
        if (DayTimeCoroutine != null)
        {
            //StopCoroutine(DayTimeCoroutine);
            DayTimeCoroutine = null;
        }
        this.StopAllCoroutines();
        DayTimeCoroutine = DayTimerCo();
        StartCoroutine(DayTimeCoroutine);
        UIManagerScript.Instance.UpdatePeople();
        //AudioManager.Instance.StartDay();
    }
    public IEnumerator DayTimerCo()
    {
        UIManagerScript.Instance.AddDay();
        DayStarted();
        GameStatus = GameStateType.DayStarted;
        int i = DayTime;
        CurrentTimeMS = 0;
        SetFood();
        AwakePeople();
        currentDayTime = i;
        while (i > 0)
        {
            yield return new WaitForSeconds(.1f);
            if (!Pause)
            {
                currentDayTime = i;
                UIManagerScript.Instance.TimerUpdate(i);
                if (i <= DayTime - DayLightTime)
                {
                    if (GameStatus != GameStateType.NightTime)
                    {
                        SpawnMonsters();
                        GameStatus = GameStateType.NightTime;
                    }
                }
                CurrentTimeMS += .1f;
                //yield return new WaitForSecondsRealtime(1);
                i = DayTime - (int)CurrentTimeMS;
            }
        }
        //the time of today is ended, start a new day
        GameStatus = GameStateType.EndOfDay;
        KillMonsters();
        ShareFoodIfNeeded();
        foreach (HouseScript house in Houses)
        {
            house.DistributeFood();
        }
        foreach (HouseScript house in Houses)
        {
            house.Breed();
        }
        Invoke("DayStarting", 1);
    }
    public void AwakePeople()
    {
        List<HumanBeingScript> humansAlive = HumansList.Where(r => r.Hp > 0).ToList();
        foreach (HumanBeingScript human in humansAlive)
        {
            human.CurrentState = StateType.LookingForFood;
            human.GoToRandomPos();
        }
    }
    private void ShareFoodIfNeeded()
    {
        foreach (HouseScript house in Houses)
        {
            if (house.FoodStore < house.HumansAlive.Count)
            {
                float foodRequest = house.HumansAlive.Count - house.FoodStore;
                foreach (HouseScript friendHouse in Houses)
                {
                    //if the house is a friend and has more food than required
                    if (friendHouse.HouseType == house.HouseType && friendHouse.FoodStore > friendHouse.HumansAlive.Count)
                    {
                        float foodTargetNumber = friendHouse.FoodStore - foodRequest;
                        //distribuisci il cibo necessario uno alla volta
                        while (friendHouse.FoodStore > friendHouse.HumansAlive.Count && friendHouse.FoodStore > foodTargetNumber)
                        {
                            friendHouse.FoodStore--;
                            house.FoodStore++;
                            if (house.FoodStore >= house.HumansAlive.Count)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private void SpawnMonsters()
    {
        foreach (MonsterHouse monsterHouse in MonsterHouses)
        {
            MonsterScript m = Instantiate(MonsterPrefab, monsterHouse.transform);
            Monsters.Add(m);
            m.House = monsterHouse.transform;
            m.RadiusOfExploration = monsterHouse.RangeOfMovement;
        }
    }
    private void KillMonsters()
    {
        for (int i = 0; i < Monsters.Count; i++)
        {
            Destroy(Monsters[i].gameObject);
        }
        Monsters = new List<MonsterScript>();
    }
    void Hbs_FinallyBackHome()
    {
        HumansAtHome++;
        //All humans are home, start a new day
        /*if (HumansAtHome == HumansList.Where(r => r.gameObject.activeInHierarchy).ToList().Count && currentDayTime >= DayTime)
        {
            foreach (HouseScript house in Houses)
            {
                house.DistributeFood();
            }
            Invoke("DayStarting", 1);
        }*/
    }
    public bool UsePlayerFood(int food)
    {
        UpdatePlayerFood();
        if (FoodPlayer >= food)
        {
            while (food > 0)
            {
                foreach (HouseScript ho in Houses)
                {
                    if (ho.HouseType == PlayerHouse && ho.FoodStore > 0)
                    {
                        ho.FoodStore--;
                        food--;
                    }
                }
            }
            UpdatePlayerFood();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void UpdatePlayerFood()
    {
        FoodPlayer = 0;
        foreach (HouseScript ho in Houses)
        {
            if (ho.HouseType == PlayerHouse)
            {
                FoodPlayer += ho.FoodStore;
            }
        }
    }
    internal void SpawnGuardian(HouseScript h)
    {

        if (GuardiansSummonable > 0)
        {
            MonsterScript m = Instantiate(MonsterPrefab);
            m.transform.position = h.transform.position;
            m.HouseHuman = h;
            m.House = h.transform;
            m.RadiusOfExploration = 30;
            GuardiansSummonable--;
        }
        CloseGuardianMenu();
    }
    internal void CloseGuardianMenu()
    {
        foreach (HouseScript h in Houses)
        {
            h.CloseGuardianCircle(false);
        }
        WorldmapCamera.Instance.IsBuilding = false;
    }
    public void AddGuardian(HousesTypes h)
    {
        foreach (HouseScript house in Houses)
        {
            if (house.HouseType == h)
            {
                MonsterScript m = Instantiate(MonsterPrefab);
                m.transform.position = house.transform.position;
                m.HouseHuman = house;
                m.House = house.transform;
                m.RadiusOfExploration = 15;
            }
        }
        /*if (h == PlayerHouse)
        {
            GuardiansSummonable++;

        }
        else
        {
            foreach (HouseScript house in Houses)
            {
                if (house.HouseType == h)
                {
                    MonsterScript m = Instantiate(MonsterPrefab);
                    m.transform.position = house.transform.position;
                    m.HouseHuman = house;
                    m.House = house.transform;
                    m.RadiusOfExploration = 15;
                }
            }
        }*/
    }
    public void SpecialAttack(Vector3 StartPosition, Vector3 EndPosition)
    {
        RangeAttackPosition = EndPosition;
        Invoke("RangeAttack", 0.3f);
    }

    public void RangeAttack()
    {
        List<HumanBeingScript> humansInRange = HumansList.Where(r => r.Hp > 0 && Vector3.Distance(RangeAttackPosition, r.transform.position) < RangeAttackDistance).ToList();
        List<MonsterScript> monsterInRange = Monsters.Where(r => r.Hp > 0 && Vector3.Distance(RangeAttackPosition, r.transform.position) < RangeAttackDistance).ToList();
        foreach (HumanBeingScript h in humansInRange)
        {
            h.UnderAttack(RangeAttackDamage);
        }
        foreach (MonsterScript m in monsterInRange)
        {
            m.UnderAttack(RangeAttackDamage);
        }
    }
}
public enum GameStateType
{
    Intro,
    DayStarted,
    EndOfDay,
    NightTime
}
public enum HousesTypes
{
    Green,
    Yellow,
    Red,
    Blue
}
public enum GameState
{
    Playing,
    Won,
    Lost
}
/*public class ResetableEnumerator<T> : IEnumerator<T>
{
    public IEnumerator<T> Enumerator { get; set; }
    public Func<IEnumerator<T>> ResetFunc { get; set; }

    public T Current { get { return Enumerator.Current; } }
    public void Dispose() { Enumerator.Dispose(); }
    object IEnumerator.Current { get { return Current; } }
    public bool MoveNext() { return Enumerator.MoveNext(); }
    public void Reset() { Enumerator = ResetFunc(); }
}*/
