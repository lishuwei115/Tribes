using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCircleScript : MonoBehaviour
{
    public void OpenBuildingCircle()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            t.gameObject.SetActive(true);
        }
        BuildHouseManager.Instance.gameObject.SetActive(true);

    }
    


}
