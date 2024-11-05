using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deform;


public class CompareObjects : MonoBehaviour
{

	public GameObject leftObject, rightObject, middleObject;
	public int[] stiffness;
	private Vector3 posOriginL, posOriginR, rotOrigin;
	public Vector3 offsetPosL, offsetPosR;
	public Vector3 initLeftPos, initRightPos, initLeftOr, initRightOr;
	public FollowOculusHands handManager;
	public Vector3 scaleObj;
	public FSRComparison fsrConvert;
	public HandRedirectionManager redirectManager;
	public bool firstTime;

    // Start is called before the first frame update
    void Start()
    {
        leftObject.tag = "Target";
		leftObject.AddComponent<Deformable>();
		leftObject.AddComponent<SquashAndStretchDeformer>();
		leftObject.GetComponent<Deformable>().AddDeformer(leftObject.GetComponent<SquashAndStretchDeformer>());// = GameObject.FindObjectOfType<Deformer>();
		
		rightObject.tag = "Target";
		rightObject.AddComponent<Deformable>();
		rightObject.AddComponent<SquashAndStretchDeformer>();
		rightObject.GetComponent<Deformable>().AddDeformer(rightObject.GetComponent<SquashAndStretchDeformer>());// = GameObject.FindObjectOfType<Deformer>();
		
		GameObject.FindObjectOfType<FSRComparison>().force = 0;

		leftObject.AddComponent<Rigidbody>();
		leftObject.GetComponent<Rigidbody>().mass = 100000f;
		leftObject.GetComponent<Rigidbody>().drag = 100000f;
		leftObject.GetComponent<Rigidbody>().angularDrag = 100000f;
		leftObject.gameObject.isStatic = true;

		rightObject.AddComponent<Rigidbody>();
		rightObject.GetComponent<Rigidbody>().mass = 100000f;
		rightObject.GetComponent<Rigidbody>().drag = 100000f;
		rightObject.GetComponent<Rigidbody>().angularDrag = 100000f;
		rightObject.gameObject.isStatic = true;
		// leftObject.SetActive(true);

		leftObject.transform.localScale = scaleObj;
    	rightObject.transform.localScale = scaleObj;
    	middleObject.transform.localScale = scaleObj;

    	handManager = this.GetComponent<FollowOculusHands>();
    	redirectManager = this.GetComponent<HandRedirectionManager>();

		// initLeftPos = leftObject.transform.position;
  //   	initRightPos = rightObject.transform.position;

  //   	initLeftOr = leftObject.transform.eulerAngles;
  //   	initRightOr = rightObject.transform.eulerAngles;
    	
    }

    // Update is called once per frame
    void Update()
    {

    	// leftObject.transform.position = initLeftPos;
    	// rightObject.transform.position = initRightPos;

    	// leftObject.transform.eulerAngles = initLeftOr;
    	// rightObject.transform.eulerAngles = initRightOr;
		// posOriginR = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPosR.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPosR.x, GameObject.Find("CalibrateMe").transform.position.z + offsetPosR.z);
		// posOriginL = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPosL.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPosL.x, GameObject.Find("CalibrateMe").transform.position.z + offsetPosL.z);
		// leftObject.transform.position = posOriginL;
  //   	rightObject.transform.position = posOriginR;
    	if(GameObject.FindObjectOfType<CalibrationQuest>().finishCalibrating)
    	{
			if(scaleObj.z == 0.055f)
			{
				if(firstTime)
				{
					firstTime = false;
                }
				else
				{
                    leftObject.transform.position = new Vector3(initLeftPos.x, initLeftPos.y, initLeftPos.z);// - 0.025f);
                    rightObject.transform.position = new Vector3(initRightPos.x, initRightPos.y, initRightPos.z);// - 0.025f);
                }
				
            }
			else
			{
                leftObject.transform.position = initLeftPos;
                rightObject.transform.position = initRightPos;
            }
            
    		leftObject.transform.eulerAngles = initLeftOr;
    		rightObject.transform.eulerAngles = initRightOr;
    	}
    	else
    	{
			if(firstTime)
			{
                initLeftPos = new Vector3(leftObject.transform.position.x, leftObject.transform.position.y, leftObject.transform.position.z - 0.025f);
                initRightPos = new Vector3(rightObject.transform.position.x, rightObject.transform.position.y, rightObject.transform.position.z - 0.025f);

            }
			else
			{
				initLeftPos = leftObject.transform.position;
				initRightPos = rightObject.transform.position;
			}
	    	initLeftOr = leftObject.transform.eulerAngles;
	    	initRightOr = rightObject.transform.eulerAngles;
    	}

		// rotOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.eulerAngles.x, GameObject.Find("CalibrateMe").transform.eulerAngles.y, GameObject.Find("CalibrateMe").transform.eulerAngles.z);
		// leftObject.transform.eulerAngles = rotOrigin;
		// rightObject.transform.eulerAngles = rotOrigin;

        if(fsrConvert.deltaDist >= (scaleObj.z - 0.005f))
		{
			fsrConvert.deltaDist = scaleObj.z - 0.005f;
		}

		if((redirectManager.numObject % 2) == 0)
		{
			leftObject.GetComponent<SquashAndStretchDeformer>().Factor = -Mathf.Lerp(0, 1, fsrConvert.deltaDist);
			leftObject.gameObject.transform.localScale = new Vector3(scaleObj.x, scaleObj.y, scaleObj.z - fsrConvert.deltaDist);
		}
		
		if((redirectManager.numObject % 2) == 1)
		{
			rightObject.GetComponent<SquashAndStretchDeformer>().Factor = -Mathf.Lerp(0, 1, fsrConvert.deltaDist);
			rightObject.gameObject.transform.localScale = new Vector3(scaleObj.x, scaleObj.y, scaleObj.z - fsrConvert.deltaDist);

		}
		

		int forceMax = 13;
		if(fsrConvert.force >= forceMax)
		{
			//(handManager.warpedL[1] || handManager.warpedR[1]) &&
			if((redirectManager.numObject % 2) == 1)
			{
				rightObject.GetComponent<MeshRenderer>().material.color = Color.green;
			}
			// else
			// {
			// 	rightObject.GetComponent<MeshRenderer>().material.color = Color.blue;
			// }
			// (handManager.warpedL[0] || handManager.warpedR[0]) &&
			if((redirectManager.numObject % 2) == 0)
			{
				leftObject.GetComponent<MeshRenderer>().material.color = Color.green;
			}
			// else
			// {
			// 	leftObject.GetComponent<MeshRenderer>().material.color = Color.blue;
			// }
		}
		else
		{
			rightObject.GetComponent<MeshRenderer>().material.color = Color.blue;
			leftObject.GetComponent<MeshRenderer>().material.color = Color.blue;
		}

		if(Input.GetKeyDown(KeyCode.Q))
		{
			if(scaleObj.z == 0.08f)
			{
				scaleObj.z = 0.055f;
				firstTime = true;
			}
			else
			{
				scaleObj.z = 0.08f;
			}
		}


				
    }
}
