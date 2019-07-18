using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveWithMouse : MonoBehaviour
{
    public bool move = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            GetComponent<RectTransform>().position = Input.mousePosition;

        }
    }

    public void ClickThis()
    {
        move = true;
    }
    public void ClickStop()
    {
        move = false;
    }
    

}
