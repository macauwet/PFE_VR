using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{

	private int j;
   	public int slidingWindow = 30;
   	public bool recordNow;
   	private Vector3 oldPosR, oldPosL, graspDirectionR, graspDirectionL;
    public GameObject handR, handL;
    public bool goingToInteractR, goingToInteractL;
    public GameObject indexR, indexL, thumbR, thumbL;
    public GameObject attachMeR, attachMeL;
    public bool[] fingerPhalanx;
    public bool graspR, graspL;
    public int state;
    // public GameObject[] checkMeUp;
    // public FollowLeapHands getMyBabies;

	// private GameObject palmR, palmL;

    // Start is called before the first frame update
    void Start()
    {
    	// getMyBabies = GameObject.FindObjectOfType<FollowLeapHands>();
        // // palmL = GameObject.Find("AvatarHands/Generic Hand_Left/baseMeshHand_Left_GRP/L_Wrist/L_Palm");
        // indexL = GameObject.Find("AvatarHands/Generic Hand_Left/baseMeshHand_Left_GRP/L_Wrist/L_Palm/L_index_meta/L_index_a/L_index_b/L_index_c/L_index_end");
        // thumbL = GameObject.Find("AvatarHands/Generic Hand_Left/baseMeshHand_Left_GRP/L_Wrist/L_Palm/L_thumb_meta/L_thumb_a/L_thumb_b/L_thumb_end");

        // // palmR = GameObject.Find("AvatarHands/Generic Hand_Right/baseMeshHand_Left_GRP/L_Wrist/L_Palm");
        // indexR = GameObject.Find("AvatarHands/Generic Hand_Right/baseMeshHand_Left_GRP/L_Wrist/L_Palm/L_index_meta/L_index_a/L_index_b/L_index_c/L_index_end");
        // thumbR = GameObject.Find("AvatarHands/Generic Hand_Right/baseMeshHand_Left_GRP/L_Wrist/L_Palm/L_thumb_meta/L_thumb_a/L_thumb_b/L_thumb_end");
    	attachMeL = GameObject.Find("VF-L");
    	attachMeR = GameObject.Find("VF-R");

    }

    // Update is called once per frame
    void Update()
    {
        recordNow = false;
    	StartCoroutine(RecordPos());
    	// handR =  GameObject.Find("---------HANDS-------/AvatarHands/GenericHand_Right/baseMeshHand_Left_GRP/L_Wrist/");


    	fingerPhalanx = new bool[52];
    	// checkMeUp = new GameObject[52];
        
        graspDirectionR = handR.transform.position - oldPosR;
        graspDirectionL = handL.transform.position - oldPosL;
        graspR = false;
        graspL = false;


        if(Vector3.Dot(graspDirectionR, handR.transform.up) > 0)
        {
            goingToInteractR = true;
        }
        else
        {
            goingToInteractR = false;
        }

		if(Vector3.Dot(graspDirectionL, handL.transform.up) > 0)
        {
            goingToInteractL = true;
        }
        else
        {
            goingToInteractL = false;
        }

        attachMeL.transform.position = (thumbL.transform.position + (indexL.transform.position - thumbL.transform.position)/2);
        attachMeR.transform.position = (thumbR.transform.position + (indexR.transform.position - thumbR.transform.position)/2);

 		for(int i = 0; i < 26; i++)
        {
        	fingerPhalanx[i] = GameObject.FindObjectsOfType<ClosestFromAvatar>()[i].phalanxContact;
        	fingerPhalanx[i+26] = GameObject.FindObjectsOfType<ClosestFromAvatar>()[i].phalanxContact;
        }
        for(int i = 0; i < 26; i++)
        {
        	if(fingerPhalanx[2*i + 1])
        	{
        		graspR = true;
        	}
        	if(fingerPhalanx[2*i])
        	{
        		graspL = true;
        	}
        }

        switch(state)
        {
        	case 0:
        		// GameObject.FindGameObjectWithTag("Target").GetComponent<Rigidbody>().isKinematic = false;

        		if((goingToInteractR && graspR) || (goingToInteractL && graspL))
		        {
		        	state = 1;
		        }
        	break;

        	case 1:
	        	if(graspR)
	        	{
	        		// GameObject.FindGameObjectWithTag("Target").GetComponent<Rigidbody>().isKinematic = true;
	        		GameObject.FindGameObjectWithTag("Target").transform.SetParent(attachMeR.transform);
	        	}
	        	else if(graspL)
	        	{
	        		// GameObject.FindGameObjectWithTag("Target").GetComponent<Rigidbody>().isKinematic = true;
	        		GameObject.FindGameObjectWithTag("Target").transform.SetParent(attachMeL.transform);
	        	}
	        	else
	        	{
	        		// GameObject.FindGameObjectWithTag("Target").GetComponent<Rigidbody>().isKinematic = false;
	        		state = 0;
	        	}

        	break;

        }
        
        // IF GOING TO INTERACT + CONTACT (graspContact/PhalanxContact) -> Set Parent - VF Pos
        // ELSE Remove Parent
        // Get closest from avatar - grasp/contact to define if contact



        j = j+1;
    	if(j%slidingWindow == 0)
    	{
    		recordNow = true;
    	}

    }

    IEnumerator RecordPos()
    {
    	yield return new WaitUntil (() => recordNow == true);
        oldPosR = handR.transform.position;
        oldPosL = handL.transform.position;

    }


}
