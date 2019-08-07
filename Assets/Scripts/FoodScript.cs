using System;
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
    public int Slots;
    private void OnEnable()
    {
        ResetFood();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Food <= 0)
        {
            Deactivate();
        }
	}
    public bool AskSlot()
    {
        bool b = Slots > 0 ? true : false;
        if (b)
        {
            Slots--;
        }
        return b;
    }
    public void ReleaseSlot()
    {
        Slots++;
    }
    public int TakeFood(int food)
    {
        if (Food >= food)
        {
            Food -= food;
            return food;
        }
        else
        {
            int f = Food;
            Food = 0;
            return f;
        }
    }
    internal void Deactivate()
    {
        GetComponent<Animator>().SetBool("UIState", false);
        Slots = 0;
        //Invoke("SetDeactive", 0.3f);
    }
    internal void SetDeactive()
    {
        gameObject.SetActive(false);
    }

    internal void ResetFood()
    {
        Food = UnityEngine.Random.Range(FoodMin, FoodMax + 1);
        Hardness = UnityEngine.Random.Range(HardnessMin, HardnessMax + 1);
        Slots = Food;
    }
}
