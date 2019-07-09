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
        Houses = Houses.Where(r => r.IsPlayer).ToList();

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
                BuildHouseManager.Instance.BuildableArea = true;
            }
        }
    }

}
