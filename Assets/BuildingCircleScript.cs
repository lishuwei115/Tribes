using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCircleScript : MonoBehaviour
{
    [Range(0, 1000)]
    public float BuildingRadiusMin = 10;
    [Range(0, 1000)]
    public float BuildingRadiusMax = 80;
    public Transform BuildingSpriteMin;
    public Transform BuildingSpriteMax;

    private void Awake()
    {
        BuildingSpriteMax = GetComponentInChildren<SpriteRenderer>().transform;
        BuildingSpriteMin = GetComponentInChildren<SpriteMask>().transform;
    }
    public void OpenBuildingCircle()
    {
        BuildingSpriteMax.gameObject.SetActive(true);
        BuildingSpriteMin.gameObject.SetActive(true);

        BuildHouseManager.Instance.gameObject.SetActive(true);
        WorldmapCamera.Instance.IsBuilding = true;
    }
    public void CloseBuildingCircle()
    {
        BuildingSpriteMax.gameObject.SetActive(false);
        BuildingSpriteMin.gameObject.SetActive(false);
        BuildHouseManager.Instance.gameObject.SetActive(false);

    }
    public void CreateHouse()
    {
        BuildingSpriteMax.gameObject.SetActive(false);
        BuildingSpriteMin.gameObject.SetActive(false);
        BuildHouseManager.Instance.gameObject.SetActive(false);
        WorldmapCamera.Instance.FinishBuilding();

    }


    internal void Initialize(float buildingRadiusMin, float buildingRadiusMax)
    {
        BuildingRadiusMin = buildingRadiusMin;
        BuildingRadiusMax = buildingRadiusMax;
        BuildingSpriteMax.transform.localScale = new Vector3(buildingRadiusMax / 3f, buildingRadiusMax / 3f, buildingRadiusMax / 3f);
        BuildingSpriteMin.transform.localScale = new Vector3(buildingRadiusMin / 3f, buildingRadiusMin / 3f, buildingRadiusMin / 3f);
    }
}
