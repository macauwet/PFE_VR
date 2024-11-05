using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.IO;
using System.Globalization;

public class GenerateFingertipsColl : MonoBehaviour
{

	public GameObject trackedHand;

	public GameObject[] firstBabies;
	public GameObject[] allBabies;

	public int etat;
    private GameObject[] closests;

	private string[] fingerNames;


    // Start is called before the first frame update
    void Start()
    {
        firstBabies = new GameObject[5];
    	allBabies = new GameObject[24];
    	closests = new GameObject[5];
        fingerNames = new string[5]{"thumb", "index", "middle", "ring", "pinky"};

      
      	etat = 0;

		for(int i = 0; i < 5; i++)
    	{
    		firstBabies[i] = trackedHand.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
    	}

    	for(int i = 0; i < 1; i++)
    	{
			allBabies[4*i] = firstBabies[i];
    		for(int j = 0; j < 3; j++)
    		{
    			allBabies[4*i + j + 1] = allBabies[4*i + j].transform.GetChild(0).gameObject;

    		}
    	}

    	for(int i = 1; i < 2; i++)
    	{
			allBabies[4*i] = firstBabies[i];
    		for(int j = 0; j < 4; j++)
    		{
    			allBabies[4*i + j + 1] = allBabies[4*i + j].transform.GetChild(0).gameObject;
    		}
    	}

    	for(int i = 2; i < 5; i++)
    	{
			allBabies[5*i-1] = firstBabies[i];
    		for(int j = 1; j < 5; j++)
    		{
    			allBabies[5*i-1 + j] = allBabies[5*i-1 + j - 1].transform.GetChild(0).gameObject;

    		}

    	}
    	for(int i = 0; i < 5; i++)
		{
			GameObject.Find("Closests").transform.GetChild(i).gameObject.name = fingerNames[i];
			closests[i] = GameObject.Find("Closests").transform.GetChild(i).gameObject;
		}
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 5; i++) // Every finger's parent
    	{
    		firstBabies[i] = trackedHand.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
    	}

    	for(int i = 0; i < 1; i++) // Big thumb
    	{
			allBabies[4*i] = firstBabies[i];
    		for(int j = 0; j < 3; j++)
    		{
    			allBabies[4*i + j + 1] = allBabies[4*i + j].transform.GetChild(0).gameObject;

    		}
    	}

    	for(int i = 1; i < 2; i++) // Index
    	{
			allBabies[4*i] = firstBabies[i];
    		for(int j = 0; j < 4; j++)
    		{
    			allBabies[4*i + j + 1] = allBabies[4*i + j].transform.GetChild(0).gameObject;
    		}
    	}

    	for(int i = 2; i < 5; i++) // All the other ones
    	{
			allBabies[5*i-1] = firstBabies[i];
    		for(int j = 1; j < 5; j++)
    		{
    			allBabies[5*i-1 + j] = allBabies[5*i-1 + j - 1].transform.GetChild(0).gameObject;

    		}

    	}

    	switch(etat)
    	{
    		case 0:
	    		for(int i = 0; i < 5; i++) // Fingers 
		    	{
		    		for(int j = 3; j < 4; j++) // Fingertips
		    		{
		    			allBabies[5*i + j].transform.gameObject.AddComponent<SphereCollider>();
		    			allBabies[5*i + j].transform.gameObject.AddComponent<Rigidbody>();
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
		    			allBabies[5*i + j].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;

		    			allBabies[5*i + j].transform.gameObject.AddComponent<AttachToFingertips>();
		    		}
		    	}

		    	etat = 1;
    		break;


    		case 1:
    		// Target Positions
    			for(int i = 0; i < 5; i++)
    			{
    				closests[i].transform.position = allBabies[3 + 5*i].gameObject.GetComponent<AttachToFingertips>().closestDist;
    			}
    		break;
		}
    }
}
