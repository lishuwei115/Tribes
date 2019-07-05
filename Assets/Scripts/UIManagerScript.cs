using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManagerScript : MonoBehaviour {

	public static UIManagerScript Instance;
    
	public TextMeshProUGUI Timer;
	public TextMeshProUGUI DayNum;
	public TextMeshProUGUI NumberOfEntity;
	public TextMeshProUGUI BirthLastDay;
	public TextMeshProUGUI DeathLastDay;
	public TextMeshProUGUI Charity;
    public TextMeshProUGUI Gratitude;
    public TextMeshProUGUI Hate;
    public TextMeshProUGUI HouseFoodUpperLeft;
    public TextMeshProUGUI HouseFoodUpperRight;
    public TextMeshProUGUI HouseFoodDownwardLeft;
    public TextMeshProUGUI HouseFoodDownwardRight;
    public TextMeshProUGUI HousePeopleUpperLeft;
    public TextMeshProUGUI HousePeopleUpperRight;
    public TextMeshProUGUI HousePeopleDownwardLeft;
    public TextMeshProUGUI HousePeopleDownwardRight;

	public int DayNumIterator = 0;

	private void Awake()
	{
		Instance = this;
	}


	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

    }

    public void TimerUpdate(int timer)
	{
		Timer.text = "Day Timer: " + timer;

    }

    public void InfoDailyUpdate(string bld, string dld)
	{
		BirthLastDay.text = "Birth last Day: " + bld;
		DeathLastDay.text = "Death last Day: " + dld;
	}

	public void InfoUpdate(string c, string g, string h)
    {
		Charity.text = "Charity:" + c;
		Gratitude.text = "Gratitude:" + g;
		Hate.text = "Hate:" + h;
    }
    public void UpdateFood()
    {
        HouseFoodUpperLeft.text = "Food: " + GameManagerScript.Instance.Houses[0].FoodStore;
        HouseFoodUpperRight.text = "Food: " + GameManagerScript.Instance.Houses[1].FoodStore;
        HouseFoodDownwardLeft.text = "Food: " + GameManagerScript.Instance.Houses[2].FoodStore;
        HouseFoodDownwardRight.text = "Food: " + GameManagerScript.Instance.Houses[3].FoodStore;
        UpdatePeople();


}
    public void UpdatePeople()
    {
        HousePeopleUpperLeft.text = "People: " + GameManagerScript.Instance.Houses[0].HumansAlive.Count;
        HousePeopleUpperRight.text = "People: " + GameManagerScript.Instance.Houses[1].HumansAlive.Count;
        HousePeopleDownwardLeft.text = "People: " + GameManagerScript.Instance.Houses[2].HumansAlive.Count;
        HousePeopleDownwardRight.text = "People: " + GameManagerScript.Instance.Houses[3].HumansAlive.Count;
    }
    public void AddDay()
	{
		DayNumIterator++;
		DayNum.text = "Day Num:" + DayNumIterator;
	}
    public void WinLoseState(int state)
    {
        WinLoseManager.Instance.WinLoseState(state);
    }
}
