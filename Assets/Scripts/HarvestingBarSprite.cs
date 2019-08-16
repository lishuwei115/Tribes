using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingBarSprite : MonoBehaviour
{
    public float HarvestingBase = 60;
    public float HarvestingRemaining = 60;
    public SpriteRenderer HarvestingBar;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateHarvest(float harvestNow, float harvestBase, HousesTypes house)
    {
        HarvestingBase = harvestBase;
        HarvestingRemaining = harvestNow;
        HarvestingBar.transform.localScale = new Vector3(HarvestingRemaining / HarvestingBase, HarvestingBar.transform.localScale.y, HarvestingBar.transform.localScale.z);
        //HarvestingBar.color = SkinManager.Instance.GetSkinInfo(house).TribeColor;
    }
}
