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


    private void Awake()
    {
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
        //eat when starving
        if (Hp < 0)
        {
            if (AnimController != null)
            {
                //AttackAnimation if there is an animator
                AnimController.transform.SetParent(GameManagerScript.Instance.HumansContainer);
                AnimController.SetInteger("UIState", 3);
            }
            //To do         //GameManagerScript.Instance.MonsterDied();
            gameObject.SetActive(false);
            //Destroy(gameObject);

        }
        if (Time.time - OffsetTimer >= GameManagerScript.Instance.DayTime)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
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
                EnemyLayer = LayerMask.GetMask("West", "South", "East", "North");
    }
    public void GoToRandomPos()
    {
        if (gameObject.activeInHierarchy)
        {
            TargetDest = new Vector3(UnityEngine.Random.Range(transform.position.x - Radius * 4, transform.position.x + Radius*4),transform.position.y, UnityEngine.Random.Range(transform.position.z - Radius * 4, transform.position.z + Radius * 4));
            //check the borders
            TargetDest.x = Mathf.Clamp(TargetDest.x, -GameManagerScript.Instance.GroundSizeWidth,GameManagerScript.Instance.GroundSizeWidth);
            TargetDest.z = Mathf.Clamp(TargetDest.z, -GameManagerScript.Instance.GroundSizeHeight, GameManagerScript.Instance.GroundSizeHeight);
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
            List<RaycastHit> EnemiesCollision = LookAroundForEnemies();
            if ( CurrentState != MonsterState.Dead && EnemiesCollision.Count > 0  && TargetHuman == null)
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
            if(Enemy.CurrentState == StateType.Home)
            {
                CurrentState = MonsterState.Patroll;
                GoToRandomPos();
                FollowCo = null;
            }
        }
        while (EnemyAlive)
        {
            //move towars target
            while (distance >= 4f)
            {
                distance = Vector3.Distance(transform.position, humanT.position);
                //offset = transform.position;
                timeCount = 0;
                yield return new WaitForEndOfFrame();
                timeCount = timeCount + Time.deltaTime * Speed / distance * 10;
                Vector3 nextpos = Vector3.Lerp(transform.position, humanT.position, timeCount);
                transform.position = nextpos;
            }
            //start another coroutine, not compatible with current system
            float Dist = Vector3.Distance(transform.position, humanT.position);
            //Attack
            if (Dist < 4f && CanIAttack)
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
                        AnimController.SetInteger("UIState", 2);
                    }
                    if (Enemy.Hp <= 0)
                    {
                        Food += Enemy.Food;
                        Enemy.Food = 0;
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
                        AnimController.SetInteger("UIState", 2);
                    }
                    if (EnemyMonster.Hp <= 0)
                    {
                        Food += EnemyMonster.Food;
                        EnemyMonster.Food = 0;
                        EnemyAlive = false;
                        HPBar.gameObject.SetActive(false);
                    }
                }
            }
            if (Dist > 4)
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
