using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionCaller : MonoBehaviour
{
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
}
