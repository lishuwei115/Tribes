using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class WorldmapCamera : MonoBehaviour
{


    public static WorldmapCamera Instance;
    public Transform PrefabPointer;
    public bool IsBuilding = false;
    public float PanSpeed = 20f;
    public float ZoomSpeedTouch = 0.1f;
    public float ZoomSpeedMouse = 0.5f;

    public float[] BoundsX = new float[] { -10f, 5f };
    float[] ProportionalBoundsX = new float[] { -10f, 5f };
    public float[] BoundsZ = new float[] { 0f, 18f };
    float[] ProportionalBoundsZ = new float[] { 0f, 18f };
    public float[] ZoomBounds = new float[] { 30f, 60f };
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
    private void Awake()
    {
        Instance = this;
        BuildManager = UnityEngine.Object.FindObjectOfType < BuildHouseManager>();
    }

    void Start()
    {
        layerMask = LayerMask.GetMask("Ground", "BlockTouch");
        cam = GetComponent<Camera>();
        ResetCam();
#if UNITY_ANDROID
        IsTouch = true;
#endif
#if UNITY_EDITOR
        IsTouch = false;

#endif

    }


    public void ResetCam()
    {
        MoveToPos(new Vector3(-254, 90, -260), .1f);
    }

    private void FixedUpdate()
    {
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
    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("there'a a button");
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
            if (MovState == MovementState.none && Input.GetMouseButtonUp(0)&&!IsBuilding && Vector2.Distance(lastPanPosition, Input.mousePosition) < 30)//&& Input.GetMouseButtonUp(0) && Time.time - OffsetTime < .1f
            {
                TribeToPoint();
                MovState = MovementState.none;
                lastPanPosition = Input.mousePosition;
                OffsetTime = Time.time;
            }
            else if (!Input.GetMouseButtonDown(0) && Input.GetMouseButton(0) && Vector2.Distance(lastPanPosition, Input.mousePosition) > 30)// - (Input.mousePosition.x + Input.mousePosition.y)) > 30 * (Screen.width / 1920)) 
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

    public void TribeToPoint()
    {

          

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane p = new Plane(Vector3.up, Vector3.zero);
        float dist = 0;
        p.Raycast(ray, out dist);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 30);
        //point of input
        RaycastHit hit;
        bool found = Physics.Raycast(ray, out hit, 10000, layerMask);
        if (found)
        {
            GameManagerScript.Instance.MoveTribeTo(hit.point, GameManagerScript.Instance.PlayerHouse);
            Instantiate(PrefabPointer, hit.point, PrefabPointer.rotation);
        }

    }
    public void PanCameraWithoutTween(Vector3 newPanPosition)
    {
        if (!isMoving)
        {
            isMoving = true;
            // Determine how much to move the camera
            Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
            Vector3 move = new Vector3(transform.position.x + offset.x * (PanSpeed ), transform.position.y,transform.position.z + offset.y * (PanSpeed));

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
            Vector3 move = new Vector3(transform.position.x + offset.x * Val* (float)(cam.orthographicSize / ZoomBounds[1]), transform.position.y, transform.position.z + offset.y * Val* (float)(cam.orthographicSize / ZoomBounds[1]));
            MoveToPos(move, 1f/PanSpeed/cam.orthographicSize);


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
                    OffsetTime = Time.time;
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                }
                else if (touch.phase == TouchPhase.Ended /*&& !PanningWitouthTween*/ && !wasZoomingLastFrame)
                {
                    /*if( Mathf.Abs((lastPanPosition.x + lastPanPosition.y) - (touch.position.x + touch.position.y)) > 100)
                    {
                        touchPhase = touchPhase.EndePad;
                        PanCameraWithTween(touch.position);
                    }
                    else*/
                    if (!BuildManager.isActiveAndEnabled && Mathf.Abs((lastPanPosition.x + lastPanPosition.y) - (Input.mousePosition.x + Input.mousePosition.y)) < .1f*(cam.orthographicSize/ZoomBounds[1]))
                    {
                        touchPhase = touchPhase.EndePad;
                        TribeToPoint();
                    }
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved && Mathf.Abs((lastPanPosition.x + lastPanPosition.y) - (touch.position.x + touch.position.y)) > 0.1f* (float)(cam.orthographicSize / ZoomBounds[1])/*(Time.time - OffsetTime) > 0.01f*/ && !wasZoomingLastFrame &&
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