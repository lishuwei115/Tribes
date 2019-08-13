using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeButton : MonoBehaviour
{
    public Color Base = Color.white;
    public Color Pressed = Color.red;
    public Color Deactive = Color.gray;
    public string ID = "Button0";
    // Start is called before the first frame update
    void Start()
    {
        WorldmapCamera.Instance.FakeButtons.Add(this);
    }
    public void DeactiveButton()
    {
        GetComponent<Image>().color = Deactive;
    }
    public void Press(float seconds)
    {
        GetComponent<Image>().color = Pressed;
        Invoke("Release", seconds);
    }
    public void Release()
    {
        GetComponent<Image>().color = Base;
    }
}
