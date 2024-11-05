using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHands : MonoBehaviour
{
	public bool enterCollision;
	public bool buttonsUp;
	public GameObject[] capsules;
	private List<(int,int)> isTouched;
    // Start is called before the first frame update
    void Start()
    {
    	capsules = new GameObject[2];
		isTouched = new List<(int, int)>{};
    }

    // Update is called once per frame
    void Update()
    {
    	enterCollision = false;
    	buttonsUp = false;

    	capsules = new GameObject[] {GameObject.Find("WarpedLeftHandColl"), GameObject.Find("WarpedRightHandColl")};
    	

		if(isTouched.Count != 0)
		{
			enterCollision = true;
		}
		else
		{
			buttonsUp = true;
		}
    	
		// for(int j = 0; j < capsules[0].transform.childCount; j++)
		// {
		// 	if(isTouched.Count != 0)
		// 	{
		// 		leftHandWarp = true;
		// 	}
		// 	else
		// 	{
		// 		buttonsUp = true;
		// 	}
		// }
    	
		// for(int j = 0; j < capsules[1].transform.childCount; j++)
		// {
		// 	if(isTouchedR.Count != 0)
		// 	{
		// 		rightHandWarp = true;
		// 	}
		// 	else
		// 	{
		// 		buttonsUp = true;
		// 	}
		// }
    	
    	

    }

    void OnTriggerEnter(Collider colliderInfo)
    {

		for(int j = 0; j < capsules[0].transform.childCount; j++)
		{
			if(colliderInfo.gameObject == capsules[0].transform.GetChild(j).gameObject)
			{
				isTouched.Add((0,j));;
			}
		}

    	for(int j = 0; j < capsules[1].transform.childCount; j++)
		{
			if(colliderInfo.gameObject == capsules[1].transform.GetChild(j).gameObject)
			{
				isTouched.Add((1,j));;
			}
		}
    	
    }

    void OnCollisionStay()
    {

    }

    void OnTriggerExit(Collider colliderInfo)
    {

		for(int j = 0; j < capsules[0].transform.childCount; j++)
		{
			if(colliderInfo.gameObject == capsules[0].transform.GetChild(j).gameObject)
			{
				isTouched.Remove((0,j));;
			}
		}

    	
		for(int j = 0; j < capsules[1].transform.childCount; j++)
		{
			if(colliderInfo.gameObject == capsules[1].transform.GetChild(j).gameObject)
			{
				isTouched.Remove((1,j));;
			}
		}
    	
    }
}
