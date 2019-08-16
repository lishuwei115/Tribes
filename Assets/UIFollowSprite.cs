using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIFollowSprite : MonoBehaviour
{
    public static UIFollowSprite Instance;
    Transform Target = null;
    public Vector3 RelativePos = new Vector2();
    public MinusFoodValue[] Values;
    private void Awake()
    {
        Instance = this;
        Values = GetComponentsInChildren<MinusFoodValue>();
        foreach (MinusFoodValue item in Values)
        {
            item.gameObject.SetActive(false);
        }
    }
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
    public void ViewFoodConsumed(int value)
    {
        MinusFoodValue[] deactiveValues = Values.Where(r=>!r.gameObject.activeInHierarchy).ToArray();
        if(deactiveValues.Length>0)
        {
            deactiveValues[0].gameObject.SetActive(true);
            deactiveValues[0].DisplayValue(value);
        }
    }

}
