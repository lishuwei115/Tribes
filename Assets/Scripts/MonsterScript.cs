using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;
using System;

public class MonsterScript : MonoBehaviour
{
    public MonsterState CurrentState = MonsterState.Patroll;
    public float Radius = 4;
    [Header("Controll parameters")]
    public float PhisicalAttack;
    public float Attack;
    public float Speed;
    public float Food;
    public float BaseHP;
    [Tooltip("The life of the food attacked")]
    public float FoodLife = 0;
    public bool Alive = true;

    [Header("Twickable parameters")]
    [Range(0, 1000)]
    public float HpMax;
    [Range(0, 1000)]
    public float HpMin;
    public float Hp;
    [Range(0, 1000)]
    public float SpeedMax;
    [Range(0, 1000)]
    public float SpeedMin;
    [Range(0, 1000)]
    public float AttackMax;
    [Range(0, 1000)]
    public float AttackMin;
    public float AttackFrequency = 1;
    public HealthBarSprite HPBar;
    public Vector3 TargetDest;
    public Transform TargetHuman;
    [HideInInspector]
    public bool IsStarted = false;
    [HideInInspector]
    public bool CanIAttack = true;
    [HideInInspector]
    private IEnumerator FollowCo;
    private IEnumerator MoveCo;
    private float OffsetTimer = 0;
    int Id = 0;
    bool AttackDecision = false;
    Animator AnimController = null;
    LayerMask EnemyLayer;
    public float RadiusOfExploration = 15;
    public Transform House;
    public HouseScript HouseHuman = null;
    private void Awake()
    {
        AnimController = GetComponentInChildren<Animator>();
    }




    // Use this for initialization
    void Start()
    {
        HPBar = GetComponentInChildren<HealthBarSprite>();
        HPBar.gameObject.SetActive(false);
        InitializeRandomParameters();
        //Set Layer of allies and enemies
        SetLayers();
        Food = 30;
        StartMoving();

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManagerScript.Instance.Pause)
        {
            if (AnimController != null)
            {
                AnimController.speed = 1;
            }

            //eat when starving
            if (Hp < 0)
            {
                if (AnimController != null)
                {
                    //Death animation if there is an animator
                    //AnimController.transform.SetParent(GameManagerScript.Instance.HumansContainer);
                    AnimController.speed = 1;

                    AnimController.SetInteger("UIState", 2);
                }
                //To do         //GameManagerScript.Instance.MonsterDied();
                gameObject.SetActive(false);
                //Destroy(gameObject);

            }
            if (GameManagerScript.Instance.DayTime - GameManagerScript.Instance.currentDayTime >= GameManagerScript.Instance.DayTime && HouseHuman == null)
            {
                //Destroy(gameObject);
                gameObject.SetActive(false);
            }
        }
        else
        {
            AnimController.speed = 0;

        }

    }


    private void InitializeRandomParameters()
    {
        Speed = UnityEngine.Random.Range(SpeedMin, SpeedMax) / 10;
        Hp = UnityEngine.Random.Range(HpMin, HpMax);
        Attack = UnityEngine.Random.Range(AttackMin, AttackMax);
        PhisicalAttack = Attack;
        BaseHP = Hp;
    }
    private void SetLayers()
    {
        gameObject.layer = LayerMask.NameToLayer("Monster");
        if (HouseHuman == null)
        {
            EnemyLayer = LayerMask.GetMask("West", "South", "East", "North");
        }
        else
        {
            switch (HouseHuman.HouseType)
            {
                case HousesTypes.Green:
                    gameObject.layer = LayerMask.NameToLayer("North");
                    EnemyLayer = LayerMask.GetMask("West", "South", "East");
                    break;
                case HousesTypes.Yellow:
                    gameObject.layer = LayerMask.NameToLayer("South");
                    EnemyLayer = LayerMask.GetMask("West", "North", "East");
                    break;
                case HousesTypes.Red:
                    gameObject.layer = LayerMask.NameToLayer("East");
                    EnemyLayer = LayerMask.GetMask("West", "South", "North");
                    break;
                case HousesTypes.Blue:
                    gameObject.layer = LayerMask.NameToLayer("West");
                    EnemyLayer = LayerMask.GetMask("North", "South", "East");
                    break;
            }
        }
    }
    public void GoToRandomPos()
    {
        if (gameObject.activeInHierarchy)
        {
            TargetDest = new Vector3(UnityEngine.Random.Range(transform.position.x - Radius * 4, transform.position.x + Radius * 4), transform.position.y, UnityEngine.Random.Range(transform.position.z - Radius * 4, transform.position.z + Radius * 4));
            //check the borders
            //TargetDest.x = Mathf.Clamp(TargetDest.x, House.transform.position.x-RadiusOfExploration, House.transform.position.x+RadiusOfExploration);
            //TargetDest.z = Mathf.Clamp(TargetDest.z, House.transform.position.z-RadiusOfExploration, House.transform.position.z+RadiusOfExploration);
            Vector3 centerPosition = House.transform.position; //center of *black circle*
            float distance2 = Vector3.Distance(TargetDest, centerPosition); //distance from ~green object~ to *black circle*

            if (distance2 > RadiusOfExploration) //If the distance is less than the radius, it is already within the circle.
            {
                Vector3 fromOriginToObject = TargetDest - centerPosition; //~GreenPosition~ - *BlackCenter*
                fromOriginToObject *= RadiusOfExploration / distance2; //Multiply by radius //Divide by Distance
                TargetDest = centerPosition + fromOriginToObject; //*BlackCenter* + all that Math
            }
            GoToPosition(TargetDest);
        }
    }
    public void GoToPosition(Vector3 nextPos)
    {

        if (gameObject.activeInHierarchy)
        {
            IsStarted = true;
            MoveCo = Move(nextPos);
            StopAllCoroutines();
            StartCoroutine(MoveCo);
            if (AnimController != null)
            {
                //Walk animation if there is an animator
                AnimController.SetInteger("UIState", 0);
            }
        }
    }
    private List<RaycastHit> LookAroundForEnemies()
    {
        List<RaycastHit> Enemy = new List<RaycastHit>();
        Enemy = Physics.SphereCastAll(transform.position, Radius, transform.forward, Radius, EnemyLayer).ToList<RaycastHit>();
        return Enemy;
    }
    void StartMoving()
    {
        OffsetTimer = Time.time;
        GoToRandomPos();

        CurrentState = MonsterState.Patroll;
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
        TargetHuman = null;
        float timeCount = 0;
        while (timeCount < 1)
        {
            //stop game
            yield return new WaitUntil(() => !GameManagerScript.Instance.Pause);
            List<RaycastHit> EnemiesCollision = LookAroundForEnemies();
            if (CurrentState != MonsterState.Dead && EnemiesCollision.Count > 0 && TargetHuman == null)
            {
                TargetHuman = EnemiesCollision[0].collider.transform;
                AttackEnemy(TargetHuman);
            }
            yield return new WaitForEndOfFrame();
            Vector3 nextpos = Vector3.Lerp(offset, dest, timeCount);
            transform.position = nextpos;
            timeCount = timeCount + Time.deltaTime * Speed / distance * 10;
        }
        CurrentState = MonsterState.Patroll;
        GoToRandomPos();
        MoveCo = null;
    }
    public void AttackEnemy(Transform humanT)
    {
        if (FollowCo == null && gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            MoveCo = null;

            FollowCo = FollowEnemy(humanT);
            StartCoroutine(FollowCo);
        }
    }

    public void UnderAttack(float damage)
    {
        //update HP Bar
        HPBar.gameObject.SetActive(true);
        HPBar.UpdateHP(Hp, BaseHP);
        Hp -= damage;
        List<RaycastHit> EnemiesCollision = LookAroundForEnemies();
        //Counterattack
        if (EnemiesCollision.Count > 0)
        {
            AttackEnemy(EnemiesCollision[0].collider.transform);
        }
    }
    private IEnumerator FollowEnemy(Transform humanT)
    {
        yield return new WaitForEndOfFrame();
        bool EnemyAlive = true;
        Vector3 offset = transform.position;
        float distance = Vector3.Distance(offset, humanT.position);
        float timeCount = 0;
        bool humanEnemy = true;
        HumanBeingScript Enemy = new HumanBeingScript();
        MonsterScript EnemyMonster = new MonsterScript();
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
        if (humanEnemy)
        {
            if (Enemy.CurrentState == StateType.Home)
            {
                CurrentState = MonsterState.Patroll;
                GoToRandomPos();
                FollowCo = null;
            }
        }
        while (EnemyAlive)
        {
            //stop game
            yield return new WaitUntil(() => !GameManagerScript.Instance.Pause);
            //move towars target
            while (distance >= 20)
            {
                //stop game
                yield return new WaitUntil(() => !GameManagerScript.Instance.Pause);
                distance = Vector3.Distance(transform.position, humanT.position);
                //offset = transform.position;
                timeCount = 0;
                yield return new WaitForEndOfFrame();
                timeCount = timeCount + Time.deltaTime * Speed / distance * 10;
                Vector3 nextpos = Vector3.Lerp(transform.position, humanT.position, timeCount);
                //nextpos.x = Mathf.Clamp(nextpos.x, House.transform.position.x - RadiusOfExploration, House.transform.position.x + RadiusOfExploration);
                //nextpos.z = Mathf.Clamp(nextpos.z, House.transform.position.z - RadiusOfExploration, House.transform.position.z + RadiusOfExploration);

                //float radius = 400; //radius of *black circle*
                Vector3 centerPosition = House.transform.position; //center of *black circle*
                float distance2 = Vector3.Distance(nextpos, centerPosition); //distance from ~green object~ to *black circle*

                if (distance2 > RadiusOfExploration) //If the distance is less than the radius, it is already within the circle.
                {
                    Vector3 fromOriginToObject = nextpos - centerPosition; //~GreenPosition~ - *BlackCenter*
                    fromOriginToObject *= RadiusOfExploration / distance2; //Multiply by radius //Divide by Distance
                    nextpos = centerPosition + fromOriginToObject; //*BlackCenter* + all that Math
                }


                transform.position = nextpos;
                if (Enemy == null)
                {
                    FollowCo = null;

                    CurrentState = MonsterState.Patroll;
                    GoToRandomPos();
                }
                else
                if (Vector3.Distance(Enemy.transform.position, House.transform.position) > RadiusOfExploration)
                {
                    FollowCo = null;

                    CurrentState = MonsterState.Patroll;
                    GoToRandomPos();
                }
                if (distance < 20)
                {
                    break;
                }
            }
            //start another coroutine, not compatible with current system
            float Dist = Vector3.Distance(transform.position, humanT.position);
            //Attack
            if (Dist < 20 && CanIAttack)
            {
                //update HP Bar
                CurrentState = MonsterState.Attacking;
                HPBar.gameObject.SetActive(true);
                HPBar.UpdateHP(Hp, BaseHP);
                CanIAttack = false;
                Invoke("AttackAction", AttackFrequency);
                if (humanEnemy)
                {
                    Enemy.UnderAttack(PhisicalAttack);
                    if (AnimController != null)
                    {
                        //AttackAnimation if there is an animator
                        AnimController.SetInteger("UIState", 1);
                    }
                    if (Enemy.Hp <= 0)
                    {
                        //Food += Enemy.Food;
                        //Enemy.Food = 0;
                        EnemyAlive = false;
                        HPBar.gameObject.SetActive(false);
                    }
                }
                else
                {
                    EnemyMonster.UnderAttack(PhisicalAttack);
                    if (AnimController != null)
                    {
                        //AttackAnimation if there is an animator
                        AnimController.SetInteger("UIState", 1);
                    }
                    if (EnemyMonster.Hp <= 0)
                    {
                        //Food += EnemyMonster.Food;
                        //EnemyMonster.Food = 0;
                        EnemyAlive = false;
                        HPBar.gameObject.SetActive(false);
                    }
                }
            }
            if (Dist > 20)
            {
                FollowCo = null;

                CurrentState = MonsterState.Patroll;
                GoToRandomPos();
            }
            //won?
            yield return new WaitForEndOfFrame();
        }

        CurrentState = MonsterState.Patroll;
        GoToRandomPos();
        FollowCo = null;
    }
    private void AttackAction()
    {
        CanIAttack = true;
    }
}



public enum MonsterState
{
    Patroll = 0,
    Attacking = 1,
    Dead = 2
}
