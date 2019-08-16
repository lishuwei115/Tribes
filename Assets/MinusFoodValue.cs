using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MinusFoodValue : MonoBehaviour
{
    public int Value = 1;

    public TextMeshProUGUI ValueText;
    public TextMeshProUGUI ValueTextShadow;

    public void DisplayValue(int value)
    {
        Value = value;
        ValueText.text = "" + Value;
        ValueTextShadow.text = "" + Value;
        GetComponent<Animator>().SetBool("UIState", true);
    }
    public void DeactiveSelf()
    {
        gameObject.SetActive(false);
    }
}
