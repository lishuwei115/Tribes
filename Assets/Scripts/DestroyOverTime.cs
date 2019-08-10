using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{

    public float Seconds = 3;
    public string Name = "Pointer";
    // Start is called before the first frame update
    void OnEnable()
    {
        //GameManagerScript.Instance.Pointer = this;
        CancelInvoke();
        Invoke("DestroyThis", Seconds);
    }

    void DestroyThis()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
