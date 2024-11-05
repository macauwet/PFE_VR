using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class DefineClosestPoint : MonoBehaviour
{

	public CCDIK[] fingerIKinematics;
	public GameObject targetMe;
	public GameObject[] closestToFinger;

    void Awake()
    {
        fingerIKinematics = new CCDIK[5];
        fingerIKinematics = this.GetComponents<CCDIK>();
        closestToFinger = new GameObject[5];       
        targetMe = GameObject.FindGameObjectWithTag("Target");
    }

    // Update is called once per frame
    void Update()
    {

        targetMe = GameObject.FindGameObjectWithTag("Target");

        for(int i = 0; i < 5; i++)
        {
    	 	fingerIKinematics[i].solver.target = GameObject.Find("Targets").transform.GetChild(3 + i*5).transform;
    	 	
        }
    }
}
