using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToHand : MonoBehaviour
{

	public GameObject engageClosest;
	public bool phalanxContact, graspContact;

	[HideInInspector]
	public Vector3 closestDist, closestPoint;
	
	public float REAL_MIN_DISTANCE;
    // Start is called before the first frame update

    void Start()
    {
        
    	graspContact = false;
    	closestPoint = new Vector3();
    	closestDist = new Vector3();
    	REAL_MIN_DISTANCE = new float();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(GameObject.FindObjectOfType<DefineClosestPoint>() != null)
        {
            engageClosest = GameObject.FindObjectOfType<DefineClosestPoint>().targetMe;
        }
        if(GameObject.FindObjectOfType<DefineClosestToFingertips>() != null)
        {
            engageClosest = GameObject.FindObjectOfType<DefineClosestToFingertips>().targetMe;
        }
        
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

    void OnCollisionEnter(Collision collisionInfo) 
    {
        if(collisionInfo != null)
        {
            graspContact = true;
        }     
    }


    void OnCollisionStay(Collision collisionInfo) 
    {
		if(collisionInfo != null)
		{
            engageClosest.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            engageClosest.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

		   if((REAL_MIN_DISTANCE < 0.0005f))
            {
                this.phalanxContact = true;
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(closestDist, 0.01f);

    }

}