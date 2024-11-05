using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoSpring : MonoBehaviour
{
    public ServoManager servoManager;

    [Range(0.0f, 1.0f)]
    public float targetPos;
    public float realPos, realCmd;
    public float speedCmd, accelCmd;

    public float signalMin = 1472;
    public float signalMax = 8281;

    [Range(0.0f, 10000.0f)]
   	public float kProxy;

   	public bool proxyOn;
   	public GameObject proxyObject, servoRealPos;
   	public GameObject courseMax, courseMin;

   	public float speedSet;

    [Range(0.0f, 500.0f)]
   	public float kForce;
   	private float inputKForce;

    public bool pseudo = false;

    private float alpha, r, d1, d2;
    public float dim_interface;

    public bool debug;
    // public FSRConversion fsrCommand;
    // public InstantiateConditions gameManager;

    
    void Start()
    {
		servoManager = GameObject.FindObjectOfType<ServoManager>();

        // Representation servomoteur
        servoRealPos = GameObject.Find("ServoRealPos");
		proxyObject = GameObject.Find("Proxy");
		courseMax =  GameObject.Find("TargetMax");
		courseMin = GameObject.Find("TargetMin");

        r = 0.015f;
        d1 = 0.023f;
        d2 = 0.014f;

        // fsrCommand = GameObject.FindObjectOfType<FSRConversion>();
        // gameManager = GameObject.FindObjectOfType<InstantiateConditions>();
    }

    // Update is called once per frame
    void Update()
    {
    	if(Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(CalibrateMe());
        }

        if(servoManager.pololu == null)
        {
            Debug.Log("RECONNECT ME");
            servoManager.pololu = servoManager.connectToPololuBoard();

        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("RECONNECT ME");
            servoManager.pololu = servoManager.connectToPololuBoard();

        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Proxy On");
            proxyOn = !proxyOn;
            // courseTarget.GetComponent<BoxCollider>().isTrigger = false;
        }

        realPos = servoManager.tryGetPosition()[3];
        speedCmd = servoManager.tryGetPosition()[1];
    	accelCmd = servoManager.tryGetPosition()[2];
		realCmd = servoManager.tryGetPosition()[0];

        proxyObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;

        // adjust the kforce to the object 
		// kForce = fsrCommand.force; // Add force from FSR Conversion here 
  //       pseudo = gameManager.pseudo; // Add pseudo from GameManager



        if(Input.GetKeyDown(KeyCode.A))
        {
        	debug = !debug;
        }
        if(debug)
        {
        	if(Input.GetKey(KeyCode.Keypad6))// || deformObject.isPressed)
			{
				proxyObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
	            inputKForce = kForce;
			}
			if(Input.GetKey(KeyCode.Keypad4))// || !deformObject.isPressed)
			{
				proxyObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
	            inputKForce = -kForce;
			}
        }
        else
        {
        	if(!pseudo)
			{
				proxyObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
	            inputKForce = kForce;
			}
			else
			{
				proxyObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
	            inputKForce = -kForce;
			}
        }
		

		proxyObject.GetComponent<Rigidbody>().AddForce(Vector3.right*inputKForce);

        if(proxyOn)
        {
        	Vector3 destination = (servoRealPos.transform.position - proxyObject.transform.position);
        	speedSet = - (destination.x*kProxy);
        	
        	if(Mathf.Abs(speedSet) >= 255f)
        	{
        		speedSet = 255f;
            }
        	
        	if(Mathf.Abs(speedSet) <= 1f)
        	{
        		speedSet = 1f;
            }

            servoManager.updateSpeed(Mathf.Abs(speedSet));
            servoManager.updateAccel(Mathf.Abs(speedSet));
            
            
            targetPos = (courseMax.transform.position.x - proxyObject.transform.position.x) / (courseMax.transform.position.x - courseMin.transform.position.x);
            // Debug.Log(targetPos + ";" + ((signalMax - realCmd)/(signalMax - signalMin)));
            //Pseudo is active, the target position isn't send to the servomotor
            if(!pseudo)
            {
                servoManager.updateServo(targetPos);
            }
            // Debug.Log((1-targetPos) + ";" + ((signalMax - realCmd)/(signalMax - signalMin)) + ";" +  (signalMax - realPos)/(signalMax - signalMin));

            servoRealPos.transform.position = courseMin.transform.position + (courseMax.transform.position - courseMin.transform.position) * (signalMax - realPos)/(signalMax - signalMin);
            CalculInterfaceDimension();
        }
    }

    void CalculInterfaceDimension()
    {
        alpha = 90 * (1 - targetPos);

        // Radian conversion
        alpha = alpha * Mathf.PI / 180;

        dim_interface = ((Mathf.Sqrt(d1 * d1 - r * r * Mathf.Sin(alpha) * Mathf.Sin(alpha)) + r * Mathf.Cos(alpha)) + d2) * 2;
    }

    IEnumerator CalibrateMe()
    {
        servoManager.updateSpeed(255);
        servoManager.updateAccel(255);
        servoManager.updateServo(0);
        yield return new  WaitForSeconds(2.0f);
        signalMin = servoManager.tryGetPosition()[3];
        Debug.Log("SignalMin: " + signalMin);
        StartCoroutine(CalibrateMeMax());
    }

    IEnumerator CalibrateMeMax()
    {
        servoManager.updateServo(1);
        yield return new  WaitForSeconds(2.0f);
        signalMax = servoManager.tryGetPosition()[3];
        Debug.Log("SignalMax: " + signalMax);
    }
}
