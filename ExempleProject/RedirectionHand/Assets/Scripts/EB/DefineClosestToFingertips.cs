using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class DefineClosestToFingertips : MonoBehaviour
{

	public CCDIK[] fingerIKinematics;
	public CCDIK[] fingerLIKinematics;
	public CCDIK[] fingerRIKinematics;

	private string[] fingerNames;
	public GameObject targetMe;
	// public GameObject[] closestToFinger;
	private string prefix, suffix;

    public HandRedirectionManager redirectManager;

    void Awake()
    {
        fingerIKinematics = new CCDIK[10];
        fingerIKinematics = this.GetComponents<CCDIK>();

        fingerLIKinematics = new CCDIK[5];
        for (int i = 0; i < 5; i++)
        {
            fingerLIKinematics[i] = fingerIKinematics[i];
        }

        fingerRIKinematics = new CCDIK[5];
        for (int i = 0; i < 5; i++)
        {
            fingerRIKinematics[i] = fingerIKinematics[i + 5];
        }
       

    }

    void Update()
    {
        


        // targetMe = GameObject.FindGameObjectWithTag("Target");

        for (int i = 0; i < 5; i++)
        {
    	 	fingerIKinematics[i].solver.target = GameObject.Find("Closests").transform.GetChild(i).transform;
    	 	fingerIKinematics[i+5].solver.target = GameObject.Find("Closests").transform.GetChild(i+5).transform;
        }
    }
}
