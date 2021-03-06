﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class WorldmapCamera : MonoBehaviour
{


    public static WorldmapCamera Instance;
    public Transform PrefabPointer;
    public Transform PrefabPointerBlue;
    public bool IsBuilding = false;
    public float PanSpeed = 20f;
    public float ZoomSpeedTouch = 0.1f;
    public float ZoomSpeedMouse = 0.5f;

    public float[] BoundsX = new float[] { -10f, 5f };
    float[] ProportionalBoundsX = new float[] { -10f, 5f };
    public float[] BoundsZ = new float[] { 0f, 18f };
    float[] ProportionalBoundsZ = new float[] { 0f, 18f };
    public float[] ZoomBounds = new float[] { 30f, 60f };
    public Vector2 DistanceBounds = new Vector2(1f, 90f);
    public float Val = 40;

    public Camera cam;
    public bool isMoving = false;
    public bool isZooming = false;
    public bool PanningWitouthTween = false;
    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only

    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only
    private float OffsetTime;
    private float LastTimeTouch;
    touchPhase touchPhase = touchPhase.none;
    public bool OnExplainMode = false;
    LayerMask layerMask;
    //public Transform PointOfLightTransform;
    //public Light DirLight;
    //float DirLightOffset;
    public bool IsTouch = false;
    //public Image logo;
    public delegate void CameraOnStartPos();
    public event CameraOnStartPos CameraOnStartPosEvent;
    public BuildHouseManager BuildManager;
    public MovementState MovState = MovementState.none;
    bool Initialized = false;
    [HideInInspector]
    public List<FakeButton> FakeButtons = new List<FakeButton>();


    private void Awake()
    {
        Instance = this;
        BuildManager = UnityEngine.Object.FindObjectOfType<BuildHouseManager>();
    }

    void Start()
    {
        InputManager_Riki.Instance.ButtonAPressedEvent += Instance_ButtonAPressedEvent;
        InputManager_Riki.Instance.ButtonBPressedEvent += Instance_ButtonBPressedEvent;
        InputManager_Riki.Instance.ButtonXPressedEvent += Instance_ButtonXPressedEvent;
        InputManager_Riki.Instance.ButtonYPressedEvent += Instance_ButtonYPressedEvent;
        InputManager_Riki.Instance.ButtonPlusPressedEvent += Instance_ButtonPlusPressedEvent;
        InputManager_Riki.Instance.RightJoystickUsedEvent += Instance_RightJoystickUsedEvent;
        InputManager_Riki.Instance.LeftJoystickUsedEvent += Instance_LeftJoystickUsedEvent;
        InputManager_Riki.Instance.ButtonLPressedEvent += Instance_ButtonLPressedEvent;
        InputManager_Riki.Instance.ButtonRPressedEvent += Instance_ButtonRPressedEvent;

        GameManagerScript.Instance.HumanSelected = HumanClass.Harvester;
        PressFakeButton("SelectFarmer", 0.1f);

        DeactiveFakeButton("SelectWarrior");

        layerMask = LayerMask.GetMask("Ground", "BlockTouch");
        cam = GetComponent<Camera>();
#if UNITY_ANDROID
        IsTouch = true;
#endif
#if UNITY_IOS
        IsTouch = true;
#endif
#if UNITY_EDITOR
        IsTouch = false;

#endif

    }

    private void Instance_ButtonRPressedEvent()
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)
            if (!GameManagerScript.Instance.Pause)
            {
                GameManagerScript.Instance.HumanSelected = HumanClass.Warrior;
                PressFakeButton("SelectWarrior", 0.1f);
                DeactiveFakeButton("SelectFarmer");
            }
    }

    private void Instance_ButtonLPressedEvent()
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)
            if (!GameManagerScript.Instance.Pause)
            {
                GameManagerScript.Instance.HumanSelected = HumanClass.Harvester;
                PressFakeButton("SelectFarmer", 0.1f);

                DeactiveFakeButton("SelectWarrior");
            }
    }

    private void Instance_LeftJoystickUsedEvent(Vector2 LeftJoystic)
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro && !GameManagerScript.Instance.Pause)
        {
            float relativeInc = cam.orthographicSize / ZoomBounds[1] / (ZoomBounds[1] / ZoomBounds[0]);
            Vector3 move = new Vector3(transform.position.x + LeftJoystic.x * (PanSpeed / 50) * (float)(relativeInc) * (Screen.width / Screen.height), DistanceBounds[0] + ((DistanceBounds[1] - DistanceBounds[0]) * (GetComponent<Camera>().orthographicSize / ZoomBounds[1])), transform.position.z + LeftJoystic.y * (PanSpeed / 50) * (float)(relativeInc) * (Screen.width / Screen.height));
            transform.position = move;
        }
        else
            SelectionMenuManager.Instance.SelectionJoystic(LeftJoystic);
    }

    private void Instance_RightJoystickUsedEvent(Vector2 joystick)
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)
            if (!GameManagerScript.Instance.Pause)
            {
                float f = GetComponent<Camera>().orthographicSize;
                float proportionalScaling = f / ZoomBounds[1];
                if ((f + (joystick.y * ((ZoomSpeedMouse) * proportionalScaling))) < ZoomBounds[1] && joystick.y < 0)
                {
                    GetComponent<Camera>().orthographicSize = (f - (joystick.y * ((ZoomSpeedMouse) * proportionalScaling)));
                }
                else if ((f + (joystick.y * ((ZoomSpeedMouse) * proportionalScaling))) > ZoomBounds[0] && joystick.y > 0)
                {
                    GetComponent<Camera>().orthographicSize = (f - (joystick.y * ((ZoomSpeedMouse) * proportionalScaling)));
                    //ZoomCamera((f + (joystick.y * ZoomSpeedMouse)) / ZoomBounds[1], ZoomSpeedMouse);
                }
                transform.position = new Vector3(transform.position.x, DistanceBounds[0] + ((DistanceBounds[1] - DistanceBounds[0]) * (GetComponent<Camera>().orthographicSize / ZoomBounds[1])), transform.position.z);
            }

    }

    private void Instance_ButtonPlusPressedEvent()
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)
        {
            GameManagerScript.Instance.Pause = GameManagerScript.Instance.Pause ? false : true;
            UIManagerScript.Instance.PauseMenu.SetBool("UIState", GameManagerScript.Instance.Pause);
            PressFakeButton("Pause", 0.1f);
        }


    }

    private void Instance_ButtonYPressedEvent()
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)
        {
            if (!GameManagerScript.Instance.Pause)
                GameManagerScript.Instance.Cultivate();
            PressFakeButton("Pray", 0.1f);

        }

    }

    private void Instance_ButtonXPressedEvent()
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)

            if (!GameManagerScript.Instance.Pause)
            {
                PressFakeButton("AddFighter", 0.1f);
                GameManagerScript.Instance.AddPlayerWarrior();
            }
    }

    private void Instance_ButtonBPressedEvent()
    {
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)
        {
            if (GameManagerScript.Instance.StateOfGame != GameState.Playing)
            {
                WinLoseManager.Instance.CloseWinLoseMenu();
                GameManagerScript.Instance.StateOfGame = GameState.Playing;
                PressFakeButton("CloseWin", 0.1f);

            }
            else
            if (!GameManagerScript.Instance.Pause)
            {
                PressFakeButton("AddFarmer", 0.1f);
                GameManagerScript.Instance.AddPlayerHarvester();
            }
            else
            {
                GameManagerScript.Instance.Pause = GameManagerScript.Instance.Pause ? false : true;
                UIManagerScript.Instance.PauseMenu.SetBool("UIState", GameManagerScript.Instance.Pause);
                PressFakeButton("Pause", 0.1f);
            }
        }
            

    }

    private void Instance_ButtonAPressedEvent()
    {
        if (GameManagerScript.Instance.StateOfGame != GameState.Playing)
        {
            RemoveEvents();
            PressFakeButton("Reset", 0);
            CancelInvoke();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        if (GameManagerScript.Instance.GameStatus != GameStateType.Intro)

            if (!GameManagerScript.Instance.Pause)
            {
                PressFakeButton("GoToPosition", 0.1f);
                TribeToPoint(new Vector2(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2));
            }
            else
            {
                RemoveEvents();
                PressFakeButton("Reset", 0);
                CancelInvoke();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        else if (GameManagerScript.Instance.Pause)
        {
            SelectionMenuManager.Instance.Selection();
            SplashScreenManager.Instance.CloseSplash();
            PressFakeButton("TribeSelection", 1);
            PressFakeButton("CloseSplashScreen", 1);
        }
    }
    public void RemoveEvents()
    {
        InputManager_Riki.Instance.ButtonAPressedEvent -= Instance_ButtonAPressedEvent;
        InputManager_Riki.Instance.ButtonBPressedEvent -= Instance_ButtonBPressedEvent;
        InputManager_Riki.Instance.ButtonXPressedEvent -= Instance_ButtonXPressedEvent;
        InputManager_Riki.Instance.ButtonYPressedEvent -= Instance_ButtonYPressedEvent;
        InputManager_Riki.Instance.ButtonPlusPressedEvent -= Instance_ButtonPlusPressedEvent;
        InputManager_Riki.Instance.RightJoystickUsedEvent -= Instance_RightJoystickUsedEvent;
        InputManager_Riki.Instance.LeftJoystickUsedEvent -= Instance_LeftJoystickUsedEvent;
    }
    public void ResetCam()
    {
        ZoomCamera(40, ZoomSpeedTouch);

        Transform playerHouse = GameManagerScript.Instance.Houses.Where(r => r.IsPlayer).ToList()[0].transform;
        //MoveToPos(new Vector3(-254, 90, -260), .1f);
        MoveToPos(new Vector3(playerHouse.position.x, 40, playerHouse.position.z), .1f);
    }
    public void PressFakeButton(string name, float seconds)
    {
        foreach (FakeButton f in FakeButtons)
        {
            if (f.ID == name)
            {
                f.Press(seconds);
            }
        }
    }
    public void DeactiveFakeButton(string name)
    {
        foreach (FakeButton f in FakeButtons)
        {
            if (f.ID == name)
            {
                f.DeactiveButton();
            }
        }
    }
    private void LateUpdate()
    {
        //Vector3 position = transform.position;
        //position.y = DistanceBounds[0] + ((DistanceBounds[1] - DistanceBounds[0]) * (GetComponent<Camera>().orthographicSize / ZoomBounds[1]));
        //transform.position = position;
        if (!GameManagerScript.Instance.Pause)
        {
            if (!Initialized)
            {
                ResetCam();
                Initialized = true;
            }
            if (IsTouch)
            {
                HandleTouch();

            }
            else
            {
                HandleMouse();

            }



            // Ensure the camera remains within bounds.
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(transform.position.x, ProportionalBoundsX[0], ProportionalBoundsX[1]);
            pos.z = Mathf.Clamp(transform.position.z, ProportionalBoundsZ[0], ProportionalBoundsZ[1]);
            transform.position = pos;
        }

    }

    public void FinishBuilding()
    {
        Invoke("IsNotBuilding", 0.1f);
    }
    public void IsNotBuilding()
    {
        IsBuilding = false;
    }
    void HandleMouse()
    {

        if (GameManagerScript.Instance.UIButtonOver)
        {
            //Debug.Log("there'a a button");
        }
        if (!GameManagerScript.Instance.UIButtonOver)
        {

            // On mouse down, capture it's position.
            // Otherwise, if the mouse is still down, pan the camera.
            if (Input.GetMouseButtonDown(0))
            {
                lastPanPosition = Input.mousePosition;
                OffsetTime = Time.time;
                MovState = MovementState.none;

            }
            else
            if (MovState == MovementState.none && Input.GetMouseButtonUp(1) && !IsBuilding /*&& Vector2.Distance(lastPanPosition, Input.mousePosition) < 40 * (Screen.width / 1920)*/)//&& Input.GetMouseButtonUp(0) && Time.time - OffsetTime < .1f
            {
                TribeToPoint();
                MovState = MovementState.none;
                lastPanPosition = Input.mousePosition;
                OffsetTime = Time.time;
            }
            else if (!Input.GetMouseButtonDown(0) && Input.GetMouseButton(0) && Vector2.Distance(lastPanPosition, Input.mousePosition) > 1 * (Screen.width / 1920))// - (Input.mousePosition.x + Input.mousePosition.y)) > 30 * (Screen.width / 1920)) 
            {
                PanCameraWithTween(Input.mousePosition);
                //PanCameraWithoutTween(Input.mousePosition);
            }

            else
            if (!Input.GetMouseButton(0))

            {
                lastPanPosition = Input.mousePosition;
                OffsetTime = Time.time;
                MovState = MovementState.none;
            }

            /*else if (Input.GetMouseButtonUp(0) && Mathf.Abs((lastPanPosition.x + lastPanPosition.y) - (Input.mousePosition.x + Input.mousePosition.y)) > 100f)
            {
                PanCameraWithoutTween(Input.mousePosition);

                //PanCameraWithTween(Input.mousePosition);
            }*/


            // Check for scrolling to zoom the camera
            if (MovState == MovementState.none)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                ZoomCamera(scroll, ZoomSpeedMouse);
            }

        }

    }
    void HandleTouch()
    {
        Touch touch;
        switch (Input.touchCount)
        {
            case 1: // Panning
                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began && !wasZoomingLastFrame)
                {
                    touchPhase = touchPhase.BeganPad;
                    PanningWitouthTween = false;
                    MovState = MovementState.none;
                    OffsetTime = Time.time;
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                }
                else if (MovState == MovementState.none && touch.phase == TouchPhase.Ended /*&& !PanningWitouthTween*/ &&
                    !wasZoomingLastFrame && !IsBuilding && Vector2.Distance(lastPanPosition, touch.position) < 3 * (Screen.width / 1920))
                {
                    touchPhase = touchPhase.EndePad;
                    MovState = MovementState.none;
                    lastPanPosition = touch.position;
                    OffsetTime = Time.time;
                    TribeToPoint();
                }
                else if (touch.fingerId == panFingerId && Vector2.Distance(lastPanPosition, touch.position) > 3 * (Screen.width / 1920) && !wasZoomingLastFrame &&
                         (touchPhase == touchPhase.BeganPad || touchPhase == touchPhase.MovePad))
                {
                    touchPhase = touchPhase.MovePad;
                    PanningWitouthTween = false;
                    PanCameraWithTween(touch.position);
                }


                break;

            case 2: // Zooming
                touchPhase = touchPhase.Zoom;
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                    MovState = MovementState.Zoom;

                }
                else
                {

                    touch = Input.GetTouch(0);
                    if (touch.fingerId == panFingerId)
                    {
                        panFingerId = -1;
                    }
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, ZoomSpeedTouch);

                    lastZoomPositions = newPositions;
                }
                break;

            default:
                lastPanPosition = transform.position;
                Invoke("SetZoomToFalse", 0.1f);
                break;
        }
    }
    public void TribeToPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane p = new Plane(Vector3.up, Vector3.zero);
        float dist = 0;
        p.Raycast(ray, out dist);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 30);
        //point of input
        RaycastHit hit;
        bool found = Physics.Raycast(ray, out hit, 10000, layerMask);
        if (found)
        {

            GameManagerScript.Instance.MoveTribeTo(hit.point, GameManagerScript.Instance.PlayerHouse);
            if (GameManagerScript.Instance.Pointer != null)
            {
                GameManagerScript.Instance.Pointer.gameObject.SetActive(false);
                GameManagerScript.Instance.Pointer.transform.position = hit.point;
                GameManagerScript.Instance.Pointer.gameObject.SetActive(true);
            }
            else
            {
                GameManagerScript.Instance.Pointer = (Instantiate(PrefabPointer, hit.point, PrefabPointer.rotation).GetComponent<DestroyOverTime>());

            }
        }

    }
    public void TribeToPoint(Vector2 Destination)
    {
        Ray ray = Camera.main.ScreenPointToRay(Destination);
        Plane p = new Plane(Vector3.up, Vector3.zero);
        float dist = 0;
        p.Raycast(ray, out dist);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 30);
        //point of input
        RaycastHit hit;
        bool found = Physics.Raycast(ray, out hit, 10000, layerMask);
        if (found)
        {

            GameManagerScript.Instance.MoveTribeTo(hit.point, GameManagerScript.Instance.PlayerHouse);
            if (GameManagerScript.Instance.HumanSelected == HumanClass.Warrior)
            {
                if (GameManagerScript.Instance.Pointer != null)
                {
                    GameManagerScript.Instance.Pointer.gameObject.SetActive(false);
                    GameManagerScript.Instance.Pointer.transform.position = hit.point;
                    GameManagerScript.Instance.Pointer.gameObject.SetActive(true);
                }
                else
                {
                    GameManagerScript.Instance.Pointer = (Instantiate(PrefabPointer, hit.point, PrefabPointer.rotation).GetComponent<DestroyOverTime>());

                }
            }
            else
            {
                if (GameManagerScript.Instance.PointerBlue != null)
                {
                    GameManagerScript.Instance.PointerBlue.gameObject.SetActive(false);
                    GameManagerScript.Instance.PointerBlue.transform.position = hit.point;
                    GameManagerScript.Instance.PointerBlue.gameObject.SetActive(true);
                }
                else
                {
                    GameManagerScript.Instance.PointerBlue = (Instantiate(PrefabPointerBlue, hit.point, PrefabPointerBlue.rotation).GetComponent<DestroyOverTime>());

                }
            }

        }

    }
    public void PanCameraWithoutTween(Vector3 newPanPosition)
    {
        if (!isMoving)
        {
            isMoving = true;
            // Determine how much to move the camera
            Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
            Vector3 move = new Vector3(transform.position.x + offset.x * (PanSpeed), transform.position.y, transform.position.z + offset.y * (PanSpeed));

            // Perform the movement
            //transform.Translate(move, Space.World);
            transform.position = move;


            // Ensure the camera remains within bounds.
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(transform.position.x, ProportionalBoundsX[0], ProportionalBoundsX[1]);
            pos.z = Mathf.Clamp(transform.position.z, ProportionalBoundsZ[0], ProportionalBoundsZ[1]);
            transform.position = pos;

            // Cache the position
            lastPanPosition = newPanPosition;
            LastTimeTouch = Time.time;
            Invoke("EnableMovement", 0.05f);

        }

    }

    public void EnableMovement()
    {
        isMoving = false;

    }



    public void PanCameraWithTween(Vector3 newPanPosition)
    {
        if (!isMoving)
        {
            MovState = MovementState.Drag;
            isMoving = true;
            // Determine how much to move the camera
            Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
            float relativeInc = cam.orthographicSize / ZoomBounds[1] / (ZoomBounds[1] / ZoomBounds[0]);
            Vector3 move = new Vector3(transform.position.x + offset.x * PanSpeed * (float)(relativeInc) * Screen.width / Screen.height, transform.position.y, transform.position.z + offset.y * PanSpeed * (float)(relativeInc));
            //MoveToPos(move, 1f/PanSpeed/(cam.orthographicSize* relativeInc));
            transform.position = move;
            isMoving = false;
            // Cache the position
            lastPanPosition = newPanPosition;
            LastTimeTouch = Time.time;
        }

    }

    public void ZoomCamera(float offset, float speed)
    {
        /*if (offset == 0)
        {
            return;
        }*/
        MovState = MovementState.Zoom;

        LastTimeTouch = Time.time;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
        ProportionalBoundsX = new float[] { -(ZoomBounds[1] - cam.orthographicSize) + BoundsX[0], (ZoomBounds[1] - cam.orthographicSize) + BoundsX[1] };
        ProportionalBoundsZ = new float[] { -(ZoomBounds[1] - cam.orthographicSize) + BoundsZ[0], (ZoomBounds[1] - cam.orthographicSize) + BoundsZ[1] };
        MovState = MovementState.none;

    }



    private void SetZoomToFalse()
    {
        wasZoomingLastFrame = false;

    }


    public void MoveToPos(Vector3 endPos, float time)
    {
        isMoving = true;
        this.gameObject.Tween("MoveCircle", transform.position, endPos, time, TweenScaleFunctions.SineEaseInOut, (t) =>
         {
             // progress
             Vector3 pos = t.CurrentValue;
             pos.x = Mathf.Clamp(t.CurrentValue.x, ProportionalBoundsX[0], ProportionalBoundsX[1]);
             pos.z = Mathf.Clamp(t.CurrentValue.z, ProportionalBoundsZ[0], ProportionalBoundsZ[1]);
             transform.position = pos;
         }, (t) =>
         {
             isMoving = false;

             if (time == 1.5f)
             {
                 //CameraOnStartPosEvent();
             }
         });
    }


    public void MovePointOfLightToPos(Vector3 endPos, float time)
    {
        /*PointOfLightTransform.gameObject.Tween("MovePointOfLight", PointOfLightTransform.position, endPos, time, TweenScaleFunctions.SineEaseInOut, (t) =>
        {
            // progress
			PointOfLightTransform.position = t.CurrentValue;
        }, (t) =>
        {
            
        });*/



    }

    public void Dimlight(bool onOff)
    {
        /*DirLight.gameObject.Tween("Dim",onOff ? DirLightOffset : 0, !onOff ? DirLightOffset : 0, 1, TweenScaleFunctions.SineEaseInOut, (t) =>
        {
            // progress
			DirLight.intensity = t.CurrentValue;
        }, (t) =>
        {

        });*/
    }
}



public enum MovementState
{
    none,
    Zoom,
    Drag
}


public enum touchPhase
{
    none,
    BeganPad,
    MovePad,
    EndePad,
    Zoom
}