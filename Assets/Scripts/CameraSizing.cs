using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizing : MonoBehaviour
{
    public float Multiplier = 1;

    // Update is called once per frame
    void Update()
    {
        transform.localScale =new Vector3 (Screen.width*(Camera.main.orthographicSize/WorldmapCamera.Instance.ZoomBounds[1])* Multiplier / ((float)Screen.width / 1920f), Screen.height * (Camera.main.orthographicSize / WorldmapCamera.Instance.ZoomBounds[1])* Multiplier / ((float)Screen.width / 1920f), 1);
    }
}
