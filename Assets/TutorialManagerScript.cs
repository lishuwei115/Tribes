using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManagerScript : MonoBehaviour
{
    public TutorialManagerScript Instance;
    public TutorialPage[] TutorialPages;
    public int TutorialIndex = 0;
    private void Awake()
    {
        Instance = this;
        TutorialPages = GetComponentsInChildren<TutorialPage>();
        GoToFirstPage();
    }

    public void GoToFirstPage()
    {
        foreach (TutorialPage t in TutorialPages)
        {
            t.GetComponent<Animator>().SetBool("UIState", false);
        }
        TutorialIndex = 0;
        TutorialPages[0].GetComponent<Animator>().SetBool("UIState", true);
    }

    public void NextPage()
    {
        if (TutorialIndex + 1 < TutorialPages.Length)
        {
            TutorialPages[TutorialIndex].GetComponent<Animator>().SetBool("UIState",false);
            TutorialIndex++;
            TutorialPages[TutorialIndex].GetComponent<Animator>().SetBool("UIState", true);
        }
    }
    public void PreviousPage()
    {
        if (TutorialIndex > 0)
        {
            TutorialPages[TutorialIndex].GetComponent<Animator>().SetBool("UIState", false);
            TutorialIndex--;
            TutorialPages[TutorialIndex].GetComponent<Animator>().SetBool("UIState", true);
        }
    }
}