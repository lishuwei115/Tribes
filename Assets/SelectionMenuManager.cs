using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMenuManager : MonoBehaviour
{
    public static SelectionMenuManager Instance;
    public List<AudioClip> HouseSounds;
    SelectionMenuNavigation selectionNavigation;
    public int Index = 0;
    AudioSource SoundSource;
    bool wait = false;
    float Fill = 0;
    private void Awake()
    {
        Instance = this;
        SoundSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManagerScript.Instance.GameStatus = GameStateType.Intro;
        GameManagerScript.Instance.Pause = true;
        selectionNavigation = GetComponentInChildren<SelectionMenuNavigation>();



    }

    public void SelectionJoystic(Vector2 LeftJoystic)
    {
        if (!SplashScreenManager.Instance.SplashActive && GameManagerScript.Instance.Pause)
        {
            /* Fill += LeftJoystic.x;

             if (Fill > 10)
             {
                 IncreaseIndex();
                 Fill = 0;
             }
             else if (Fill < -10)
             {
                 DecreaseIndex();
                 Fill = 0;
             }*/
            if (LeftJoystic.x > 0.4f && !wait)
            {
                IncreaseIndex();
                wait = true;
                Invoke("KeepSelecting", 0.5f);
            }
            if (LeftJoystic.x < -0.4f && !wait)
            {
                DecreaseIndex();
                wait = true;
                Invoke("KeepSelecting", 0.5f);
            }
            if (Mathf.Abs(LeftJoystic.x) < .3f)
            {
                CancelInvoke();
                wait = false;
            }
        }

    }
    void KeepSelecting()
    {
        wait = false;
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
    public void TouchSelection(int i)
    {
        selectionNavigation.GoToIndex(i);
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
        SoundSource.PlayOneShot(HouseSounds[Index]);
    }
    public void DecreaseIndex()
    {

        selectionNavigation.DecreaseIndex();
        SoundSource.PlayOneShot(HouseSounds[Index]);

    }
}
