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
    public Transform GetSkinFromHouse(HousesTypes house, HumanClass h)
    {
        //return the prefab found in the list of skins corrisponding only to the house type
        switch (h)
        {
            case HumanClass.Standard:
                return HousesSkin.Where(a => a.House == house).ToList()[0].SkinPrefab;
            case HumanClass.Harvester:
                return HousesSkin.Where(a => a.House == house).ToList()[0].HarvestPrefab;
            case HumanClass.Warrior:
                return HousesSkin.Where(a => a.House == house).ToList()[0].WarriorPrefab;
        }
        return null;
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
    public HousesTypes House = HousesTypes.Red;
    public Transform SkinPrefab;
    public Transform HarvestPrefab;
    public Transform WarriorPrefab;
    
    public Color TribeColor = Color.red;
    public Transform HousePrefab;
    public Transform Flower;
    public AudioClip Attack;
    public AudioClip Farm;
    public AudioClip Death;
    public AudioClip Born;
    public AudioClip Worship;
}
