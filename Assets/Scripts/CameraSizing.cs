using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale =new Vector3 (Screen.width*(Camera.main.orthographicSize/WorldmapCamera.Instance.ZoomBounds[1]), Screen.height * (Camera.main.orthographicSize / WorldmapCamera.Instance.ZoomBounds[1]), 1);
    }
}
