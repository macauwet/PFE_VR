using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InDaZone : MonoBehaviour
{
	public GameObject target;
    public Vector3 scaleMeBaby;
    public FollowOculusHands handManager;
    public HandRedirectionManager redirectManager;

    // Start is called before the first frame update
    void Start()
    {
        // scaleMeBaby = new Vector3(1.2f, 2f, 1.2f);
        scaleMeBaby = new Vector3(2f, 2f, 1.2f);
        redirectManager = GameObject.FindObjectOfType<HandRedirectionManager>();

    }

    // Update is called once per frame
    void Update()
    {
        handManager = GameObject.FindObjectOfType<FollowOculusHands>();
        if((redirectManager.numObject % 2) == 0)
        {
            target = GameObject.Find("Targets/Left");
        }
        
        if((redirectManager.numObject % 2) == 1)
        {
            target = GameObject.Find("Targets/Right");
        }
        // else
        // {
        //     target = GameObject.Find("Targets/Left");
        // }

        // if(GameObject.FindGameObjectWithTag("Target") != null)
        // {
        //     target = GameObject.FindGameObjectWithTag("Target");
            this.transform.localScale = Vector3.Scale(target.transform.localScale, scaleMeBaby);
            this.transform.position = target.transform.position;
            this.transform.eulerAngles = target.transform.eulerAngles;

            // this.GetComponent<Collider>().isTrigger = true;
            this.GetComponent<Collider>().transform.position = target.GetComponent<Collider>().transform.position;
        // }
        
    }

	void OnTriggerEnter(Collider other)
    {
    	if(other.gameObject.GetComponent<ClosestFromAvatar>() != null)
    	{
    		other.gameObject.GetComponent<ClosestFromAvatar>().inDaZone = true;
    		Debug.Log("DA ZONE");
            // target.GetComponent<Rigidbody>().isKinematic = true;

    	}

    }
    void OnTriggerStay(Collider other)
    {
    	if(other.gameObject.GetComponent<ClosestFromAvatar>() != null)
    	{
    		other.gameObject.GetComponent<ClosestFromAvatar>().inDaZone = true;
            Debug.Log("DA ZONE");
            Debug.Log(other.gameObject.name);
            // target.GetComponent<Rigidbody>().isKinematic = true;

    	}

    }
    void OnTriggerExit(Collider other)
    {
    	if(other.gameObject.GetComponent<ClosestFromAvatar>() != null)
    	{
    		other.gameObject.GetComponent<ClosestFromAvatar>().inDaZone = false;
    	}

    }

}
