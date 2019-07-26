using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{

    public float Seconds = 3;
    public string Name = "Pointer";
    // Start is called before the first frame update
    void Start()
    {
        if (GameManagerScript.Instance.Pointer)
            Destroy(GameManagerScript.Instance.Pointer.gameObject);
        GameManagerScript.Instance.Pointer = this;
        Invoke("DestroyThis", Seconds);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
