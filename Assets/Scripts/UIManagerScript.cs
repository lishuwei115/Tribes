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
    public Animator PauseMenu;
    public Button AddHouse;
    public int DayNumIterator = 0;
    public CardFactionScript[] CardsPositioning;
    public Vector3 PlayerPosition = new Vector3();
    public CardFactionScript PlayerCardBlue;
    public CardFactionScript PlayerCardRed;
    public CardFactionScript PlayerCardGreen;
    public CardFactionScript PlayerCardYellow;
    public float RedFood = 0;
    public float GreenFood = 0;
    public float YellowFood = 0;
    public float BlueFood = 0;

    private void Awake()
    {
        Instance = this;
        PlayerPosition = CardsPositioning[0].transform.position;
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
        CardsPositioning[0].SetPlayer(false);
        Vector2 help;
        foreach (CardFactionScript c in CardsPositioning)
        {
            if (h == c.house)
            {
                c.SetPlayer(true);
                help = c.transform.position;
                c.transform.position = CardsPositioning[0].transform.position;
                CardsPositioning[0].transform.position = help;
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

    public void InfoDailyUpdate( )
    {
        BirthLastDay.text = "total Births: " + (GameManagerScript.Instance.HumansList.Count - (GameManagerScript.Instance.Humans * 4));
        DeathLastDay.text = "total Deaths: " + GameManagerScript.Instance.HumansList.Where(r=>!r.isActiveAndEnabled).ToList().Count;
        NumberOfEntity.text = "total Population: " + (GameManagerScript.Instance.HumansList.Where(r=>r.isActiveAndEnabled).ToList().Count);
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
            List<HumanBeingScript> harvester = h.Humans.Where(r => r.Hp > 0 && r.HumanJob == HumanClass.Harvester).ToList();
            List<HumanBeingScript> warrior = h.Humans.Where(r => r.Hp > 0 && r.HumanJob == HumanClass.Warrior).ToList();
            float foodRequired = (harvester.Count * GameManagerScript.Instance.FoodRequiredHarvester) + (warrior.Count * GameManagerScript.Instance.FoodRequiredWarrior);

            switch (h.HouseType)
            {
                case HousesTypes.Green:
                    GreenFood += h.FoodStore;
                    PlayerCardGreen.Food.text = "" + (h.FoodStore - foodRequired);// + "\n(-" + foodRequired + ")";
                    break;
                case HousesTypes.Yellow:
                    YellowFood += h.FoodStore;
                    PlayerCardYellow.Food.text = "" + (h.FoodStore - foodRequired);// + "\n(-" + foodRequired + ")";
                    break;
                case HousesTypes.Red:
                    RedFood += h.FoodStore;
                    PlayerCardRed.Food.text = "" + (h.FoodStore - foodRequired);// + "\n(-" + foodRequired + ")";
                    break;
                case HousesTypes.Blue:
                    BlueFood += h.FoodStore;
                    PlayerCardBlue.Food.text = "" + (h.FoodStore - foodRequired);// + "\n(-" + foodRequired + ")";
                    break;
            }
        }
        UpdatePeople();

    }
    public void UpdatePeople()
    {
        PlayerCardRed.Warrior.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Red && r.isActiveAndEnabled && r.HumanJob == HumanClass.Warrior).ToList().Count;
        PlayerCardYellow.Warrior.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Yellow && r.isActiveAndEnabled && r.HumanJob == HumanClass.Warrior).ToList().Count;
        PlayerCardBlue.Warrior.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Blue && r.isActiveAndEnabled && r.HumanJob == HumanClass.Warrior).ToList().Count;
        PlayerCardGreen.Warrior.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Green && r.isActiveAndEnabled && r.HumanJob == HumanClass.Warrior).ToList().Count;

        PlayerCardRed.Farmer.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Red && r.isActiveAndEnabled &&r.HumanJob==HumanClass.Harvester).ToList().Count;
        PlayerCardYellow.Farmer.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Yellow && r.isActiveAndEnabled && r.HumanJob == HumanClass.Harvester).ToList().Count;
        PlayerCardBlue.Farmer.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Blue && r.isActiveAndEnabled && r.HumanJob == HumanClass.Harvester).ToList().Count;
        PlayerCardGreen.Farmer.text = "" + GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Green && r.isActiveAndEnabled && r.HumanJob == HumanClass.Harvester).ToList().Count;

        //PlayerCardRed.Warrior.text += "(" + (GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Red && r.isActiveAndEnabled && r.HumanJob == HumanClass.Warrior).ToList().Count*GameManagerScript.Instance.FoodRequiredWarrior)+")";
        //PlayerCardYellow.Warrior.text += "(" + (GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Yellow && r.isActiveAndEnabled && r.HumanJob == HumanClass.Warrior).ToList().Count*GameManagerScript.Instance.FoodRequiredWarrior)+")";
        //PlayerCardBlue.Warrior.text += "(" +( GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Blue && r.isActiveAndEnabled && r.HumanJob == HumanClass.Warrior).ToList().Count*GameManagerScript.Instance.FoodRequiredWarrior)+")";
        //PlayerCardGreen.Warrior.text += "(" + (GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Green && r.isActiveAndEnabled && r.HumanJob == HumanClass.Warrior).ToList().Count*GameManagerScript.Instance.FoodRequiredWarrior)+")";

        //PlayerCardRed.Farmer.text += "(" +( GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Red && r.isActiveAndEnabled && r.HumanJob == HumanClass.Harvester).ToList().Count*GameManagerScript.Instance.FoodRequiredHarvester)+")";
        //PlayerCardYellow.Farmer.text += "(" +( GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Yellow && r.isActiveAndEnabled && r.HumanJob == HumanClass.Harvester).ToList().Count*GameManagerScript.Instance.FoodRequiredHarvester) +")";
        //PlayerCardBlue.Farmer.text += "(" +( GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Blue && r.isActiveAndEnabled && r.HumanJob == HumanClass.Harvester).ToList().Count*GameManagerScript.Instance.FoodRequiredHarvester) +")";
        //PlayerCardGreen.Farmer.text += "(" + (GameManagerScript.Instance.HumansList.Where(r => r.HouseType == HousesTypes.Green && r.isActiveAndEnabled && r.HumanJob == HumanClass.Harvester).ToList().Count*GameManagerScript.Instance.FoodRequiredHarvester) +")";
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
