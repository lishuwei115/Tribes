using System;
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
    public GameState StateOfGame = GameState.Playing;
    public delegate void StartDay();
    public event StartDay DayStarted;
    public HousesTypes PlayerHouse = HousesTypes.East;
    HousesTypes PlayerHouseChake = HousesTypes.East;
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

    [Range(0, 300)]
    public int DayTime = 30;

    [Range(0, 300)]
    public int DayLightTime = 60;

    [Range(1, 500)]
    public int Humans = 10;

    //[Range(0, 500)]
    //public int NumberOfMonsters = 6;

    [Range(1, 500)]
    public int FoodPerDay = 10;


    [Range(0, 10)]
    public int MaxNumChildren = 3;

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
    private void Awake()
    {
        Instance = this;
        MonsterHouses = UnityEngine.Object.FindObjectsOfType<MonsterHouse>().ToList<MonsterHouse>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;


    }

    // Use this for initialization
    void Start()
    {
        StartGame();
        Invoke("DayStarting", 3);
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
                house.MoveTribeTo(pos);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
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
                    UIManagerScript.Instance.ChangePlayer(house.HouseType);
                }
                else
                {
                    house.IsPlayer = false;

                }

            }
        }

    }

    public void StartGame()
    {
        UIButtonOver = false;

        //Initialize all the humans in all houses
        foreach (HouseScript house in Houses)
        {
            house.NewHouse = false;
            if (house.HouseType == PlayerHouse)
            {
                house.IsPlayer = true;
                UIManagerScript.Instance.ChangePlayer(house.HouseType);
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
                human.GetComponent<MeshFilter>().sharedMesh = HumanMesh;
                HumansList.Add(hbs);

                hbs.HouseType = house.HouseType;
                hbs.TargetHouse = house;
                hbs.FinallyBackHome += Hbs_FinallyBackHome;
                hbs.TargetHouse = house.GetComponent<HouseScript>();
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
            food.GetComponent<MeshRenderer>().sharedMaterial = FoodMaterial;
            food.SetActive(false);
            FoodsList.Add(food.GetComponent<FoodScript>());

        }

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
        for (int i = 0; i < FoodPerDay - 1; i++)
        {

            FoodsList[i].GetComponent<Animator>().SetBool("UIState", false);
            Invoke("RandomizeFoodPosition", 0.3f);
        }
        foreach (Animator dead in DeadList)
        {

            GameObject flower = Instantiate(Flower, DeadContainer);
            flower.SetActive(true);
            flower.transform.position = new Vector3(dead.transform.position.x, 0, dead.transform.position.z) + new Vector3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(-3, 3));
            FlowerList.Add(flower.GetComponent<Animator>());
            flower.GetComponent<SpriteRenderer>().color = dead.GetComponent<TribeColorScript>().TribeColor;
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
        for (int i = 0; i < FoodPerDay - 1; i++)
        {

            FoodsList[i].gameObject.SetActive(true);
            FoodsList[i].GetComponent<Animator>().SetBool("UIState", true);
            FoodsList[i].transform.position = GetFreeSpaceOnGround(0);
        }
    }

    public Vector3 GetFreeSpaceOnGround(float y)
    {
        Vector3 res = new Vector3(UnityEngine.Random.Range(-GroundSizeWidth, GroundSizeWidth), y, UnityEngine.Random.Range(-GroundSizeWidth, GroundSizeWidth));

        return res;
    }

    public void Reproduction(HouseScript home)
    {
        int childNumber = UnityEngine.Random.Range(1, MaxNumChildren + 1);
        for (int i = 0; i < childNumber; i++)
        {
            GameObject human = Instantiate(Human, home.transform.position, Quaternion.identity, HumansContainer);
            HumanBeingScript hbs = human.GetComponent<HumanBeingScript>();
            HumansList.Add(hbs);
            human.GetComponent<HumanBeingScript>().Hp = 60;
            hbs.TargetHouse = home;
            hbs.HouseType = home.HouseType;
            hbs.FinallyBackHome += Hbs_FinallyBackHome;
            home.Humans.Add(hbs);
            hbs.WearSkin();
            ReproducedLastDay++;
            HumansAtHome++;
        }
    }

    public void HumanBeingDied()
    {
        UIManagerScript.Instance.UpdatePeople();
        DiedLastDay++;
    }


    public void DayStarting()
    {

        UIManagerScript.Instance.InfoDailyUpdate(ReproducedLastDay.ToString(), DiedLastDay.ToString());
        ReproducedLastDay = 0;
        DiedLastDay = 0;
        HumansAtHome = 0;
        if (DayTimeCoroutine != null)
        {
            StopCoroutine(DayTimeCoroutine);
        }
        StopAllCoroutines();

        DayTimeCoroutine = DayTimerCo();
        StartCoroutine(DayTimeCoroutine);
        UIManagerScript.Instance.UpdatePeople();
        AudioManager.Instance.StartDay();

    }

    public IEnumerator DayTimerCo()
    {
        SetFood();
        DayStarted();
        UIManagerScript.Instance.AddDay();
        GameStatus = GameStateType.DayStarted;
        int i = DayTime;
        CurrentTimeMS = 0;

        while (i > 0)
        {
            currentDayTime = i;
            UIManagerScript.Instance.TimerUpdate(i);
            if (i <= DayTime - DayLightTime)
            {
                if (GameStatus != GameStateType.NightTime)
                    SpawnMonsters();
                GameStatus = GameStateType.NightTime;

            }
            yield return new WaitForSecondsRealtime(.01f);
            CurrentTimeMS += .01f;
            yield return new WaitForSecondsRealtime(1);
            if (!Pause)
            {
                i--;
            }
        }
        //the time of today is ended, start a new day
        GameStatus = GameStateType.EndOfDay;
        KillMonsters();


        foreach (HouseScript house in Houses)
        {
            house.Breed();

        }
        ShareFoodIfNeeded();

        foreach (HouseScript house in Houses)
        {
            house.DistributeFood();
        }
        Invoke("DayStarting", 1);
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
        if (h == PlayerHouse)
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
    North,
    South,
    East,
    West
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
