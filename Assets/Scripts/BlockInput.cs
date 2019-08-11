using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockInput : MonoBehaviour
{
    public bool UIButtonOver;
    private void Update()
    {

        Rect rect = GetComponent<RectTransform>().rect;
        rect = new Rect(GetComponent<RectTransform>().position.x+(rect.width* GetComponent<RectTransform>().pivot.x), GetComponent<RectTransform>().position.y + (rect.height * GetComponent<RectTransform>().pivot.y), rect.width*(float)Screen.width/1920f, rect.height * (float)Screen.width / 1920f);
        if (rect.Contains(Input.mousePosition))
        {
            UIButtonOver = true;
        }
        else
        {
            UIButtonOver = false;
        }
    }
}
