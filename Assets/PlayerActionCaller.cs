using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActionCaller : MonoBehaviour
{
    public Color BreedOn = Color.white;
    public Color BreedOff = Color.white;
    public TextMeshProUGUI BreedOnOffText;
    public TextMeshProUGUI AttackOnOffText;
    public Color AttackOn;
    public Color AttackOff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CultivatePlayer()
    {
        GameManagerScript.Instance.Cultivate();
    }

    public void AddHouse()
    {
        GameManagerScript.Instance.AddHouse(GameManagerScript.Instance.PlayerHouse);
    }
    public void ToogleBreeding()
    {
        bool breeding = GameManagerScript.Instance.ToogleBreeding();
        GetComponent<Image>().color = breeding ? BreedOn : BreedOff;
        BreedOnOffText.text = breeding ? "is ON" : "is OFF";
    }
    public void ToogleAttack()
    {
        bool attacking = GameManagerScript.Instance.ToogleAttack();
        GetComponent<Image>().color = attacking ? AttackOn : AttackOff;
        AttackOnOffText.text = attacking ? "is ON" : "is OFF";
    }
}
