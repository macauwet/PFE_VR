using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLogic : MonoBehaviour
{
    private int state;
    private bool collisionWithNext;
    public Transform[] targets;
    private Vector3[] targetOriginPos, targetOriginRot;

    private int oldNumObject = 0;
    public Transform currentTarget;
    private HandRedirectionOculus handManager;
    private TriggerHands buttonNext;
    public Transform physicalTarget;
    public Transform resetTarget;
    public int numObject=0;
    // Start is called before the first frame update
    void Start()
    {

        numObject = 0;
        state = 0;
        targets = new Transform[GameObject.Find("Targets").transform.childCount]; // REDIRIGES

        targetOriginPos = new Vector3[GameObject.Find("Targets").transform.childCount];
        targetOriginRot = new Vector3[GameObject.Find("Targets").transform.childCount];

        for (int i = 0; i < GameObject.Find("Targets").transform.childCount; i++)
        {
            targets[i] = GameObject.Find("Targets").transform.GetChild(i).transform;
        }

        physicalTarget = GameObject.Find("RealPosition").transform; // ALWAYS SAME POSITION
        currentTarget = targets[numObject]; // ICI JUST LEFT AND RIGHT - si warp[0]: numObject = 0; else numOBject = 1

        handManager = this.GetComponent<HandRedirectionOculus>();

        buttonNext = GameObject.FindObjectOfType<TriggerHands>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargets();
    }


    void UpdateTargets()
    {
        if (buttonNext.enterCollision)
        {
            collisionWithNext = true;
        }

        if (collisionWithNext && buttonNext.buttonsUp)
        {
            state = -1;
            collisionWithNext = false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            state = -1;
        }

        switch (state)
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

        currentTarget = targets[numObject % 2];
        ClosestFromAvatarHand.engageClosest = currentTarget.gameObject;
        handManager.physicalTarget = physicalTarget.transform;
        handManager.currentTarget = currentTarget;
        
        handManager.resetTarget = resetTarget.transform;
        if ((numObject % 2) == 0)
        {
            for (int i = 0; i < targets[1].childCount; i++)
            {
                targets[0].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
                targets[1].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }

        }
        else
        {
            for (int i = 0; i < targets[1].childCount; i++)
            {
                targets[0].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                targets[1].transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }

        }
        
    }


    IEnumerator LoadOtherObject()
    {
        yield return new WaitForSeconds(0.2f);
        numObject = oldNumObject + 1;
        state = 1;
    }
}
