using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildHouseManager : MonoBehaviour
{
    public static BuildHouseManager Instance;
    public bool BuildableArea = false;
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
        Houses = Houses.Where(r => r.IsPlayer && !r.Defeated).ToList();

        Vector3 mPos =  Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(mPos.x,transform.position.y, mPos.z);
        Color cState = BuildableArea ? Color.green : Color.red;
        cState.a = 0.5f;
        GetComponent<SpriteRenderer>().color = cState;
        BuildableArea = false;
        foreach (HouseScript house in Houses)
        {
            mPos = new Vector3(mPos.x, house.transform.position.y, mPos.z);

            if (Vector3.Distance(mPos, house.transform.position) < 80 && Vector3.Distance(mPos, house.transform.position) > 20)
            {
                BuildableArea = true;
            }
            if(Vector3.Distance(mPos, house.transform.position) < 20)
            {
                BuildableArea = false;
                break;
            }
            if (!GameManagerScript.Instance.IsInBoundary(mPos))
            {
                BuildableArea = false;
                break;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Offset = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) && BuildableArea && GameManagerScript.Instance.FoodPlayer>= FoodRequired && Vector2.Distance(Offset,Input.mousePosition)<30)
        {
            GameManagerScript.Instance.UsePlayerFood(FoodRequired);
            GameManagerScript.Instance.SpawnNewHouse(GameManagerScript.Instance.PlayerHouse,mPos);
        }
        else if(Input.GetMouseButtonUp(0) && !BuildableArea && GameManagerScript.Instance.FoodPlayer >= FoodRequired && Vector2.Distance(Offset, Input.mousePosition) < 30)
        {
            GameManagerScript.Instance.CloseBuildMenu();
        }
    }

}
