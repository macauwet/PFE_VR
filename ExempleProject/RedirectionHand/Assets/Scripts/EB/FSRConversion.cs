using UnityEngine;

public class FSRConversion : MonoBehaviour
{
    public ContactManager contactManager;
    public InstantiateConditions gameManager;
    public float stiffness;
    public float deltaDist;

    public bool debug;

    public bool isTouched;
    public float fsrResistance;
    public float force;
    private float fsrForce;
    // public float resistance, fsrVoltage, appliedForce;

    void Start()
    {
    	gameManager = GameObject.FindObjectOfType<InstantiateConditions>();
        contactManager = GameObject.FindObjectOfType<ContactManager>();
        force = 0;
        isTouched = false;
        debug = true;
        if(contactManager.boardConnected)
        {
            debug = false;
        }
    }

    void Update()
    {
    	stiffness = gameManager.stiffnessToTouch;
        isTouched = false;
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     debug = !debug;
        // }
        if (Input.GetKeyDown(KeyCode.E))
        {
            contactManager.connectToArduino();
        }

        if (debug)
        {
            // force = ConversionFSRValue(fsrResistance);
            deltaDist = CalculDeformationObject(force);
            
            if(fsrResistance != 0)
            {
                isTouched = true;                
            }
            // if (Input.GetKey(KeyCode.Keypad6))
            // {
            //     force = force + 0.01f;
            //     Debug.Log(force);
            // }
            
            // if (Input.GetKey(KeyCode.Keypad4))
            // {
            //     force = force - 0.01f;
            //     Debug.Log(force);
            // }
        }
        else
        {
            fsrResistance = contactManager.fsrValue;
            force = ConversionFSRValue(fsrResistance);
            deltaDist = CalculDeformationObject(force);
            
            if(fsrResistance != 0)
            {
                isTouched = true;                
            }

        }
        
    }

    float ConversionFSRValue(float fsrResistance)
    {
        if(fsrResistance > 0)
        {
            fsrForce = 6629.7f * Mathf.Pow(fsrResistance, -0.727f);
            // if(fsrResistance > 125 * 1000)
            // {
            //     fsrForce = 6522.7f * Mathf.Pow(fsrResistance, -1.354f);
            // }
            // else
            // {
            //     fsrForce = 773.36f * Mathf.Pow(fsrResistance, -0.766f);
            // }
        }
        else
        {
            fsrForce  = 0;
        }
        if(fsrForce > 30)
        {
            fsrForce = 0;
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
