using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToFingertips : MonoBehaviour
{

	public GameObject engageClosest;
	public bool phalanxContact, graspContact;

	[HideInInspector]
	public Vector3 closestDist, closestPoint, closestPointBoundaries;
	
	public float REAL_MIN_DISTANCE;
    private int stateMe;
    private Vector3 posWhenTouching;
    // Start is called before the first frame update

    void Start()
    {
        
    	graspContact = false;
    	closestPoint = new Vector3();
    	closestDist = new Vector3();
    	REAL_MIN_DISTANCE = new float();
        stateMe = 1;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(GameObject.FindGameObjectWithTag("Target") != null)
        {
            engageClosest = GameObject.FindGameObjectWithTag("Target");
            graspContact = false;

            REAL_MIN_DISTANCE = new float();
            closestPoint = new Vector3();
            closestDist = new Vector3();
           
            closestPoint = engageClosest.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);
            closestDist = engageClosest.GetComponent<Collider>().ClosestPoint(closestPoint);

            REAL_MIN_DISTANCE = Vector3.Distance(closestDist, this.transform.position);

            if((REAL_MIN_DISTANCE < 0.001f) && phalanxContact)
            {
                graspContact = true;

            }
        }
		
        switch(stateMe)
        {
            case 0:
                closestPointBoundaries = closestDist;
                posWhenTouching = this.transform.position;
                // Debug.Log(this.name + "is touching.");
                stateMe = 1;
            break;

            case 1:

            break;
        }



   	}

    void OnCollisionEnter(Collision collisionInfo) 
    {
        if(collisionInfo != null && collisionInfo.gameObject.tag == "Target")
        {
            graspContact = true;
            stateMe = 0;
        }     
    }


    void OnCollisionStay(Collision collisionInfo) 
    {
		if(collisionInfo != null)
		{
            // engageClosest.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            // engageClosest.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

		   if((REAL_MIN_DISTANCE < 0.0005f))
            {
                this.phalanxContact = true;
                this.transform.position = posWhenTouching;
            }
            else
            {

            }

       }
            	
           
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if(collisionInfo != null)
		{
			this.phalanxContact = false;
    	}
    }


    void OnDrawGizmos()
    {
        // Gizmos.color = Color.cyan;
        // Gizmos.DrawSphere(closestDist, 0.01f);

        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(closestPointBoundaries, 0.01f);

    }

}