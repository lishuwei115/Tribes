using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayLight : MonoBehaviour
{
    Image Filter; 
    public Gradient light;
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
        Filter.color = light.Evaluate((float)(60f - GameManagerScript.Instance.currentDayTime) / 60);
    }
}
