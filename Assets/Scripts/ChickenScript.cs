using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//Or sequence | |
public class ChickenScript : MonoBehaviour
{
   /* [Range(0,180)]
    public float VisualWidthDegree;
    [Range(0, 100)]
    public float VisualDistance;

    [Header("Walk")]
    [Range(1, 5)]
    public float WalkSpeed = 10;
    [Range(1, 100)]
    public float WalkDistance = 10;
    [Header("Run")]
    [Range(1, 5)]
    public float RunSpeed = 10;
    [Range(1, 100)]
    public float RunDistance = 10;


    [Header("Follow")]
    [Range(1, 5)]
    public float FollowSpeed = 10;
    [Range(1, 100)]
    public float FollowDistance = 10;

    private IEnumerator Walk;
    private IEnumerator Run;
    private IEnumerator Follow;
    private IEnumerator GoTo;

    public ChickenState CState;
    private Rigidbody rb;
    public Transform DogOwner;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
      if(Input.GetKeyUp(KeyCode.A))
        {
            LookingAround();
        }
    }


    private void StartingMatchEvent()
    {
        CState = ChickenState.Idle;
        StartCoroutine(Live());
    }


    private IEnumerator Live()
    {
        bool isItAlive = true;
        while(isItAlive)
        {
            float NextActionTime = 2; //Random.Range(0, 10);
            yield return new WaitForSecondsRealtime(NextActionTime);
            if (CState == ChickenState.Idle)
            {
               
                int c = Random.Range(0, 100);
                 if(c < 50)
                 {
                     if(Walk != null)
                     {
                         StopCoroutine(Walk);

                     }
                     Walk = Walking();
                     StartCoroutine(Walk);
                 }
                 else
                 {
                    
                 }  
                
            }
        }
    }

    public void FollowSetup(Transform dogOwner, PlayerType owner)
    {
        Owner = owner;
        DogOwner = dogOwner;
        CState = ChickenState.Follow;
        if (Follow != null)
        {
            StopCoroutine(Follow);
        }
        Follow = Following();
        StartCoroutine(Follow);
    }

    public void StopFollowing(PlayerType barkingDog, Vector3 dogPos)
    {
        if(Owner != PlayerType.none && Owner.ToString() != barkingDog.ToString())
        {
            Owner = PlayerType.none;
            StopCoroutine(Follow);
            StartCoroutine(Running(dogPos));
        }
    }

    public IEnumerator Following()
    {
        while(CState == ChickenState.Follow && DogOwner != null)
        {
            yield return new WaitForEndOfFrame();
            float dis = Vector3.Distance(DogOwner.position, transform.position);
           // Debug.Log(dis);
            if (dis > 3 && GoTo == null)
            {
                GoTo = GoToCo(DogOwner.position);
                StartCoroutine(GoTo);
            }
        }
    }

    public void Idle()
    {
        //Idle
    }


    public IEnumerator GoToCo(Vector3 pos)
    {
        pos += new Vector3(Random.Range(-FollowDistance, FollowDistance), 0, Random.Range(-FollowDistance, FollowDistance));
        Vector3 offset = transform.position;
        float timeCount = 0.0f;
        while (timeCount < 1)
        {
            transform.LookAt(pos);
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(offset, pos, timeCount);
            timeCount = timeCount + Time.deltaTime * FollowSpeed;
        }

        GoTo = null;
    }

    public IEnumerator Walking()
    {
        CState = ChickenState.Walk;
        LookingAround();
        Vector3 offset = transform.position;
        Vector3 NextPos = transform.position + new Vector3(Random.Range(-WalkDistance, WalkDistance), 0, Random.Range(-WalkDistance, WalkDistance));
        NextPos.y = 0;
        transform.LookAt(NextPos);

       // Debug.Log(NextPos);
        yield return new WaitForEndOfFrame();
        float timeCount = 0.0f;
        while (timeCount < 1)
        {
            yield return new WaitForEndOfFrame();
            LookingAround();
            transform.position = Vector3.Lerp(offset, NextPos, timeCount);
            timeCount = timeCount + Time.deltaTime * WalkSpeed;
        }
        CState = ChickenState.Idle;
    }

    public IEnumerator Running(Vector3 enemyPos)
    {
        CState = ChickenState.Run;
        Vector3 offset = transform.position;
        Vector3 NextPos = (enemyPos - transform.position) * -RunDistance;
        NextPos.y = 0;
        transform.LookAt(NextPos);

        yield return new WaitForEndOfFrame();
        float timeCount = 0.0f;
        while (timeCount < 1)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(offset, NextPos, timeCount);
            timeCount = timeCount + Time.deltaTime * RunSpeed;
        }
        CState = ChickenState.Idle;
    }



    /// <summary>
    /// The chicken is looking around for other Chickens or enemies
    /// </summary>
    private void LookingAround()
    {
        List<RaycastHit> ElementHitted = new List<RaycastHit>();
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, VisualDistance, transform.forward, VisualDistance);
        ElementHitted.AddRange(hits.ToList());
        ElementHitted = ElementHitted.Where(r=> r.collider.tag == "Dog").ToList();
        if(ElementHitted.Count > 0)
        {
           
            foreach (RaycastHit hit in ElementHitted)
            {
                Vector3 targetDir = hit.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);

                if(angle < (VisualWidthDegree / 2))
                {
                    if (Walk != null)
                    {
                        StopCoroutine(Walk);
                    }
                    StartCoroutine(Running(hit.transform.position));
                }
               
            }
        }
        else
        {

        }

    }

    private void LateUpdate()
    {
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
    }*/
}



public enum ChickenState
{
    none,
    Idle,
    Looking,
    Walk,
    Run,
    Follow
}
