using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AddGuardianManager : MonoBehaviour
{
    public static AddGuardianManager Instance;
    public bool SpawnableArea = false;
    public List<HouseScript> Houses;
    bool initialized = false;
    public int FoodRequired = 30;
    Vector2 Offset;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Houses = GameManagerScript.Instance.Houses;
        initialized = true;

    }
    private void OnEnable()
    {
        if (initialized)
        {
            Houses = GameManagerScript.Instance.Houses;
        }

    }
    public void Update()
    {
        Houses = GameManagerScript.Instance.Houses;
        Houses = Houses.Where(r => r.IsPlayer).ToList();

        Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(mPos.x, transform.position.y, mPos.z);
        Color cState = SpawnableArea ? Color.green : Color.red;
        cState.a = 0.5f;
        GetComponent<SpriteRenderer>().color = cState;
        SpawnableArea = false;
        HouseScript SpawnHouse = null;
        foreach (HouseScript house in Houses)
        {
            mPos = new Vector3(mPos.x, house.transform.position.y, mPos.z);

            if (Vector3.Distance(mPos, house.transform.position) < 20)
            {
                SpawnableArea = true;
            }
            if (Vector3.Distance(mPos, house.transform.position) >   20)
            {
                SpawnableArea = false;
                break;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Offset = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) && SpawnableArea && Vector2.Distance(Offset, Input.mousePosition) < 30)
        {
            SpawnHouse = Houses[0];
            foreach (HouseScript house in Houses)
            {

                if(Vector3.Distance(mPos, house.transform.position)< Vector3.Distance(mPos, SpawnHouse.transform.position))
                {
                    SpawnHouse = house;

                }
            }
                GameManagerScript.Instance.SpawnGuardian(SpawnHouse);
        }
        else if (Input.GetMouseButtonUp(0) && !SpawnableArea && GameManagerScript.Instance.UsePlayerFood(FoodRequired) && Vector2.Distance(Offset, Input.mousePosition) < 30)
        {
            GameManagerScript.Instance.CloseGuardianMenu();
        }
    }

}
