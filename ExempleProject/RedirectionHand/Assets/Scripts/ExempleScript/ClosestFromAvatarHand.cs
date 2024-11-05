using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestFromAvatarHand : MonoBehaviour
{
    public static GameObject engageClosest;
    public bool phalanxContact, inDaZone;

    [HideInInspector]
    public Vector3 closestDist, closestPoint, closestPointBoundaries;

    public float REAL_MIN_DISTANCE;
    private int stateMe;
    public Vector3 posOnTable;
    // Start is called before the first frame update

    void Start()
    {
        closestPoint = new Vector3();
        closestDist = new Vector3();
        REAL_MIN_DISTANCE = new float();
        inDaZone = false;
    }

    // Update is called once per frame
    void Update()
    {
        


        REAL_MIN_DISTANCE = new float();
        closestPoint = new Vector3();
        closestDist = new Vector3();

        closestPoint = engageClosest.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);
        closestDist = engageClosest.GetComponent<Collider>().ClosestPoint(closestPoint);

        REAL_MIN_DISTANCE = Vector3.Distance(closestDist, this.transform.position);

        switch (stateMe)
        {
            case 0:
                closestPointBoundaries = closestPoint;
                // Debug.Log(this.name + " is touching.");
                stateMe = 1;
                break;

            case 1:
                if (engageClosest.GetComponent<Collider>().bounds.Contains(this.transform.position))
                {
                    this.transform.position = closestPoint; //engageClosest.GetComponent<Collider>().ClosestPoint(this.transform.position);
                }
                break;
        }
        // }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo != null && collisionInfo.gameObject.tag == "Target")
        {
            // graspContact = true;
            stateMe = 0;
        }
        if (collisionInfo != null && collisionInfo.gameObject.tag == "Table")
        {
            posOnTable = this.transform.position;
        }
    }


    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo != null && collisionInfo.gameObject.tag == "Target")
        {
            // this.transform.position = closestPointBoundaries;
            stateMe = 0;

            // engageClosest.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            if ((REAL_MIN_DISTANCE < 0.001f))
            {
                this.phalanxContact = true;
            }
            else
            {
                this.phalanxContact = false;
            }

        }
        if (collisionInfo != null && collisionInfo.gameObject.tag == "Table")
        {
            this.transform.position = posOnTable;
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo != null && collisionInfo.gameObject.tag == "Target")
        {
            this.phalanxContact = false;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(closestPoint, 0.008f);

    }
}
