using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIFollowSprite : MonoBehaviour
{
    Transform Target = null;
    public Vector3 RelativePos = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
    }

    private void LateUpdate()
    {
        if (!GameManagerScript.Instance.Pause)
        {
            if (Target != null)
                transform.position = Camera.main.WorldToScreenPoint(Target.position + (Vector3)RelativePos);
            else
                Target = GameManagerScript.Instance.Houses.Where(r => r.IsPlayer).ToList()[0].transform;
            transform.localScale = new Vector3(1.2f - Camera.main.orthographicSize / WorldmapCamera.Instance.ZoomBounds[1], 1.2f - Camera.main.orthographicSize / WorldmapCamera.Instance.ZoomBounds[1], 1.2f - Camera.main.orthographicSize / WorldmapCamera.Instance.ZoomBounds[1]);

        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {


    }
}
