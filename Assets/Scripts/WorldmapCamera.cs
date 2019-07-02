using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class WorldmapCamera : MonoBehaviour {


	public static WorldmapCamera Instance;

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
	//public Transform PointOfLightTransform;
	//public Light DirLight;
	//float DirLightOffset;

	//public Image logo;
	public delegate void CameraOnStartPos();
	public event CameraOnStartPos CameraOnStartPosEvent;
	public CanvasScaler CanvasScalerWM;

	//public CanvasScaler CanvasScalerWMIndiscators;
    //public CanvasScaler LoadingCanvesScaler;
	// Use this for initialization

	private void Awake()
	{
		Instance = this;
    }

	void Start () {
		cam = GetComponent<Camera>();
		ResetCam();
        ProportionalBoundsX = new float[] { -(ZoomBounds[1] - cam.orthographicSize) + BoundsX[0], (ZoomBounds[1] - cam.orthographicSize) + BoundsX[1] };
        ProportionalBoundsZ = new float[] { -(ZoomBounds[1] - cam.orthographicSize) + BoundsZ[0], (ZoomBounds[1] - cam.orthographicSize) + BoundsZ[1] };
        //DirLightOffset = DirLight.intensity;
    }


    public void ResetCam()
	{
		//MoveToPos(new Vector3(0, 10, -23), 1.5f);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchSupported && !OnExplainMode)
        {
            HandleTouch();
        }
		else if (!Input.touchSupported && !OnExplainMode)
        {
            HandleMouse();
        }
		if(Time.time - LastTimeTouch > 5 && !isMoving && transform.position != new Vector3(0, 10, -23) && !OnExplainMode)
		{
			//cam.fieldOfView = 60;
			//MoveToPos(new Vector3(0, 10, -23),2);
		}
			

		// Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, ProportionalBoundsX[0], ProportionalBoundsX[1]);
		pos.z = Mathf.Clamp(transform.position.z, ProportionalBoundsZ[0], ProportionalBoundsZ[1]);
        transform.position = pos;
	}
    

	void HandleMouse()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
			OffsetTime = Time.time;
        }
		else if (Input.GetMouseButton(0) && (Time.time - OffsetTime) > 0.5f)
        {
            PanCameraWithoutTween(Input.mousePosition);
        }
		else if (Input.GetMouseButtonUp(0))
        {
			PanCameraWithTween(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

	public void PanCameraWithoutTween(Vector3 newPanPosition)
    {
        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
		Vector3 move = new Vector3(offset.x * PanSpeed,offset.y, offset.z * PanSpeed);

        // Perform the movement
        transform.Translate(move, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, ProportionalBoundsX[0], ProportionalBoundsX[1]);
		pos.z = Mathf.Clamp(transform.position.z, ProportionalBoundsZ[0], ProportionalBoundsZ[1]);
        transform.position = pos;

        // Cache the position
        lastPanPosition = newPanPosition;
		LastTimeTouch = Time.time;
    }





	public void PanCameraWithTween(Vector3 newPanPosition)
    {
		if(!isMoving)
		{
			isMoving = true;
			// Determine how much to move the camera
            Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
			Vector3 move = new Vector3(transform.position.x + offset.x * Val, transform.position.y , transform.position.z + offset.y * Val);
			MoveToPos(move, 0.5f);
           
            
            // Cache the position
            lastPanPosition = newPanPosition;
			LastTimeTouch = Time.time;
		}
       
    }

    public void ZoomCamera(float offset, float speed)
    {
        if (offset == 0)
        {
            return;
        }
        LastTimeTouch = Time.time;
        cam.orthographicSize = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
        ProportionalBoundsX = new float[] { -(ZoomBounds[1] - cam.orthographicSize ) + BoundsX[0], (ZoomBounds[1] - cam.orthographicSize ) + BoundsX[1] };
        ProportionalBoundsZ = new float[] { -(ZoomBounds[1] - cam.orthographicSize ) + BoundsZ[0], (ZoomBounds[1] - cam.orthographicSize ) + BoundsZ[1] };

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
				else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved && (Time.time - OffsetTime) > 0.15f && !wasZoomingLastFrame && 
				         (touchPhase == touchPhase.BeganPad || touchPhase == touchPhase.MovePad))
                {
					touchPhase = touchPhase.MovePad;
					PanningWitouthTween = true;
					PanCameraWithoutTween(touch.position);
                }
				else if(touch.phase == TouchPhase.Ended && !PanningWitouthTween && !wasZoomingLastFrame)
				{
					touchPhase = touchPhase.EndePad;
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
					if(touch.fingerId == panFingerId)
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
		this.gameObject.Tween("MoveCircle", transform.position, endPos,time, TweenScaleFunctions.SineEaseInOut, (t) =>
        {
            // progress
            Vector3 pos = t.CurrentValue;
            pos.x = Mathf.Clamp(t.CurrentValue.x, ProportionalBoundsX[0], ProportionalBoundsX[1]);
            pos.z = Mathf.Clamp(t.CurrentValue.z, ProportionalBoundsZ[0], ProportionalBoundsZ[1]);
            transform.position = pos;
		}, (t) =>
        {
            isMoving = false;

            if(time == 1.5f)
			{
				CameraOnStartPosEvent();
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






public enum touchPhase
{
	none,
	BeganPad,
    MovePad,
    EndePad,
    Zoom
}