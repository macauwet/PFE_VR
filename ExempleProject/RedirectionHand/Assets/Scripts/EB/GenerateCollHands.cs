using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.IO;
using System.Globalization;


public class GenerateCollHands : MonoBehaviour
{

	[Tooltip("Here slide your tracked hand in.")]	
	public GameObject trackedHand;

	public GameObject[] firstBabies;
	public GameObject[] allBabies;

	[Tooltip("Here associate the AvatarHand based on Oculus Model. This is a parent containing 19 babies.")]	
    public GameObject oculusHandColl;

    public int etat;
    private GameObject[] closests;

    void Start()
    {
    	firstBabies = new GameObject[5];
    	allBabies = new GameObject[24];
    	closests = new GameObject[24];
      
      	etat = 0;

		for(int i = 0; i < 5; i++)
    	{
    		firstBabies[i] = trackedHand.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
    	}

    	for(int i = 0; i < 1; i++)
    	{
			allBabies[4*i] = firstBabies[i];
    		for(int j = 0; j < 3; j++)
    		{
    			allBabies[4*i + j + 1] = allBabies[4*i + j].transform.GetChild(0).gameObject;

    		}
    	}

    	for(int i = 1; i < 2; i++)
    	{
			allBabies[4*i] = firstBabies[i];
    		for(int j = 0; j < 4; j++)
    		{
    			allBabies[4*i + j + 1] = allBabies[4*i + j].transform.GetChild(0).gameObject;
    		}
    	}

    	for(int i = 2; i < 5; i++)
    	{
			allBabies[5*i-1] = firstBabies[i];
    		for(int j = 1; j < 5; j++)
    		{
    			allBabies[5*i-1 + j] = allBabies[5*i-1 + j - 1].transform.GetChild(0).gameObject;

    		}

    	}
    }

    // Update is called once per frame
    void Update()
    {
    	

	// Get back all the babies and attach pos from Livecollision hand
  		for(int i = 0; i < 5; i++)
    	{
    		firstBabies[i] = trackedHand.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
    	}

    	for(int i = 0; i < 1; i++)
    	{
			allBabies[4*i] = firstBabies[i];
    		for(int j = 0; j < 3; j++)
    		{
    			allBabies[4*i + j + 1] = allBabies[4*i + j].transform.GetChild(0).gameObject;

    		}
    	}

    	for(int i = 1; i < 2; i++)
    	{
			allBabies[4*i] = firstBabies[i];
    		for(int j = 0; j < 4; j++)
    		{
    			allBabies[4*i + j + 1] = allBabies[4*i + j].transform.GetChild(0).gameObject;
    		}
    	}

    	for(int i = 2; i < 5; i++)
    	{
			allBabies[5*i-1] = firstBabies[i];
    		for(int j = 1; j < 5; j++)
    		{
    			allBabies[5*i-1 + j] = allBabies[5*i-1 + j - 1].transform.GetChild(0).gameObject;

    		}

    	}


    	switch(etat)
    	{
    		case 0:

    			for(int i = 0; i < 1; i++) // Thumb 
		    	{
		    		allBabies[5*i].transform.gameObject.AddComponent<CapsuleCollider>();
		    		allBabies[5*i].transform.gameObject.AddComponent<Rigidbody>();
		    		allBabies[5*i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    		allBabies[5*i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    		allBabies[5*i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    		allBabies[5*i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;

		    		allBabies[5*i].transform.gameObject.AddComponent<AttachToHand>();

		    		allBabies[5*i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().center;
				    allBabies[5*i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().radius;
				    allBabies[5*i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().height;
				    allBabies[5*i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().direction + 1;

		    	
		    		for(int j = 1; j < 3; j++)  
		    		{
		    			allBabies[5*i + j].transform.gameObject.AddComponent<CapsuleCollider>();
		    			allBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
						
				        allBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().center;
				        allBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().radius;
				        allBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().height;
				        allBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().direction + 1;


				        allBabies[5*i + j].transform.gameObject.AddComponent<AttachToHand>();
		    		}

		    		for(int j = 3; j < 4; j++) // Fingertips
		    		{
		    			allBabies[5*i + j].transform.gameObject.AddComponent<SphereCollider>();
		    			allBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;

		    			allBabies[5*i + j].transform.gameObject.AddComponent<AttachToHand>();
		    		}
		    	}

	    		for(int i = 1; i < 5; i++) // Fingers 
		    	{
		    		allBabies[5*i-1].transform.gameObject.AddComponent<CapsuleCollider>();
		    		allBabies[5*i-1].transform.gameObject.AddComponent<Rigidbody>();
		    		allBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    		allBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    		allBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    		allBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;

		    		allBabies[5*i-1].transform.gameObject.AddComponent<AttachToHand>();

		    		allBabies[5*i-1].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().center;
				    allBabies[5*i-1].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().radius;
				    allBabies[5*i-1].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().height;
				    allBabies[5*i-1].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().direction;

		    	
		    		for(int j = 0; j < 3; j++)  
		    		{
		    			allBabies[5*i + j].transform.gameObject.AddComponent<CapsuleCollider>();
		    			allBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
						
				        allBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().center;
				        allBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().radius;
				        allBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().height;
				        allBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().direction;


				        allBabies[5*i + j].transform.gameObject.AddComponent<AttachToHand>();
		    		}
		    		for(int j = 3; j < 4; j++) // Fingertips
		    		{
		    			allBabies[5*i + j].transform.gameObject.AddComponent<SphereCollider>();
		    			allBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;

		    			allBabies[5*i + j].transform.gameObject.AddComponent<AttachToHand>();
		    		}
		    	}

		    	etat = 1;
    		break;


    		case 1:
    		// Target Positions
    			for(int i = 0; i < 23; i++)
    			{
    				closests[i] = GameObject.Find("Targets").transform.GetChild(i).gameObject;
    				closests[i].transform.position = allBabies[i].gameObject.GetComponent<AttachToHand>().closestDist;
    			}

    	
    		break;

    		case 2:


    		break;


    	}
    }
}
// }
