using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMenuNavigation : MonoBehaviour
{
    public Transform SelectionSquareRed;
    public Transform SelectionSquareYellow;
    public Transform SelectionSquareBlue;
    public Transform SelectionSquareGreen;

    public int Index = 0;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSelection();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void IncreaseIndex()
    {
        if (Index < 3)
        {
            Index++;
            UpdateSelection();
        }
    }
    public void DecreaseIndex()
    {
        if (Index > 0)
        {
            Index--;
            UpdateSelection();
        }
    }
    private void UpdateSelection()
    {
        SelectionMenuManager.Instance.Index = Index;
        SelectionSquareRed.gameObject.SetActive(false);
        SelectionSquareYellow.gameObject.SetActive(false);
        SelectionSquareBlue.gameObject.SetActive(false);
        SelectionSquareGreen.gameObject.SetActive(false);
        if (Index == 0)
        {
            SelectionSquareRed.gameObject.SetActive(true);
        }
        if (Index == 1)
        {
            SelectionSquareYellow.gameObject.SetActive(true);
        }
        if (Index == 2)
        {
            SelectionSquareBlue.gameObject.SetActive(true);
        }
        if (Index == 3)
        {
            SelectionSquareGreen.gameObject.SetActive(true);
        }
    }
}
