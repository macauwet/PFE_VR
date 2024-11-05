using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.IO;
using System.Globalization;

public class CreateLiveCollisionHand : MonoBehaviour
{
	private Vector3[] rotApproxSkel, posApproxSkel;
    private Vector3[] rotApproxSkelL, posApproxSkelL;

    [Tooltip("Associate the OVR Custom Skeleton script to your tracked hand.")]   
	public OVRCustomSkeleton handSkelRight;

	private string pathCollProperty, pathCollNames;

    private string[] dataCollProp, dataNames;
    private string[] collProperties;
    private StreamReader srCollProp, srNames;
    
    void Start()
    {
        posApproxSkel = new Vector3[19];
        rotApproxSkel = new Vector3[19];

        for(int i = 0; i < 19; i++)
        {

            this.transform.GetChild(i).transform.gameObject.AddComponent<CapsuleCollider>();
            this.transform.GetChild(i).transform.gameObject.AddComponent<Rigidbody>();
    		this.transform.GetChild(i).transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            this.transform.GetChild(i).transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            this.transform.GetChild(i).transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            this.transform.GetChild(i).transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;

            pathCollNames = "Assets/Resources/DataCollection/namesColliders.csv";
            pathCollProperty = "Assets/Resources/DataCollection/propsColliders.csv";
            

			StreamReader srNames = new StreamReader(pathCollNames, true);
	        if (srNames.Peek() > -1) 
	        {
	            string line = srNames.ReadToEnd();     
	            dataNames = line.Split('\n');
	        }

        	this.transform.GetChild(i).name = dataNames[i];

		    StreamReader srCollProp = new StreamReader(pathCollProperty, true);
		    
		    if (srCollProp.Peek() > -1) 
		    {
		        string line = srCollProp.ReadToEnd();     
		        dataCollProp = line.Split('\n');
		    }
	    	collProperties = dataCollProp[1].Split(';');

	    	
    		int idProp = int.Parse(collProperties[(i*13)]);
            this.transform.GetChild(idProp).transform.gameObject.GetComponent<CapsuleCollider>().center = new Vector3(float.Parse((collProperties[(i*13)+7]), CultureInfo.InvariantCulture), float.Parse((collProperties[(i*13)+8]), CultureInfo.InvariantCulture), float.Parse((collProperties[(i*13)+9]), CultureInfo.InvariantCulture));
            this.transform.GetChild(idProp).transform.gameObject.GetComponent<CapsuleCollider>().radius = float.Parse((collProperties[(i*13)+10]), CultureInfo.InvariantCulture);
            this.transform.GetChild(idProp).transform.gameObject.GetComponent<CapsuleCollider>().height = float.Parse((collProperties[(i*13)+11]), CultureInfo.InvariantCulture);
            this.transform.GetChild(idProp).transform.gameObject.GetComponent<CapsuleCollider>().direction = int.Parse(collProperties[(i*13)+12]);
            

    	}

    }

    // Update is called once per frame
    void Update()
    {
        
        posApproxSkel[0] = handSkelRight.CustomBones[2].transform.position;
        posApproxSkel[1] = handSkelRight.CustomBones[2].transform.position;
        posApproxSkel[2] = handSkelRight.CustomBones[0].transform.position;
        posApproxSkel[3] = handSkelRight.CustomBones[15].transform.position;
        posApproxSkel[4] = handSkelRight.CustomBones[3].transform.position;
        posApproxSkel[5] = handSkelRight.CustomBones[4].transform.position;
        posApproxSkel[6] = handSkelRight.CustomBones[5].transform.position;
        posApproxSkel[7] = handSkelRight.CustomBones[6].transform.position;
        posApproxSkel[8] = handSkelRight.CustomBones[7].transform.position;
        posApproxSkel[9] = handSkelRight.CustomBones[8].transform.position;
        posApproxSkel[10] = handSkelRight.CustomBones[9].transform.position;
        posApproxSkel[11] = handSkelRight.CustomBones[10].transform.position;
        posApproxSkel[12] = handSkelRight.CustomBones[11].transform.position;
        posApproxSkel[13] = handSkelRight.CustomBones[12].transform.position;
        posApproxSkel[14] = handSkelRight.CustomBones[13].transform.position;
        posApproxSkel[15] = handSkelRight.CustomBones[14].transform.position;
        posApproxSkel[16] = handSkelRight.CustomBones[16].transform.position;
        posApproxSkel[17] = handSkelRight.CustomBones[17].transform.position;
        posApproxSkel[18] = handSkelRight.CustomBones[18].transform.position;           	

        rotApproxSkel[0] = handSkelRight.CustomBones[2].transform.eulerAngles;
		rotApproxSkel[1] = handSkelRight.CustomBones[2].transform.eulerAngles;
        rotApproxSkel[2] = handSkelRight.CustomBones[0].transform.eulerAngles;
        rotApproxSkel[3] = handSkelRight.CustomBones[15].transform.eulerAngles;
        rotApproxSkel[4] = handSkelRight.CustomBones[3].transform.eulerAngles;
        rotApproxSkel[5] = handSkelRight.CustomBones[4].transform.eulerAngles;
        rotApproxSkel[6] = handSkelRight.CustomBones[5].transform.eulerAngles;
        rotApproxSkel[7] = handSkelRight.CustomBones[6].transform.eulerAngles;
        rotApproxSkel[8] = handSkelRight.CustomBones[7].transform.eulerAngles;
        rotApproxSkel[9] = handSkelRight.CustomBones[8].transform.eulerAngles;
        rotApproxSkel[10] = handSkelRight.CustomBones[9].transform.eulerAngles;
        rotApproxSkel[11] = handSkelRight.CustomBones[10].transform.eulerAngles;
        rotApproxSkel[12] = handSkelRight.CustomBones[11].transform.eulerAngles;
        rotApproxSkel[13] = handSkelRight.CustomBones[12].transform.eulerAngles;
        rotApproxSkel[14] = handSkelRight.CustomBones[13].transform.eulerAngles;
        rotApproxSkel[15] = handSkelRight.CustomBones[14].transform.eulerAngles;
        rotApproxSkel[16] = handSkelRight.CustomBones[16].transform.eulerAngles;
        rotApproxSkel[17] = handSkelRight.CustomBones[17].transform.eulerAngles;
        rotApproxSkel[18] = handSkelRight.CustomBones[18].transform.eulerAngles;

        
    	for(int k = 0; k < 19; k++)
    	{
    		this.transform.GetChild(k).gameObject.transform.position = posApproxSkel[k];
    		this.transform.GetChild(k).gameObject.transform.eulerAngles = rotApproxSkel[k];

    	}
		
    }
}