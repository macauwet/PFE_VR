using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationQuest : MonoBehaviour
{
    private Vector3 offsetPos, offsetOr;
    private GameObject topTable, cameraRig, environment;
    // private int state = 0;

    public GameObject palmCenterR;
    public bool finishCalibrating = false;

    // Start is called before the first frame update
    void Start()
    {
        offsetPos = new Vector3();
        offsetOr = new Vector3();
        palmCenterR = GameObject.Find("Warped_OculusHand_R/b_r_wrist/r_palm_center_marker");
        // forearmR = GameObject.Find("Warped_OculusHand_R/b_r_wrist/b_r_forearm_stub");
        topTable = GameObject.Find("CalibrateMe");
        cameraRig = GameObject.Find("OVRCameraRig");
        environment = GameObject.Find("Environment");

        environment.transform.forward = GameObject.Find("CenterEyeAnchor").transform.forward;
    }

    // Update is called once per frame
    void Update()
    {

    	Debug.DrawRay(topTable.transform.position, palmCenterR.transform.position - topTable.transform.position, Color.blue);

    	if(!finishCalibrating)
    	{
    		if(Input.GetKeyDown(KeyCode.A))
		    {
		        environment.transform.forward = Vector3.ProjectOnPlane( GameObject.Find("CenterEyeAnchor").transform.forward,Vector3.up);

                offsetPos = topTable.transform.position - palmCenterR.transform.position;//GameObject.Find("CenterEyeAnchor").transform.position;
		        cameraRig.transform.position = new Vector3(cameraRig.transform.position.x + offsetPos.x, cameraRig.transform.position.y + offsetPos.y + 0.04f, cameraRig.transform.position.z + offsetPos.z);
		    }

    	}
    	
        if(Input.GetKeyDown(KeyCode.Space))
        {
	        finishCalibrating = !finishCalibrating;

        }
	    

    }
}
