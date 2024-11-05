using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CalibOffsetDir : MonoBehaviour
{
	public enum fwdV {trackerForward, trackerUp, trackerRight};
    public enum upV {trackerForward, trackerUp, trackerRight};

    [Serializable]
    public struct directionTracker
    {
		public fwdV forwardVector;
		public bool inverseFwd;
		public upV upVector;
		public bool inverseUp;
    }

    public directionTracker myDirectionTracker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
