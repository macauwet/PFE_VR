using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressNext : MonoBehaviour
{
	public bool next;
	public InstantiateConditions gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<InstantiateConditions>();
    }

    // Update is called once per frame
    void Update()
    {
        // next = false;
        Debug.Log(next);
    }

    void OnTriggerEnter(Collider other)
    {

		next = true;
        Debug.Log(next);
       
    }
    void OnTriggerStay(Collider other)
    {
    	if(other.gameObject.tag == "leftHand")
    	{
    		next = false;
    	}

    }

}
