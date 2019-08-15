using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AlphaIfAvaiable : MonoBehaviour
{
    public Color StartColor;
    public Color EndColor;

    public ButtonFunction function = ButtonFunction.AddWarrior;
    Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManagerScript.Instance.Pause)
            switch (function)
            {
                case ButtonFunction.AddWarrior:
                    if (GameManagerScript.Instance.Houses.Where(r => r.IsPlayer).ToList()[0].FoodStore < GameManagerScript.Instance.FoodRequiredWarrior)
                        image.color = EndColor;
                    else
                        image.color = StartColor;
                    break;
                case ButtonFunction.AddFarmer:
                    if (GameManagerScript.Instance.Houses.Where(r => r.IsPlayer).ToList()[0].FoodStore < GameManagerScript.Instance.FoodRequiredHarvester)
                        image.color = EndColor;
                    else
                        image.color = StartColor;
                    break;
                default:
                    break;
            }
    }
}
public enum ButtonFunction
{
    AddWarrior,
    AddFarmer
}