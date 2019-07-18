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
        rect = new Rect(GetComponent<RectTransform>().position.x, GetComponent<RectTransform>().position.y, rect.width, rect.height);
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
