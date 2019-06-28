using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerScript : MonoBehaviour
{


    public Transform Prefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane p = new Plane(Vector3.up, Vector3.zero);
            float dist = 0;
            p.Raycast(ray, out dist);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 30);
            //point of input
            RaycastHit hit;
            bool found = Physics.Raycast(ray, out hit, 10000, layerMask);
            if (found)
            {
                GameManagerScript.Instance.MoveTribeTo(hit.point);
                Instantiate(Prefab,  hit.point,Prefab.rotation);
            }
        }





    }
}
