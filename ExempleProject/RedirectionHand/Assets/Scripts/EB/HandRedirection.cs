using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRedirection : MonoBehaviour
{
    public Transform warpedHand;
    public Transform currentTarget;
    public Transform resetTarget;
    public Transform physicalTarget;

    public bool reverse;
    private Vector3 warpVector, projectVector;
    private float ds, dp;

    public bool startWarp;
    public List<bool> warped;
    public Vector3 vectorTest;

    void Start()
    {
        warped = new List<bool> {false, false};
    }


    public void LateUpdate()
    {

        if (currentTarget == null || resetTarget == null || !enabled)
            return;

        // Do the redirection here
        //current transform.position has the position of the tracked hand
        //Update transform.position

        for(int i = 0; i < GameObject.FindGameObjectsWithTag("WarpZone").Length; i++)
        {
            warped[i] = GameObject.FindGameObjectsWithTag("WarpZone")[i].transform.gameObject.GetComponent<Collider>().bounds.Contains(this.transform.position);
            Debug.Log(warped[0] + ";" + warped[1]);// + ";" + GameObject.FindGameObjectsWithTag("WarpZone")[i].gameObject.name);
        }

        if(startWarp)
        {
            if(!reverse)
            {
                projectVector = new Vector3((physicalTarget.position.x - resetTarget.position.x), 0, (physicalTarget.position.z - resetTarget.position.z)).normalized;
                            
                dp = Vector3.Project((physicalTarget.position - this.transform.position), projectVector).magnitude;
                ds = Vector3.Project((this.transform.position - resetTarget.position), projectVector).magnitude;

                warpVector = (ds / (ds + dp)) * (currentTarget.position - physicalTarget.position);

            }
            else
            {
                projectVector = (currentTarget.position - resetTarget.position).normalized;

                ds = Vector3.Project((this.transform.position - resetTarget.position), projectVector).magnitude;
                dp = Vector3.Project((currentTarget.position - this.transform.position), projectVector).magnitude;

                warpVector = (ds / (ds + dp)) * (physicalTarget.position - currentTarget.position);
                Debug.Log(warpVector);
            }
        }
        else
        {
            warpVector = Vector3.zero;
        }
        
       
        this.transform.position = this.transform.position + warpVector;

    }
}
