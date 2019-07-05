using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;


    public List<Skin> HousesSkin = new List<Skin>();

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetSkinFromHouse(HousesTypes house)
    {
        //return the prefab found in the list of skins corrisponding only to the house type
        return HousesSkin.Where(a =>a.House == house).ToList()[0].SkinPrefab;
    }
    public Skin GetSkinInfo(HousesTypes house)
    {
        //return the prefab found in the list of skins corrisponding only to the house type
        return HousesSkin.Where(a => a.House == house).ToList()[0];
    }

}

[System.Serializable]
public class Skin
{
    public HousesTypes House = HousesTypes.East;
    public Transform SkinPrefab;
    public Color TribeColor = Color.red;
}
