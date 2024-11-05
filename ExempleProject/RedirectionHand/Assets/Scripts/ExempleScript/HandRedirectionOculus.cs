using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRedirectionOculus : MonoBehaviour
{
    [HideInInspector]
    public GameObject rightHand, leftHand, warpedRightHand, warpedLeftHand;

    private GameObject anchorL, anchorR;


    public Transform currentTarget;
    public Transform resetTarget;
    public Transform physicalTarget;

    private Vector3 warpVectorL, warpVectorR, projectVector;
    private float ds, dp;

    public  bool redirecting;
    private float speedRedirectChange=0.05f;// m/s

    //finger redirection
    private string prefixR, prefixL, prefixGlobalL, prefixGlobalR;
    private string[] fingerNames;
    private GameObject[] fingertipsR, fingertipsL;
    private GameObject closests;
    private GameObject avatarHandParent;//, anchorLBase, anchorRBase;
    public bool objectTouched;
    private int stateFinger= 0;

    void Start()
    {
        Initialisation();
    }

    private void Initialisation()
    {
        prefixR = "r_";
        prefixL = "l_";
        fingerNames = new string[5] { "index", "middle", "pinky", "ring", "thumb" };
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
        for (int i = 0; i < 5; i++)
        {
            closests.transform.GetChild(i + 5).gameObject.name = prefixR + fingerNames[i];
            closests.transform.GetChild(i).gameObject.name = prefixL + fingerNames[i];
        }
        for (int i = 0; i < 5; i++)
        {
            if ((fingerNames[i] != "pinky") || ((fingerNames[i] != "thumb")))
            {
                fingertipsL[i] = GameObject.Find(prefixGlobalL + "Warped_OculusHand_L/b_" + prefixL + "wrist/b_" + prefixL + fingerNames[i] + "1/b_" + prefixL + fingerNames[i] + "2/b_" + prefixL + fingerNames[i] + "3/" + prefixL + fingerNames[i] + "_finger_tip_marker");
                fingertipsR[i] = GameObject.Find(prefixGlobalR + "Warped_OculusHand_R/b_" + prefixR + "wrist/b_" + prefixR + fingerNames[i] + "1/b_" + prefixR + fingerNames[i] + "2/b_" + prefixR + fingerNames[i] + "3/" + prefixR + fingerNames[i] + "_finger_tip_marker");
            }
            if ((i == 2) || (i == 4))
            {
                fingertipsL[i] = GameObject.Find(prefixGlobalL + "Warped_OculusHand_L/b_" + prefixL + "wrist/b_" + prefixL + fingerNames[i] + "0/b_" + prefixL + fingerNames[i] + "1/b_" + prefixL + fingerNames[i] + "2/b_" + prefixL + fingerNames[i] + "3/" + prefixL + fingerNames[i] + "_finger_tip_marker");
                fingertipsR[i] = GameObject.Find(prefixGlobalR + "Warped_OculusHand_R/b_" + prefixR + "wrist/b_" + prefixR + fingerNames[i] + "0/b_" + prefixR + fingerNames[i] + "1/b_" + prefixR + fingerNames[i] + "2/b_" + prefixR + fingerNames[i] + "3/" + prefixR + fingerNames[i] + "_finger_tip_marker");
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        HandRedirectionUpdate();
        FinderRedirectionUpdate();
    }

    public void HandRedirectionUpdate()
    {
        
        

        if (redirecting)
        {
            // left HAND
            projectVector = new Vector3((physicalTarget.position.x - resetTarget.position.x), 0, (physicalTarget.position.z - resetTarget.position.z)).normalized;

            dp = Vector3.Project((physicalTarget.position - leftHand.transform.position), projectVector).magnitude;
            ds = Vector3.Project((leftHand.transform.position - resetTarget.position), projectVector).magnitude;

            Vector3 warpVectorgoalL =  (ds / (ds + dp)) * (currentTarget.position - physicalTarget.position);
            warpVectorL = Vector3.MoveTowards(warpVectorL, warpVectorgoalL, speedRedirectChange * Time.deltaTime);

            // right HAND
            projectVector = new Vector3((physicalTarget.position.x - resetTarget.position.x), 0, (physicalTarget.position.z - resetTarget.position.z)).normalized;

            dp = Vector3.Project((physicalTarget.position - rightHand.transform.position), projectVector).magnitude;
            ds = Vector3.Project((rightHand.transform.position - resetTarget.position), projectVector).magnitude;

            Vector3 warpVectorgoalR = (ds / (ds + dp)) * (currentTarget.position - physicalTarget.position);
            warpVectorR =Vector3.MoveTowards(warpVectorR, warpVectorgoalR, speedRedirectChange * Time.deltaTime);

        }
        else
        {
            warpVectorR = Vector3.MoveTowards(warpVectorR, Vector3.zero,speedRedirectChange*Time.deltaTime);
            warpVectorL = Vector3.MoveTowards(warpVectorR, Vector3.zero, speedRedirectChange * Time.deltaTime);
        }


        anchorL.transform.position = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform.position + warpVectorL;
        anchorR.transform.position = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform.position + warpVectorR;

        anchorL.transform.eulerAngles = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform.eulerAngles;
        anchorR.transform.eulerAngles = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform.eulerAngles;
    }

    
    public void FinderRedirectionUpdate()
    {
        switch (stateFinger)
        {
            case 0:
                for (int i = 0; i < 5; i++)
                {
                    fingertipsR[i].AddComponent<SphereCollider>();
                    fingertipsL[i].AddComponent<SphereCollider>();

                    fingertipsR[i].transform.gameObject.AddComponent<Rigidbody>();
                    fingertipsR[i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    fingertipsR[i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    fingertipsR[i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    fingertipsR[i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
                    fingertipsR[i].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f * 0.02f;

                    fingertipsL[i].transform.gameObject.AddComponent<Rigidbody>();
                    fingertipsL[i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    fingertipsL[i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    fingertipsL[i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    fingertipsL[i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
                    fingertipsL[i].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f * 0.02f;

                    fingertipsR[i].AddComponent<ClosestFromAvatarHand>();
                    fingertipsL[i].AddComponent<ClosestFromAvatarHand>();
                }

                stateFinger = 1;
                break;

            case 1:



                for (int i = 0; i < 5; i++) // GET CLOSEST POINTS - THESE ARE THE TARGETS FOR IK
                {
                    if (warpedRightHand != null)
                    {
                        closests.transform.GetChild(i + 5).transform.position = fingertipsR[i].gameObject.GetComponent<ClosestFromAvatarHand>().closestPoint;
                    }
                    if (warpedLeftHand != null)
                    {
                        closests.transform.GetChild(i).transform.position = fingertipsL[i].gameObject.GetComponent<ClosestFromAvatarHand>().closestPoint;
                    }
                }
                for (int i = 0; i < 5; i++) // Fingers 
                {
                    if (!fingertipsR[i].GetComponent<ClosestFromAvatarHand>().inDaZone)
                    {
                        if ((fingertipsR[4].gameObject.GetComponent<ClosestFromAvatarHand>().REAL_MIN_DISTANCE < 0.08f) && objectTouched)
                        {
                            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[9].enabled = true;

                            // avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[9].enabled = true;
                        }
                        else
                        {
                            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i + 5].enabled = false;
                        }
                    }
                    else
                    {
                        avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i + 5].enabled = true;
                    }

                    if (!fingertipsL[i].GetComponent<ClosestFromAvatarHand>().inDaZone)
                    {
                        if ((fingertipsL[4].gameObject.GetComponent<ClosestFromAvatarHand>().REAL_MIN_DISTANCE < 0.08f) && objectTouched)
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

                for (int i = 0; i < 23; i++)
                {
                    warpedRightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.position = rightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.position + warpVectorR;
                    warpedRightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles = new Vector3(rightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.x, rightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.y, rightHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.z);

                    warpedLeftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.position = leftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.position + warpVectorL;
                    warpedLeftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles = new Vector3(leftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.x, leftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.y, leftHand.GetComponent<OVRCustomSkeleton>().CustomBones[i].transform.eulerAngles.z);
                }
                break;
        }
    }
}
