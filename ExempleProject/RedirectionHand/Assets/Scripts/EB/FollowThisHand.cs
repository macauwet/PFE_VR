using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.IO;
using System.Globalization;
using UnityEngine.Animations.Rigging;


public class FollowThisHand : MonoBehaviour
{
    [Tooltip("Here slide your real tracked hand in.")]	
	public GameObject realHand;

    [Tooltip("Here slide your Oculus avatar \"pseudo hand\" in.")]	
    public GameObject avatarHand;

    public GameObject[] realParents, avatarParents;
    public GameObject[] realBabies, avatarBabies;

    public bool fsrTouching;
    private string suffixFingerpads, prefix, suffixKnucle;

	private string[] fingerNames;
	private int etat = 0;

	[Tooltip("Here associate the AvatarHand based on Oculus Model. This is a parent containing 19 babies.")]	
    public GameObject oculusHandColl;

	// These become the targets for IK
	private GameObject[] closests;

    // Start is called before the first frame update
    void Start()
    {
    	fsrTouching = false;

        realParents = new GameObject[5];
        avatarParents = new GameObject[5];

        realBabies = new GameObject[25];
        avatarBabies = new GameObject[25];

    	closests = new GameObject[5];


    	suffixFingerpads = "_finger_pad_marker";
        suffixKnucle = "_knuckle_marker";
        prefix = "r_";
        fingerNames = new string[5]{"thumb", "index", "middle", "ring", "pinky"};

		// LEAP MOTION COMPONENTS
        realBabies[realBabies.Length-1] = GameObject.Find("L_Wrist");

        for(int i = 0; i < 5; i++)
    	{
    		realParents[i] = realHand.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
    		avatarParents[i] = avatarHand.transform.GetChild(0).gameObject.transform.GetChild(i+1).gameObject;
    	}

    	for(int i = 0; i < 1; i++) // Real Thumb (from Leap) - 0-3 Thumb
    	{
			realBabies[i] = realParents[i];
    		for(int j = 0; j < 3; j++)
    		{
    			realBabies[j + 1] = realBabies[j].transform.GetChild(0).gameObject;

    		}
    	}

    	for(int i = 1; i < 2; i++) // Real Index (from Leap) 4-8 Index
    	{
			realBabies[4*i] = realParents[i];
    		for(int j = 0; j < 4; j++)
    		{
    			realBabies[4*i + j + 1] = realBabies[4*i + j].transform.GetChild(0).gameObject;
    		}
    	}

    	for(int i = 2; i < 5; i++) // Real Other Fingers (from Leap) 9-13 Middle, 14-18 Ring, 19-23 Pinky
    	{
			realBabies[5*i-1] = realParents[i];
    		for(int j = 1; j < 5; j++)
    		{
    			realBabies[5*i-1 + j] = realBabies[5*i-1 + j - 1].transform.GetChild(0).gameObject;

    		}

    	}


    	// OCULUS HAND COMPONENTS

    	avatarBabies[avatarBabies.Length-1] = GameObject.Find("b_" + prefix + "wrist");
        for(int i = 0; i < 1; i++) // THUMB OCULUS
    	{
    		for(int j = 1; j < 4; j++)
    		{
    			avatarBabies[5*i + j-1] = GameObject.Find("b_" + prefix + fingerNames[i] + j.ToString());
    		}
    		avatarBabies[5*i+3] = GameObject.Find(prefix + fingerNames[i] + suffixFingerpads);
    	}

    	for(int i = 1; i < 4; i++) // FINGERS OCULUS
    	{
    		for(int j = 1; j < 4; j++)
    		{
    			avatarBabies[5*i + j-1] = GameObject.Find("b_" + prefix + fingerNames[i] + j.ToString());
    		}
    		avatarBabies[5*i+3] = GameObject.Find(prefix + fingerNames[i] + suffixFingerpads);
	    	avatarBabies[5*i - 1] = GameObject.Find(prefix + fingerNames[i] + suffixKnucle);

    	}

    	for(int i = 4; i < 5; i++) // PINKY OCULUS
    	{
    		for(int j = 1; j < 4; j++)
    		{
    			avatarBabies[5*i + j-1] = GameObject.Find("b_" + prefix + fingerNames[i] + j.ToString());
    		}
    		avatarBabies[5*i+3] = GameObject.Find(prefix + fingerNames[i] + suffixFingerpads);
    		avatarBabies[5*i - 1] = GameObject.Find(prefix + fingerNames[i] + suffixKnucle);

    	}

    	// DEFINE GAMEOBJECTS WHICH POSITION ARE THE TARGETS FOR IK
    	for(int i = 0; i < 5; i++)
		{
			GameObject.Find("Closests").transform.GetChild(i).gameObject.name = fingerNames[i];
			closests[i] = GameObject.Find("Closests").transform.GetChild(i).gameObject;
		}


    }

    // Update is called once per frame
    void Update()
    {
        
    	switch(etat)
    	{
    		case 0:

    			// ADD COLLIDERS TO THE AVATAR HAND
    			for(int i = 0; i < 1; i++) // Thumb 
		    	{
		    		avatarBabies[5*i].transform.gameObject.AddComponent<CapsuleCollider>();
		    		avatarBabies[5*i].transform.gameObject.AddComponent<Rigidbody>();
		    		avatarBabies[5*i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    		avatarBabies[5*i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    		avatarBabies[5*i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    		avatarBabies[5*i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;

		    		// avatarBabies[5*i].transform.gameObject.AddComponent<AttachToHand>();

		    		avatarBabies[5*i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().center;
				    avatarBabies[5*i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().radius;
				    avatarBabies[5*i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().height;
				    avatarBabies[5*i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().direction;

		    	
		    		for(int j = 1; j < 3; j++)  
		    		{
		    			avatarBabies[5*i + j].transform.gameObject.AddComponent<CapsuleCollider>();
		    			avatarBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
						
				        avatarBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().center;
				        avatarBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().radius;
				        avatarBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().height;
				        avatarBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().direction;
		    		}

		    		for(int j = 3; j < 4; j++) // Fingertips
		    		{
		    			avatarBabies[5*i + j].transform.gameObject.AddComponent<SphereCollider>();
		    			avatarBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;
		    		}
		    	}

	    		for(int i = 1; i < 5; i++) // Fingers 
		    	{
		    		avatarBabies[5*i-1].transform.gameObject.AddComponent<CapsuleCollider>();
		    		avatarBabies[5*i-1].transform.gameObject.AddComponent<Rigidbody>();
		    		avatarBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    		avatarBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    		avatarBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    		avatarBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;

		    		// avatarBabies[5*i-1].transform.gameObject.AddComponent<AttachToHand>();

		    		avatarBabies[5*i-1].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().center;
				    avatarBabies[5*i-1].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().radius;
				    avatarBabies[5*i-1].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().height;
				    avatarBabies[5*i-1].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild((3*i) + 4).GetComponent<CapsuleCollider>().direction;

		    	
		    		for(int j = 0; j < 3; j++)  
		    		{
		    			avatarBabies[5*i + j].transform.gameObject.AddComponent<CapsuleCollider>();
		    			avatarBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
						
				        avatarBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().center;
				        avatarBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().radius;
				        avatarBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().height;
				        avatarBabies[5*i + j].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild((3*i) + j + 4).GetComponent<CapsuleCollider>().direction;

		    		}
		    		for(int j = 3; j < 4; j++) // Fingertips
		    		{
		    			avatarBabies[5*i + j].transform.gameObject.AddComponent<SphereCollider>();
		    			avatarBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
		    			avatarBabies[5*i + j].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;
		    		}
		    	}

		    	// ATTACH SPHERE COLLIDERS TO REAL HAND FINGERTIPS AND GET THE CLOSEST POINTS

		    	for(int i = 0; i < 5; i++) // Fingers 
		    	{
		    		for(int j = 3; j < 4; j++) // Fingertips
		    		{
		    			realBabies[5*i + j].transform.gameObject.AddComponent<SphereCollider>();
		    			realBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			realBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			realBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			realBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			realBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
		    			realBabies[5*i + j].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;

		    			realBabies[5*i + j].transform.gameObject.AddComponent<AttachToFingertips>();
		    		}
		    	}

		    	etat = 1;
    		break;


    		case 1:

    			for(int i = 0; i < 5; i++) // GET CLOSEST POINTS - THESE ARE THE TARGETS FOR IK
    			{
    				closests[i].transform.position = realBabies[3 + 5*i].gameObject.GetComponent<AttachToFingertips>().closestDist;
    			}

		    	if(!fsrTouching)
		    	{
		    		for(int i = 0; i < avatarParents.Length; i++)
		    		{
			    		avatarHand.GetComponent<DefineClosestToFingertips>().fingerRIKinematics[i].enabled = false;
		    		}
		    		
		    		for(int i = 0; i < 1; i++)
		    		{
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[i].position;
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].gameObject.transform.eulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[i].eulerAngles;

		    		}
		    		avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[2].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[1].position;
	    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[2].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[1].localEulerAngles;

		    		for(int i = 0; i < 3; i++) // THUMB
		    		{
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+3].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[23+i].position;
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+3].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[23+i].localEulerAngles;

		    		}
		    		for(int i = 0; i < 3; i++) // INDEX
		    		{
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+6].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[3+i].position;
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+6].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[3+i].localEulerAngles;

		    		}
		    		for(int i = 0; i < 3; i++) // MIDDLE
		    		{
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+9].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[8+i].position;
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+9].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[8+i].localEulerAngles;
		    		}
		    		for(int i = 0; i < 3; i++) // RING
		    		{
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+12].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[18+i].position;
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+12].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[18+i].localEulerAngles;
		    		}
		    		for(int i = 1; i < 4; i++) // PINKY
		    		{
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+15].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[12+i].position;
		    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[i+15].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[12+i].localEulerAngles;
		    		}
		    		// FINGERTIPS
		    		avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[19].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[25].position;
	    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[19].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[25].localEulerAngles;
		    		// FINGERTIPS
		    		avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[20].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[6].position;
	    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[20].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[6].localEulerAngles;
		    		// FINGERTIPS
		    		avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[21].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[11].position;
	    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[21].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[11].localEulerAngles;
		    		// FINGERTIPS
		    		avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[22].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[21].position;
	    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[22].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[21].localEulerAngles;
		    		// FINGERTIPS
		    		avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[23].gameObject.transform.position = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[16].position;
	    			avatarHand.GetComponent<OVRCustomSkeleton>().CustomBones[23].gameObject.transform.localEulerAngles = GameObject.Find("L_Wrist").GetComponent<BoneRenderer>().transforms[16].localEulerAngles;
		    	}
		    	else
		    	{
		    		for(int i = 0; i < avatarParents.Length; i++)
		    		{
			    		avatarHand.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i].enabled = true;	
		    		}
		    	}
    		break;

    	}
    }
}
