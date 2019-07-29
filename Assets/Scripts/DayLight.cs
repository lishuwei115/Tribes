using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayLight : MonoBehaviour
{
    Image Filter; 
    public Gradient light;
    public SpriteRenderer Sky;
    // Start is called before the first frame update

    private void Awake()
    {
        Filter = GetComponent<Image>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color c = Color.white;
        float Nightime = GameManagerScript.Instance.DayTime - GameManagerScript.Instance.DayLightTime;
        c.a = GameManagerScript.Instance.currentDayTime < Nightime ?
           1- Mathf.Abs((float)((float)GameManagerScript.Instance.DayTime-(float)GameManagerScript.Instance.CurrentTimeMS - ((float)(Nightime / 2)) ) / (float)( Nightime/2 )) : 0;
        Sky.color = c;
        Filter.color = light.Evaluate((float)(GameManagerScript.Instance.CurrentTimeMS) / GameManagerScript.Instance.DayTime);
    }
}
