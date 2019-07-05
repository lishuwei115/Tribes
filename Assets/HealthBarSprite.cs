using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarSprite : MonoBehaviour
{
    public float HPBase = 60;
    public float HPCurrent = 60;
    public SpriteRenderer HPBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateHP(float hp,float baseHp, HousesTypes house)
    {
        HPBase = baseHp;
        HPCurrent = hp;
        HPBar.transform.localScale =new Vector3( HPCurrent / HPBase, HPBar.transform.localScale.y, HPBar.transform.localScale.z);
        HPBar.color = SkinManager.Instance.GetSkinInfo(house).TribeColor;
    }
}
