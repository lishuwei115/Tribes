using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMenuManager : MonoBehaviour
{
    public static SelectionMenuManager Instance;
    SelectionMenuNavigation selectionNavigation;
    public int Index = 0;
    float Fill = 0;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManagerScript.Instance.GameStatus = GameStateType.Intro;
        GameManagerScript.Instance.Pause = true;
        selectionNavigation = GetComponentInChildren<SelectionMenuNavigation>();

        

    }

    public  void SelectionJoystic(Vector2 LeftJoystic)
    {
        if (!SplashScreenManager.Instance.SplashActive && GameManagerScript.Instance.Pause)
        {
            Fill += LeftJoystic.x;

            if (Fill > 2)
            {
                IncreaseIndex();
                Fill = 0;
            }
            else if (Fill < -2)
            {
                DecreaseIndex();
                Fill = 0;
            }
        }
            
    }
    public void Selection()
    {
        if (!SplashScreenManager.Instance.SplashActive && GameManagerScript.Instance.Pause)
        {
            if (Index == 0)
            {
                GameManagerScript.Instance.PlayerHouse = HousesTypes.Red;
            }
            if (Index == 1)
            {
                GameManagerScript.Instance.PlayerHouse = HousesTypes.Yellow;
            }
            if (Index == 2)
            {
                GameManagerScript.Instance.PlayerHouse = HousesTypes.Blue;
            }
            if (Index == 3)
            {
                GameManagerScript.Instance.PlayerHouse = HousesTypes.Green;
            }
            UIManagerScript.Instance.ChangePlayer(GameManagerScript.Instance.PlayerHouse);
            GameManagerScript.Instance.Pause = false;
            GameManagerScript.Instance.StartGame();
            WorldmapCamera.Instance.ResetCam();
            //gameObject.SetActive(false);
            GetComponent<Animator>().SetBool("UIState", true);
        }
        
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void IncreaseIndex()
    {
        selectionNavigation.IncreaseIndex();
    }
    public void DecreaseIndex()
    {

        selectionNavigation.DecreaseIndex();
    }
}
