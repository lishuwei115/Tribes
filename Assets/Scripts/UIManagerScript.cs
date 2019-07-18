using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManagerScript : MonoBehaviour {

	public static UIManagerScript Instance;

    public Animator DayNightWheel;
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
    public Button AddHouse;
	public int DayNumIterator = 0;

	private void Awake()
	{
		Instance = this;
	}


	// Use this for initialization
	void Start () {
        DayNightWheel.speed = 0;

    }

    // Update is called once per frame
    void Update () {
        UpdatePeople();
        UpdateFood();
    }

    public void TimerUpdate(int timer)
	{
		Timer.text = "" + timer;
    }

    public void InfoDailyUpdate(string bld, string dld)
	{
		BirthLastDay.text = "Births: " + bld;
		DeathLastDay.text = "Deaths: " + dld;
	}

	public void InfoUpdate(string c, string g, string h)
    {
		Charity.text = "Charity:" + c;
		Gratitude.text = "Gratitude:" + g;
		Hate.text = "Hate:" + h;
    }
    public void UpdateFood()
    {
        GameManagerScript.Instance.UpdatePlayerFood();
        if (GameManagerScript.Instance.FoodPlayer <= BuildHouseManager.Instance.FoodRequired)
        {
            AddHouse.interactable = false;
            GameManagerScript.Instance.CloseBuildMenu();
        }
        else
        {
            AddHouse.interactable = true;
        }
        HouseFoodUpperLeft.text = "" + GameManagerScript.Instance.Houses[0].FoodStore;
        HouseFoodUpperRight.text = "" + GameManagerScript.Instance.Houses[1].FoodStore;
        HouseFoodDownwardLeft.text = "" + GameManagerScript.Instance.FoodPlayer;
        HouseFoodDownwardRight.text = "" + GameManagerScript.Instance.Houses[3].FoodStore;
        UpdatePeople();

}
    public void UpdatePeople()
    {

        HousePeopleUpperLeft.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.East && r.isActiveAndEnabled).ToList().Count;
        HousePeopleUpperRight.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.South && r.isActiveAndEnabled).ToList().Count;
        HousePeopleDownwardLeft.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.West && r.isActiveAndEnabled).ToList().Count;
        HousePeopleDownwardRight.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.North && r.isActiveAndEnabled).ToList().Count;
    }
        public void AddDay()
	{
		DayNumIterator++;
		DayNum.text = "Day: " + DayNumIterator;
        DayNightWheel.speed = 1f / GameManagerScript.Instance.DayTime;
        DayNightWheel.SetTrigger("ResetDay");
    }
    public void WinLoseState(int state)
    {
        WinLoseManager.Instance.WinLoseState(state);
    }
}
