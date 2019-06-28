using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{

    public float Seconds = 3;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyThis", Seconds);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
