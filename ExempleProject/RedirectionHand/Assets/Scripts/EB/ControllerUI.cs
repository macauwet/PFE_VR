using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class ControllerUI : MonoBehaviour
{
    public bool increase, decrease;
    public bool next, reset;
    public InstantiateConditions gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<InstantiateConditions>();
        if(!gameManager.quest)
        {
            SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);
            SteamVR_Actions.default_SnapTurnRight.AddOnStateDownListener(TurnRight, SteamVR_Input_Sources.Any);
            SteamVR_Actions.default_SnapTurnLeft.AddOnStateDownListener(TurnLeft, SteamVR_Input_Sources.Any);
            SteamVR_Actions.default_GrabGrip.AddOnStateDownListener(GrabGrip, SteamVR_Input_Sources.Any);
        }
    	
    }


    // Update is called once per frame
    void LateUpdate()
    {
    	increase = false;
    	decrease = false;
    	reset = false;
    	next = false;

        if(!gameManager.quest)
        {
            SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);
            SteamVR_Actions.default_SnapTurnRight.AddOnStateDownListener(TurnRight, SteamVR_Input_Sources.Any);
            SteamVR_Actions.default_SnapTurnLeft.AddOnStateDownListener(TurnLeft, SteamVR_Input_Sources.Any);
            SteamVR_Actions.default_GrabGrip.AddOnStateDownListener(GrabGrip, SteamVR_Input_Sources.Any);
        }

        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            reset = true;
            next = false;
            increase = false;
            decrease = false;
        }

        if(Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            decrease = true;
            increase = false;
            reset = false;
            next = false;
        }

        if(Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            increase = true;
            decrease = false;
            reset = false;
            next = false;
        }

        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            next = true;
            reset = false;
            increase = false;
            decrease = false;
        }


    }


    private void GrabGrip(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
    	reset = true;
    	next = false;
    	increase = false;
    	decrease = false;
    }

    private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

    	next = true;
    	reset = false;
	    increase = false;
	    decrease = false;

    }
    

    private void TurnRight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
		increase = true;
    	decrease = false;
    	reset = false;
    	next = false;

    }
    private void TurnLeft(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
		decrease = true;
		increase = false;
    	reset = false;
    	next = false;

    }

}
