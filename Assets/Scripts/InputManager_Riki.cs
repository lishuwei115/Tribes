using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_SWITCH
using Rewired.Platforms.Switch;
#endif
public class InputManager_Riki : MonoBehaviour
{
    public delegate void ButtonAPressed();
    public event ButtonAPressed ButtonAPressedEvent;
    public delegate void ButtonBPressed();
    public event ButtonBPressed ButtonBPressedEvent;
    public delegate void ButtonXPressed();
    public event ButtonXPressed ButtonXPressedEvent;
    public delegate void ButtonYPressed();
    public event ButtonYPressed ButtonYPressedEvent;
    public delegate void ButtonUpPressed();
    public event ButtonUpPressed ButtonUpPressedEvent;
    public delegate void ButtonDownPressed();
    public event ButtonDownPressed ButtonDownPressedEvent;
    public delegate void ButtonLeftPressed();
    public event ButtonLeftPressed ButtonLeftPressedEvent;
    public delegate void ButtonRightPressed();
    public event ButtonRightPressed ButtonRightPressedEvent;
    public delegate void ButtonRPressed();
    public event ButtonRPressed ButtonRPressedEvent;
    public delegate void ButtonZRPressed();
    public event ButtonZRPressed ButtonZRPressedEvent;
    public delegate void ButtonLPressed();
    public event ButtonLPressed ButtonLPressedEvent;
    public delegate void ButtonZLPressed();
    public event ButtonZLPressed ButtonZLPressedEvent;
    public delegate void LeftJoystickUsed();
    public event LeftJoystickUsed LeftJoystickUsedEvent;
    public delegate void RightJoystickUsed();
    public event RightJoystickUsed RightJoystickUsedEvent;



    public static InputManager_Riki Instance;

    public int playerId = 0;
    private Player player; // The Rewired Player
    public Vector2 LeftJoystic, RightJoystic;
    void Awake()
    {
        Instance = this;
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
#if UNITY_SWITCH
        if(player.GetButtonDown("A"))
        {
            Debug.Log(player.GetButtonDown("A"));
            if(ButtonAPressedEvent != null)
            {
                ButtonAPressedEvent();
            }
        }
        if (player.GetButtonDown("B"))
        {
            Debug.Log(player.GetButtonDown("B"));
            if (ButtonBPressedEvent != null)
            {
                ButtonBPressedEvent();
            }
        }
        if (player.GetButtonDown("X"))
        {
            Debug.Log(player.GetButtonDown("X"));
            if (ButtonXPressedEvent != null)
            {
                ButtonXPressedEvent();
            }
        }
        if (player.GetButtonDown("Y"))
        {
            Debug.Log(player.GetButtonDown("Y"));
            if (ButtonYPressedEvent != null)
            {
                ButtonYPressedEvent();
            }
        }
        if (player.GetButtonDown("R"))
        {
            Debug.Log(player.GetButtonDown("R"));
            if (ButtonRPressedEvent != null)
            {
                ButtonRPressedEvent();
            }
        }
        if (player.GetButtonDown("ZR"))
        {
            Debug.Log(player.GetButtonDown("ZR"));
            if (ButtonZRPressedEvent != null)
            {
                ButtonZRPressedEvent();
            }
        }
        if (player.GetButtonDown("L"))
        {
            Debug.Log(player.GetButtonDown("L"));
            if (ButtonLPressedEvent != null)
            {
                ButtonLPressedEvent();
            }
        }
        if (player.GetButtonDown("ZL"))
        {
            Debug.Log(player.GetButtonDown("ZL"));
            if (ButtonZLPressedEvent != null)
            {
                ButtonZLPressedEvent();
            }
        }
        if (player.GetButtonDown("Right"))
        {
            Debug.Log(player.GetButtonDown("Right"));
            if (ButtonRightPressedEvent != null)
            {
                ButtonRightPressedEvent();
            }
        }
        if (player.GetButtonDown("Up"))
        {
            Debug.Log(player.GetButtonDown("Up"));
            if (ButtonUpPressedEvent != null)
            {
                ButtonUpPressedEvent();
            }
        }
        if (player.GetButtonDown("Left"))
        {
            Debug.Log(player.GetButtonDown("Left"));
            if (ButtonLeftPressedEvent != null)
            {
                ButtonLeftPressedEvent();
            }
        }
        if (player.GetButtonDown("Down"))
        {
            Debug.Log(player.GetButtonDown("Down"));
            if (ButtonDownPressedEvent != null)
            {
                ButtonDownPressedEvent();
            }
        }
        LeftJoystic = new Vector2(player.GetAxis("Left Move Horizontal"), player.GetAxis("Left Move Vertical"));
        if (LeftJoystic != Vector2.zero)
        {
            Debug.Log(player.GetButtonDown("Left Joystic"));
            if (LeftJoystickUsedEvent != null)
            {
                LeftJoystickUsedEvent();
            }
        }
        RightJoystic = new Vector2(player.GetAxis("Right Move Horizontal"), player.GetAxis("Right Move Vertical"));
        if (RightJoystic != Vector2.zero)
        {
            Debug.Log(player.GetButtonDown("Right Joystic"));
            if (RightJoystickUsedEvent != null)
            {
                RightJoystickUsedEvent();
            }
        }

#endif

    }

}
