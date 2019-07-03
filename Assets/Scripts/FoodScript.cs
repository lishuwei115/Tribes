using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour {



    [Range(0, 100)]
    public int FoodMin = 2;
    [Range(0, 100)]
    public int FoodMax = 6;
	public int Food;
    [Range(0,1000)]
    public float HardnessMin;
    [Range(0, 1000)]
    public float HardnessMax;
    public float Hardness = 10;
    private void OnEnable()
    {
        Food = Random.Range(FoodMin, FoodMax + 1);
        Hardness = Random.Range(HardnessMin, HardnessMax+1);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
