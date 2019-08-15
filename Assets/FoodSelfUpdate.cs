using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FoodSelfUpdate : MonoBehaviour
{

    TextMeshProUGUI me;
    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManagerScript.Instance.Pause)
        {
            HouseScript h = GameManagerScript.Instance.Houses.Where(r => r.IsPlayer).ToList()[0];
            List<HumanBeingScript> harvester = h.Humans.Where(r => r.Hp > 0 && r.HumanJob == HumanClass.Harvester).ToList();
            List<HumanBeingScript> warrior = h.Humans.Where(r => r.Hp > 0 && r.HumanJob == HumanClass.Warrior).ToList();
            float foodRequired = (harvester.Count * GameManagerScript.Instance.FoodRequiredHarvester) + (warrior.Count * GameManagerScript.Instance.FoodRequiredWarrior);

            switch (h.HouseType)
            {
                case HousesTypes.Green:
                    me.text = "Store: " + h.FoodStore + "\nunits: -" + foodRequired + "";
                    break;
                case HousesTypes.Yellow:
                    me.text = "Store: " + h.FoodStore + "\nunits: -" + foodRequired + "";
                    break;
                case HousesTypes.Red:
                    me.text = "Store: " + h.FoodStore + "\nunits: -" + foodRequired + "";
                    break;
                case HousesTypes.Blue:
                    me.text = "Store: " + h.FoodStore + "\nunits: -" + foodRequired + "";
                    break;
            }
        }
        
    }
}

