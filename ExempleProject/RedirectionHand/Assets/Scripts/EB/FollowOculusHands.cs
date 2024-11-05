using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOculusHands : MonoBehaviour
{
	[HideInInspector]
	public GameObject rightHand, leftHand, warpedRightHand, warpedLeftHand;//, warpedSecondRightHand;
    private string prefixR, prefixL, prefixGlobalL, prefixGlobalR;
    private string[] fingerNames;

    private GameObject[] fingertipsR, fingertipsL;

    private GameObject closests;
  
    private GameObject avatarHandParent, anchorL, anchorR;//, anchorLBase, anchorRBase;
    public bool inDaZoneR, inDaZoneL;
    private bool[] zoneMe;
    private int numberColliders;

    private int state = 0;
    private FSRComparison fsrConversion;


    public Transform currentTarget;
    public Transform resetTarget;
    public Transform physicalTarget;

    public bool reverse;
    private Vector3 warpVectorL, warpVectorR, projectVector;
    private float ds, dp;

    public bool startWarpL, startWarpR;
    public List<bool> warpedL, warpedR;
    public Vector3 vectorTest;

    public GameObject camera;



    // Start is called before the first frame update
    void Start()
    {
        prefixR = "r_";
        prefixL = "l_";
        fingerNames = new string[5]{"index", "middle", "pinky", "ring", "thumb"};
        prefixGlobalL = "Hands/AvatarHands/TrackingSpace/LeftHandAnchor/WarpedLeftHand/";
        prefixGlobalR = "Hands/AvatarHands/TrackingSpace/RightHandAnchor/WarpedRightHand/";

        anchorL = GameObject.Find("Hands/AvatarHands/TrackingSpace/LeftHandAnchor/");
        anchorR = GameObject.Find("Hands/AvatarHands/TrackingSpace/RightHandAnchor/");

        leftHand = GameObject.Find("OVRCustomHandPrefab_L"); // THIS IS HP = Hand Physical
    	rightHand = GameObject.Find("OVRCustomHandPrefab_R");
    	
    	warpedRightHand = GameObject.Find("WarpedRightHand"); // THIS IS HV = Hand Virtual
    	warpedLeftHand = GameObject.Find("WarpedLeftHand");
    	
        fingertipsR = new GameObject[5];
        fingertipsL = new GameObject[5];
        closests = GameObject.Find("Closests");
        avatarHandParent = GameObject.Find("AvatarHands");

        // DEFINE GAMEOBJECTS WHICH POSITION ARE THE TARGETS FOR IK
        for(int i = 0; i < 5; i++)
        {
            closests.transform.GetChild(i+5).gameObject.name = prefixR + fingerNames[i];
            closests.transform.GetChild(i).gameObject.name = prefixL + fingerNames[i];
        }
        for(int i = 0; i < 5; i++)
        {
            if((fingerNames[i] != "pinky") || ((fingerNames[i] != "thumb") ))
            {
                fingertipsL[i] = GameObject.Find(prefixGlobalL + "Warped_OculusHand_L/b_" + prefixL + "wrist/b_" + prefixL + fingerNames[i] + "1/b_" + prefixL + fingerNames[i] + "2/b_" + prefixL + fingerNames[i] + "3/" + prefixL + fingerNames[i] + "_finger_tip_marker");
                fingertipsR[i] = GameObject.Find(prefixGlobalR + "Warped_OculusHand_R/b_" + prefixR + "wrist/b_" + prefixR + fingerNames[i] + "1/b_" + prefixR + fingerNames[i] + "2/b_" + prefixR + fingerNames[i] + "3/" + prefixR + fingerNames[i] + "_finger_tip_marker");              
            }
            if((i == 2) || (i == 4))
            {
                fingertipsL[i] = GameObject.Find(prefixGlobalL + "Warped_OculusHand_L/b_" + prefixL + "wrist/b_" + prefixL + fingerNames[i] + "0/b_" + prefixL + fingerNames[i] + "1/b_" + prefixL + fingerNames[i] + "2/b_" + prefixL + fingerNames[i] + "3/" + prefixL + fingerNames[i] + "_finger_tip_marker");
                fingertipsR[i] = GameObject.Find(prefixGlobalR + "Warped_OculusHand_R/b_" + prefixR + "wrist/b_" + prefixR + fingerNames[i] + "0/b_" + prefixR + fingerNames[i] + "1/b_" + prefixR + fingerNames[i] + "2/b_" + prefixR + fingerNames[i] + "3/" + prefixR + fingerNames[i] + "_finger_tip_marker"); 
            }
            
        }
        numberColliders = GameObject.FindObjectsOfType<ClosestFromAvatar>().Length;        
        state = 0;
        fsrConversion = this.GetComponent<FSRComparison>();

        warpedL = new List<bool> {false, false};
        warpedR = new List<bool> {false, false};

        camera = GameObject.Find("CenterEyeAnchor");

    }

    // Update is called once per frame
    void Update()
    {
        // anchorLBase = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor");
        // anchorRBase = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor");
        switch(state)
        {
            case 0:
                for(int i = 0; i < 5; i++)
                {
                    fingertipsR[i].AddComponent<SphereCollider>();
                    fingertipsL[i].AddComponent<SphereCollider>();

                    fingertipsR[i].transform.gameObject.AddComponent<Rigidbody>();
                    fingertipsR[i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    fingertipsR[i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    fingertipsR[i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    fingertipsR[i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
                    fingertipsR[i].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;

                    fingertipsL[i].transform.gameObject.AddComponent<Rigidbody>();
                    fingertipsL[i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    fingertipsL[i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    fingertipsL[i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    fingertipsL[i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
                    fingertipsL[i].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;

                    fingertipsR[i].AddComponent<ClosestFromAvatar>();
                    fingertipsL[i].AddComponent<ClosestFromAvatar>();
                }

                state = 1;
            break;

            case 1:

                WarpHands();

                fsrConversion = this.GetComponent<FSRComparison>();

                for(int i = 0; i < 5; i++) // GET CLOSEST POINTS - THESE ARE THE TARGETS FOR IK
                {
                    if(warpedRightHand != null)
                    {
                        closests.transform.GetChild(i+5).transform.position = fingertipsR[i].gameObject.GetComponent<ClosestFromAvatar>().closestPoint;
                    }
                    if(warpedLeftHand != null)
                    {
                        closests.transform.GetChild(i).transform.position = fingertipsL[i].gameObject.GetComponent<ClosestFromAvatar>().closestPoint;
                    }
                }
                for(int i = 0; i < 5; i++) // Fingers 
                {
                    if(!fingertipsR[i].GetComponent<ClosestFromAvatar>().inDaZone)
                    {
                        if((fingertipsR[4].gameObject.GetComponent<ClosestFromAvatar>().REAL_MIN_DISTANCE < 0.08f) && fsrConversion.isTouched)
                        {
                            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[9].enabled = true;

                            // avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[9].enabled = true;
                        }
                        else
                        {
                            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i+5].enabled = false;
                        }
                    }
                    else
                    {
                        avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i+5].enabled = true;
                    }

                    if(!fingertipsL[i].GetComponent<ClosestFromAvatar>().inDaZone)
                    {
                        if((fingertipsL[4].gameObject.GetComponent<ClosestFromAvatar>().REAL_MIN_DISTANCE < 0.08f) && fsrConversion.isTouched)
                        {
                            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[4].enabled = true;
                        }
                        else
                        {
                            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i].enabled = false;
                        }
                    }
                    else
                    {
                        avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i].enabled = true;
                    }

                }

                for(int i = 0; i < 23; i++)
                {
                    warpedRightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.position = rightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.position + warpVectorR;
                    warpedRightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles = new Vector3(rightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.x, rightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.y, rightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.z);

                    warpedLeftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.position = leftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.position + warpVectorL;
                    warpedLeftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles = new Vector3(leftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.x, leftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.y, leftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.z);
                }
            break;
        }
        
    }


    public void WarpHands()
    {
        // int layerMask = 0<< 9;
        // layerMask = ~layerMask;

        // RaycastHit hit;
        // // Does the ray intersect any objects excluding the player layer
        // if (Physics.Raycast(camera.transform.forward, camera.transform.forward, out hit, Mathf.Infinity, layerMask))
        // {
        //     Debug.DrawRay(camera.transform.position, camera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //     Debug.Log("Did Hit");
        // }
        // else
        // {
        //     Debug.DrawRay(camera.transform.position, camera.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        //     Debug.Log("Did not Hit");
        // }

        for(int i = 0; i < GameObject.Find("WarpZones").transform.childCount; i++)
        {
            warpedL[i] = GameObject.Find("WarpZones").transform.GetChild(i).gameObject.transform.gameObject.GetComponent<Collider>().bounds.Contains(GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform.position);
            warpedR[i] = GameObject.Find("WarpZones").transform.GetChild(i).gameObject.transform.gameObject.GetComponent<Collider>().bounds.Contains(GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform.position);
            // Debug.Log(warped[0] + ";" + warped[1]);// + ";" + GameObject.FindGameObjectsWithTag("WarpZone")[i].gameObject.name);
        }

        // anchorLBase.transform.position = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform.position;
        // anchorRBase.transform.position = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform.position;

        if(startWarpL)
        {
            if(!reverse)
            {
                projectVector = new Vector3((physicalTarget.position.x - resetTarget.position.x), 0, (physicalTarget.position.z - resetTarget.position.z)).normalized;
                            
                dp = Vector3.Project((physicalTarget.position - leftHand.transform.position), projectVector).magnitude;
                ds = Vector3.Project((leftHand.transform.position - resetTarget.position), projectVector).magnitude;

                warpVectorL = (ds / (ds + dp)) * (currentTarget.position - physicalTarget.position);

            }
            else
            {
                projectVector = (currentTarget.position - resetTarget.position).normalized;

                ds = Vector3.Project((leftHand.transform.position - resetTarget.position), projectVector).magnitude;
                dp = Vector3.Project((currentTarget.position - leftHand.transform.position), projectVector).magnitude;

                warpVectorL = (ds / (ds + dp)) * (physicalTarget.position - currentTarget.position);
            }
        }
        else
        {
            warpVectorL = Vector3.zero;
        }
        
        if(startWarpR)
        {
            if(!reverse)
            {
                projectVector = new Vector3((physicalTarget.position.x - resetTarget.position.x), 0, (physicalTarget.position.z - resetTarget.position.z)).normalized;
                            
                dp = Vector3.Project((physicalTarget.position - rightHand.transform.position), projectVector).magnitude;
                ds = Vector3.Project((rightHand.transform.position - resetTarget.position), projectVector).magnitude;

                warpVectorR = (ds / (ds + dp)) * (currentTarget.position - physicalTarget.position);

            }
            else
            {
                projectVector = (currentTarget.position - resetTarget.position).normalized;

                ds = Vector3.Project((rightHand.transform.position - resetTarget.position), projectVector).magnitude;
                dp = Vector3.Project((currentTarget.position - rightHand.transform.position), projectVector).magnitude;

                warpVectorR = (ds / (ds + dp)) * (physicalTarget.position - currentTarget.position);
            }
        }
        else
        {
            warpVectorR = Vector3.zero;
        }


        anchorL.transform.position = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform.position + warpVectorL;
        anchorR.transform.position = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform.position + warpVectorR;

        // warpedLeftHand.transform.position = leftHand.transform.position + warpVectorL;
        // warpedRightHand.transform.position = rightHand.transform.position + warpVectorR;

        anchorL.transform.eulerAngles = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform.eulerAngles;
        anchorR.transform.eulerAngles = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform.eulerAngles;
        

    }
}
