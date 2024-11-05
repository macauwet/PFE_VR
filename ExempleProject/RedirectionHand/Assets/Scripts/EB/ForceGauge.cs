using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForceGauge : MonoBehaviour
{

	public FSRConversion fsrConversion;
	public float forceRead;
	public TextMeshPro textForce, textStiff;
    // Start is called before the first frame update
    void Start()
    {
        textForce = this.transform.GetChild(2).GetComponent<TextMeshPro>();
        textStiff = this.transform.GetChild(3).GetComponent<TextMeshPro>();

    }

    // Update is called once per frame
    void Update()
    {
		fsrConversion = GameObject.FindObjectOfType<FSRConversion>();

		forceRead = fsrConversion.force;
        this.transform.GetChild(1).transform.eulerAngles = new Vector3(0, 180, -80f + 160/30*forceRead);
        textForce.text = "Force : " + forceRead.ToString("F0") + " N";
        textStiff.text = "Stiffness : " + fsrConversion.stiffness.ToString("F0") + " N/cm";

    }
}
