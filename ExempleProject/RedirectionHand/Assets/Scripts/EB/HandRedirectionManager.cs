using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HandRedirectionManager : MonoBehaviour
{
    // public HandRedirection rightHand;
    // public HandRedirection leftHand;
    public FollowOculusHands handManager;

    public Transform currentTarget;
    public Transform physicalTarget;
    public Transform resetTarget;

    public TriggerHands buttonNext;

    public int numObject=0;//, newNumObject;
    public Transform[] targets;//, objects;
    // public Transform[] destinations;

    private Vector3[] targetOriginPos, targetOriginRot;//, destinationOriginPos;

    private bool grasped, pressStart;
    public int state = 0;
    private Color oldColor;

    // public bool blocOver = false;


    // public TextMeshPro debugMe;
    private bool forceBased = true;

    // public float stiffness;
    private bool collisionWithNext = false;
    private int oldNumObject = 0;

    // Start is called before the first frame update
    void Start()
    {

        numObject = 0;
        // newNumObject = 1;
        state = 0;
        targets = new Transform[GameObject.Find("Targets").transform.childCount]; // REDIRIGES

        targetOriginPos = new Vector3[GameObject.Find("Targets").transform.childCount];
        targetOriginRot = new Vector3[GameObject.Find("Targets").transform.childCount];

        for(int i = 0; i < GameObject.Find("Targets").transform.childCount; i++)
        {
            targets[i] = GameObject.Find("Targets").transform.GetChild(i).transform;
        }

        physicalTarget = GameObject.Find("RealPosition").transform; // ALWAYS SAME POSITION
        currentTarget = targets[numObject]; // ICI JUST LEFT AND RIGHT - si warp[0]: numObject = 0; else numOBject = 1

        handManager = this.GetComponent<FollowOculusHands>();

        buttonNext = GameObject.FindObjectOfType<TriggerHands>();
    }

    void Update()
    {
        UpdateTargets();
    }


    void UpdateTargets()
    {
        if(buttonNext.enterCollision)
        {
            collisionWithNext = true;
        }

        if(collisionWithNext && buttonNext.buttonsUp)
        {
            state = -1;
            collisionWithNext = false;
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            state = -1;
        }

        switch(state)
        {
            case -1:
                StartCoroutine(LoadOtherObject());
                state = 0;
                break;

            case 0:
                break;

            case 1:
                oldNumObject = numObject;
                break;

        }


        // if press next - target is the other

        if((numObject % 2) == 0)
        {
            GameObject.Find("WarpZones/Right").GetComponent<Collider>().enabled = false;
            GameObject.Find("WarpZones/Left").GetComponent<Collider>().enabled = true;
            if(handManager.warpedL[0])
            {
                handManager.startWarpL = true;
                handManager.reverse = false;
            }
            else
            {
                handManager.startWarpL = false;
            }

            if(handManager.warpedR[0])
            {
                handManager.startWarpR = true;
                handManager.reverse = false;
            }
            else
            {
                handManager.startWarpR = false;
            }
        }
        else
        {
            GameObject.Find("WarpZones/Right").GetComponent<Collider>().enabled = true;
            GameObject.Find("WarpZones/Left").GetComponent<Collider>().enabled = false;
            if(handManager.warpedL[1])
            {
                handManager.startWarpL = true;
                handManager.reverse = false;
            }
            else
            {
                handManager.startWarpL = false;
            }

            if (handManager.warpedR[1])
            {
                handManager.startWarpR = true;
                handManager.reverse = false;
            }
            else
            {
                handManager.startWarpR = false;
            }
        }

        // if(!handManager.warpedL[0] && !handManager.warpedL[1])
        // {
        //     handManager.startWarpL = false;
        // }
        // if(!handManager.warpedR[0] && !handManager.warpedR[1])
        // {
        //     handManager.startWarpR = false;
        // }


        // }


        // if(handManager.warpedR[1])
        // {
        //     numObject = 2;
        //     handManager.startWarpR = true;
        //     handManager.reverse = true;
        // }
        // else if(handManager.warpedR[0])
        // {
        //     numObject = 1;
        //     handManager.startWarpR = true;
        //     handManager.reverse = false;
        // }
        // else
        // {
        //     handManager.startWarpR = false;
        // }


        // if(handManager.warpedL[1])
        // {
        //     numObject = 2;
        //     handManager.startWarpL = true;
        //     handManager.reverse = true;
        // }
        // else if(handManager.warpedL[0])
        // {
        //     numObject = 1;
        //     handManager.startWarpL = true;
        //     handManager.reverse = false;
        // }
        // else
        // {
        //     handManager.startWarpL = false;
        // }
        
        currentTarget = targets[numObject%2];
        if((numObject % 2) == 0)
        {
            for(int i = 0; i < targets[1].childCount; i++)
            {
                targets[0].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
                targets[1].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }

        }
        else
        {
            for(int i = 0; i < targets[1].childCount; i++)
            {
                targets[0].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                targets[1].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }

        }
        
        handManager.currentTarget = currentTarget;
        handManager.physicalTarget = physicalTarget.transform;
        handManager.resetTarget = resetTarget.transform;

        // debugMe.text = "Etat : " + state.ToString() + "; Stiff " + rightCollider.stiffness.ToString();
    }

    IEnumerator LoadOtherObject()
    {
        yield return new WaitForSeconds(0.2f);
        numObject = oldNumObject + 1;
        state = 1;
    }

}
