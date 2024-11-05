using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSRComparison : MonoBehaviour
{
    public ContactManager contactManager;
    public FollowOculusHands handManager;
    public CompareObjects gameManager;
    public float stiffness;
    public float deltaDist;

    public bool debug;

    public bool isTouched;
    public float fsrResistance;
    public float force;
    private float fsrForce;
    public HandRedirectionManager redirectManager;

    // public float resistance, fsrVoltage, appliedForce;

    void Start()
    {
        redirectManager = this.GetComponent<HandRedirectionManager>();
    	handManager = this.GetComponent<FollowOculusHands>();
        contactManager = this.GetComponent<ContactManager>();
        gameManager = this.GetComponent<CompareObjects>();
        force = 0;
        isTouched = false;
        // debug = true;
        if(contactManager.boardConnected)
        {
            debug = false;
        }
        stiffness = 10;
    }

    void Update()
    {
        // if( (redirectManager.numObject % 2) == 0)
        // {
        //     stiffness = gameManager.stiffness[0];
        // }
        

        // if((redirectManager.numObject % 2) == 1)
        // {
        //     stiffness = gameManager.stiffness[1];
        // }
        stiffness = gameManager.stiffness[redirectManager.numObject % gameManager.stiffness.Length];


        isTouched = false;
        if (Input.GetKeyDown(KeyCode.E))
        {
            contactManager.connectToArduino();
        }

        
        fsrResistance = contactManager.fsrValue;
        force = ConversionFSRValue(fsrResistance);
        deltaDist = CalculDeformationObject(force);
        
        if(fsrResistance != 0)
        {
            isTouched = true;                
        }

        
    }

    float ConversionFSRValue(float fsrResistance)
    {
        if(fsrResistance > 0)
        {
            fsrForce = 3140.8f * Mathf.Pow(fsrResistance, -0.744f);
        }
        else
        {
            fsrForce  = 0;
        }
       

        
        return fsrForce;
    }

    float CalculDeformationObject(float force)
    {
        if ((stiffness != 0) && (force >= 0.1f))
        {
            deltaDist = force / (stiffness * Mathf.Pow(10, 2)); // force - N, stiffness (N/cm) -> N/m
            // deltaDist en m
        }
        else
        {
            deltaDist = 0;
        }
        return deltaDist;
    }


}
