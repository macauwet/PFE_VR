using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Calibration_EB : MonoBehaviour
{
    public GameObject controllerRight, topTable, cameraRig, table;
    private Vector3 offsetPos, offsetOr;
    private GameObject modelController;
    public int state = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        if(controllerRight == null)
        {
            controllerRight = GameObject.Find("Controller (right)");
            cameraRig = GameObject.Find("[CameraRig]");
            topTable = GameObject.Find("CalibrateMe");
            table = GameObject.Find("cover");
        }
        offsetPos = new Vector3();
        offsetOr = new Vector3();

    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case 0:
            if(controllerRight != null)
            {
                modelController = (GameObject)Instantiate((GameObject.Find("body")), topTable.transform);
                modelController.AddComponent<MeshCollider>();
                modelController.GetComponent<MeshCollider>().convex = true;
                modelController.AddComponent<Rigidbody>();
                state = 1;
            }
            break;

            case 1:

                Destroy(modelController.GetComponent<Rigidbody>());
                modelController.GetComponent<MeshCollider>().enabled = false;
                modelController.GetComponent<MeshRenderer>().enabled = false;
                Destroy(table.GetComponent<Rigidbody>());
                
                state = 2;
            break;

            case 2:

                if(Input.GetKeyDown(KeyCode.Keypad0))
                {
                    offsetPos = modelController.transform.position - controllerRight.transform.position;
                    offsetOr = modelController.transform.eulerAngles - controllerRight.transform.eulerAngles;
                    cameraRig.transform.position = new Vector3(cameraRig.transform.position.x + offsetPos.x, cameraRig.transform.position.y + offsetPos.y, cameraRig.transform.position.z + offsetPos.z);
                    cameraRig.transform.eulerAngles = new Vector3(cameraRig.transform.eulerAngles.x + offsetOr.x, cameraRig.transform.eulerAngles.y + offsetOr.y, cameraRig.transform.eulerAngles.z + offsetOr.z);
                }
            break;
        }
        
        
    }
}
