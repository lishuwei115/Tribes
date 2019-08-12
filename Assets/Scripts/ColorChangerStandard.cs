using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangerStandard : MonoBehaviour
{
    public Gradient light;
    public SpriteRenderer Light;
    public bool Transparent = true;
    // Start is called before the first frame update

    private void Awake()
    {
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Color c = Color.white;
        float Nightime = GameManagerScript.Instance.DayTime - GameManagerScript.Instance.DayLightTime;
        if (Transparent)
        {
            c.a = GameManagerScript.Instance.currentDayTime < Nightime ?
           1 - Mathf.Abs((float)((float)GameManagerScript.Instance.DayTime - (float)GameManagerScript.Instance.CurrentTimeMS - ((float)(Nightime / 2))) / (float)(Nightime / 2)) : 0;
        }
        //Light.color = c;

        Light.color = light.Evaluate((float)(GameManagerScript.Instance.CurrentTimeMS) / GameManagerScript.Instance.DayTime);
    }
}
