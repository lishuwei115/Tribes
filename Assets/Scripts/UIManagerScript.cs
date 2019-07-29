using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManagerScript : MonoBehaviour
{

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
    public TextMeshProUGUI HouseFoodBlu;
    public TextMeshProUGUI HouseFoodYellow;
    public TextMeshProUGUI HouseFoodRed;
    public TextMeshProUGUI HouseFoodGreen;
    public TextMeshProUGUI HousePeopleUpperLeft;
    public TextMeshProUGUI HousePeopleUpperRight;
    public TextMeshProUGUI HousePeopleDownwardLeft;
    public TextMeshProUGUI HousePeopleDownwardRight;
    public Button AddHouse;
    public int DayNumIterator = 0;
    public CardFactionScript[] PlayerCards;
    public float RedFood = 0;
    public float GreenFood = 0;
    public float YellowFood = 0;
    public float BlueFood = 0;
    private void Awake()
    {
        Instance = this;
    }


    // Use this for initialization
    void Start()
    {
        DayNightWheel.speed = 0;

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePeople();
        UpdateFood();
        if (GameManagerScript.Instance.Pause)
        {
            DayNightWheel.speed = 0;
        }
        else if (DayNumIterator > 0)
        {
            DayNightWheel.speed = 1f / GameManagerScript.Instance.DayTime;
        }
    }
    public void ChangePlayer(HousesTypes h)
    {
        PlayerCards[0].SetPlayer(false);
        Vector2 help;
        foreach (CardFactionScript c in PlayerCards)
        {
            if (h == c.house)
            {
                c.SetPlayer(true);
                help = c.transform.position;
                c.transform.position = PlayerCards[0].transform.position;
                PlayerCards[0].transform.position = help;
            }
            else
            {
                c.SetPlayer(false);
            }
            
        }
        
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
        GreenFood = 0;
        BlueFood = 0;
        RedFood = 0;
        YellowFood = 0;
        foreach (HouseScript h in GameManagerScript.Instance.Houses)
        {
            switch (h.HouseType)
            {
                case HousesTypes.North:
                    GreenFood += h.FoodStore;
                    break;
                case HousesTypes.South:
                    YellowFood += h.FoodStore;
                    break;
                case HousesTypes.East:
                    RedFood += h.FoodStore;
                    break;
                case HousesTypes.West:
                    BlueFood += h.FoodStore;
                    break;
            }
        }
        HouseFoodBlu.text = "" + BlueFood;
        HouseFoodYellow.text = "" + YellowFood;
        HouseFoodRed.text = "" + RedFood;
        //HouseFoodDownwardLeft.text = "" + GameManagerScript.Instance.FoodPlayer;
        HouseFoodGreen.text = "" + GreenFood;
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
