using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetFromController : MonoBehaviour
{
	public GameObject trackedObject;
	public GameObject wristR;
	public GameObject wristV;
	public Vector3 offsetPos;
	public Vector3 offsetRot;


    private int invForward, invUp;
    public CalibOffsetDir calibrateDirections;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        wristV.transform.position = new Vector3(trackedObject.transform.position.x + offsetPos.x, trackedObject.transform.position.y + offsetPos.y, trackedObject.transform.position.z + offsetPos.z);
        // hands.transform.SetParent(this.transform);
        // this.transform.eulerAngles = new Vector3(trackedObject.transform.eulerAngles.x + offsetRot.x, trackedObject.transform.eulerAngles.y + offsetRot.y, trackedObject.transform.eulerAngles.z + offsetRot.z);
        // wristR.transform.SetParent(this.transform);

        // this.transform.rotation = Quaternion.LookRotation(-trackedObject.transform.right , trackedObject.transform.forward );
        // GameObject.Find("Hands").transform.eulerAngles = new Vector3(trackedObject.transform.eulerAngles.x + offsetRot.x, trackedObject.transform.eulerAngles.y + offsetRot.y, trackedObject.transform.eulerAngles.z + offsetRot.z);

		CalibOffsetDir.fwdV forwardChoice = calibrateDirections.myDirectionTracker.forwardVector;
    	CalibOffsetDir.upV upChoice = calibrateDirections.myDirectionTracker.upVector;

    	if(calibrateDirections.myDirectionTracker.inverseFwd)
    	{
    		invForward = -1;
    	}
    	else
    	{
    		invForward = 1;
    	}

    	if(calibrateDirections.myDirectionTracker.inverseUp)
    	{
    		invUp = -1;
    	}
    	else
    	{
    		invUp = 1;
    	}

        if(forwardChoice == CalibOffsetDir.fwdV.trackerForward)
        {
        	if(upChoice == CalibOffsetDir.upV.trackerForward)
        	{
        		Debug.Log("Directions Up and Forward can't be similar!");
        		wristR.transform.rotation = trackedObject.transform.rotation;
        	}
        	if(upChoice == CalibOffsetDir.upV.trackerUp)
        	{
        		wristR.transform.rotation = Quaternion.LookRotation(invForward * this.trackedObject.transform.forward, invUp * this.trackedObject.transform.up);
        	}
        	if(upChoice == CalibOffsetDir.upV.trackerRight)
        	{
        		wristR.transform.rotation = Quaternion.LookRotation(invForward * this.trackedObject.transform.forward, invUp * this.trackedObject.transform.right);
        	}
        }


        if(forwardChoice == CalibOffsetDir.fwdV.trackerUp)
        {
        	if(upChoice == CalibOffsetDir.upV.trackerForward)
        	{
        		 wristR.transform.rotation = Quaternion.LookRotation(invForward * this.trackedObject.transform.up, invUp * this.trackedObject.transform.forward);
        	}
        	if(upChoice == CalibOffsetDir.upV.trackerUp)
        	{
        		Debug.Log("Directions Up and Forward can't be similar!");
        		 wristR.transform.rotation = this.trackedObject.transform.rotation;
        	}
        	if(upChoice == CalibOffsetDir.upV.trackerRight)
        	{
        		 wristR.transform.rotation = Quaternion.LookRotation(invForward * this.trackedObject.transform.up, invUp * this.trackedObject.transform.right);
        	}
        }


        if(forwardChoice == CalibOffsetDir.fwdV.trackerRight)
        {
        	if(upChoice == CalibOffsetDir.upV.trackerForward)
        	{
        		 wristR.transform.rotation = Quaternion.LookRotation(invForward * this.trackedObject.transform.right, invUp * this.trackedObject.transform.forward);
        	}
        	if(upChoice == CalibOffsetDir.upV.trackerUp)
        	{
        		 wristR.transform.rotation = Quaternion.LookRotation(invForward * this.trackedObject.transform.right, invUp * this.trackedObject.transform.up);
        	}
        	if(upChoice == CalibOffsetDir.upV.trackerRight)
        	{
        		Debug.Log("Directions Up and Forward can't be similar!");
        		wristR.transform.rotation = this.trackedObject.transform.rotation;
        	}
        }


         // wristR.transform.rotation = Quaternion.LookRotation(-this.trackedObject.transform.right, this.trackedObject.transform.forward);


         // wristR.transform.position = this.trackedObject.transform.position;
	     // wristR.transform.rotation = this.trackedObject.transform.rotation;


    }
}
