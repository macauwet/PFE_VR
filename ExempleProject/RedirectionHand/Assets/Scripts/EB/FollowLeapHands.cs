using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.IO;
using System.Globalization;
using UnityEngine.Animations.Rigging;


public class FollowLeapHands : MonoBehaviour
{
    [Tooltip("Here slide your real tracked hand in.")]  
    public GameObject realLHand, realRHand;

    [Tooltip("Here slide your Oculus avatar \"pseudo hand\" in.")]  
    public GameObject avatarLHand, avatarRHand;

    private GameObject[] realRParents, avatarRParents;
    private GameObject[] realRBabies;
    [HideInInspector]
    public GameObject[] avatarRBabies;

    private GameObject[] realLParents, avatarLParents;
    private GameObject[] realLBabies;
    [HideInInspector]
    public GameObject[] avatarLBabies;

    public bool fsrTouching;
    private string prefixR, prefixL;

    private string[] fingerNames;
    private int etat = 0;
  
    [Tooltip("Here associate the AvatarHand based on Oculus Model. This is a parent containing 19 babies.")]  
  	public GameObject oculusHandColl;
  
    // These become the targets for IK
    private GameObject[] closests;
  
    public GameObject avatarHandParent;
    public bool inDaZoneR, inDaZoneL;
    private bool[] zoneMe;
    // public int kHand = 20;
    private int numberColliders;

    // public int spring, damper;
    // public float maxDistance;


    // Start is called before the first frame update
    void Start()
    {
        fsrTouching = false;

        realRParents = new GameObject[5];
        avatarRParents = new GameObject[5];
        realRBabies = new GameObject[26];
        avatarRBabies = new GameObject[26];

        realLParents = new GameObject[5];
        avatarLParents = new GameObject[5];
        realLBabies = new GameObject[26];
        avatarLBabies = new GameObject[26];

        closests = new GameObject[10];

        prefixR = "r_";
        prefixL = "l_";
        fingerNames = new string[5]{"index", "middle", "pinky", "ring", "thumb"};
        // fingerNames = new string[5]{"thumb", "index", "middle", "ring", "pinky"};

        // avatarHandParent = GameObject.Find("AvatarHands");

    // LEAP MOTION RIGHT COMPONENTS
      for(int i = 0; i < 5; i++)
      {
        realRParents[i] = realRHand.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
        avatarRParents[i] = avatarRHand.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
      }

      for(int i = 0; i < 4; i++) // Index to Pinky
      {
      realRBabies[5*i] = realRParents[i];
      avatarRBabies[5*i] = avatarRParents[i];
        for(int j = 0; j < 4; j++)
        {
          realRBabies[5*i + j + 1] = realRBabies[5*i + j].transform.GetChild(0).gameObject;
          avatarRBabies[5*i + j + 1] = avatarRBabies[5*i + j].transform.GetChild(0).gameObject;

        }
      }

      for(int i = 4; i < 5; i++) // Thumb
      {
      realRBabies[5*i] = realRParents[i];
      avatarRBabies[5*i] = avatarRParents[i];     
        for(int j = 0; j < 3; j++)
        {
          realRBabies[5*i + j + 1] = realRBabies[5*i + j].transform.GetChild(0).gameObject;
          avatarRBabies[5*i + j + 1] = avatarRBabies[5*i + j].transform.GetChild(0).gameObject;

        }

      }

    realRBabies[realRBabies.Length-2] = realRParents[0].transform.parent.transform.gameObject;
    avatarRBabies[avatarRBabies.Length-2] = avatarRParents[0].transform.parent.transform.gameObject;
    realRBabies[realRBabies.Length-1] = realRParents[0].transform.parent.transform.parent.transform.gameObject;
    avatarRBabies[avatarRBabies.Length-1] = avatarRParents[0].transform.parent.transform.parent.transform.gameObject;
    

    // LEAP MOTION LEFT COMPONENTS
        for(int i = 0; i < 5; i++)
      {
        realLParents[i] = realLHand.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
        avatarLParents[i] = avatarLHand.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
      }

      for(int i = 0; i < 4; i++) // Index to Pinky
      {
      realLBabies[5*i] = realLParents[i];
      avatarLBabies[5*i] = avatarLParents[i];
        for(int j = 0; j < 4; j++)
        {
          realLBabies[5*i + j + 1] = realLBabies[5*i + j].transform.GetChild(0).gameObject;
          avatarLBabies[5*i + j + 1] = avatarLBabies[5*i + j].transform.GetChild(0).gameObject;

        }
      }

      for(int i = 4; i < 5; i++) // THUMB
      {
      realLBabies[5*i] = realLParents[i];
      avatarLBabies[5*i] = avatarLParents[i];     
        for(int j = 0; j < 3; j++)
        {
          realLBabies[5*i + j + 1] = realLBabies[5*i + j].transform.GetChild(0).gameObject;
          avatarLBabies[5*i + j + 1] = avatarLBabies[5*i + j].transform.GetChild(0).gameObject;

        }

      }

    	realLBabies[realLBabies.Length-2] = realLParents[0].transform.parent.transform.gameObject;
    	avatarLBabies[avatarLBabies.Length-2] = avatarLParents[0].transform.parent.transform.gameObject;
    	realLBabies[realLBabies.Length-1] = realLParents[0].transform.parent.transform.parent.transform.gameObject;
    	avatarLBabies[avatarLBabies.Length-1] = avatarLParents[0].transform.parent.transform.parent.transform.gameObject;

      // DEFINE GAMEOBJECTS WHICH POSITION ARE THE TARGETS FOR IK
      for(int i = 0; i < 5; i++)
	    {
	      GameObject.Find("Closests").transform.GetChild(i+5).gameObject.name = prefixR + fingerNames[i];
	      GameObject.Find("Closests").transform.GetChild(i).gameObject.name = prefixL + fingerNames[i];
	      closests[i] = GameObject.Find("Closests").transform.GetChild(i).gameObject;
	      closests[i+5] = GameObject.Find("Closests").transform.GetChild(i+5).gameObject;
	    }

    }

    // Update is called once per frame
    void Update()
    {

      if(Input.GetKeyDown(KeyCode.A))
      {
        fsrTouching = !fsrTouching;
      }
      zoneMe = new bool[numberColliders];
      inDaZoneR = false;
      inDaZoneL = false;
      // avatarHandParent = GameObject.Find("AvatarHands");


      switch(etat)
      {
        case 0:

          // ADD COLLIDERS TO THE RIGHT AVATAR HAND
          for(int i = 0; i < 3; i++) // thumb
          {
            // THUMB
            avatarRBabies[20+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarRBabies[20+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarRBabies[20+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarRBabies[20+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarRBabies[20+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarRBabies[20+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarRBabies[20+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(4+i).GetComponent<CapsuleCollider>().center;
            avatarRBabies[20+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(4+i).GetComponent<CapsuleCollider>().radius;
            avatarRBabies[20+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(4+i).GetComponent<CapsuleCollider>().height;
            avatarRBabies[20+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(4+i).GetComponent<CapsuleCollider>().direction;
          
            // INDEX
            avatarRBabies[1+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarRBabies[1+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarRBabies[1+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarRBabies[1+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarRBabies[1+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarRBabies[1+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarRBabies[1+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(7+i).GetComponent<CapsuleCollider>().center;
            avatarRBabies[1+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(7+i).GetComponent<CapsuleCollider>().radius;
            avatarRBabies[1+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(7+i).GetComponent<CapsuleCollider>().height;
            avatarRBabies[1+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(7+i).GetComponent<CapsuleCollider>().direction;

            // MIDDLE
            avatarRBabies[6+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarRBabies[6+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarRBabies[6+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarRBabies[6+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarRBabies[6+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarRBabies[6+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarRBabies[6+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(10+i).GetComponent<CapsuleCollider>().center;
            avatarRBabies[6+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(10+i).GetComponent<CapsuleCollider>().radius;
            avatarRBabies[6+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(10+i).GetComponent<CapsuleCollider>().height;
            avatarRBabies[6+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(10+i).GetComponent<CapsuleCollider>().direction;
          
            // RING
            avatarRBabies[16+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarRBabies[16+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarRBabies[16+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarRBabies[16+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarRBabies[16+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarRBabies[16+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarRBabies[16+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(13+i).GetComponent<CapsuleCollider>().center;
            avatarRBabies[16+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(13+i).GetComponent<CapsuleCollider>().radius;
            avatarRBabies[16+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(13+i).GetComponent<CapsuleCollider>().height;
            avatarRBabies[16+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(13+i).GetComponent<CapsuleCollider>().direction;
          

            // PINKY
            avatarRBabies[11+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarRBabies[11+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarRBabies[11+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarRBabies[11+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarRBabies[11+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarRBabies[11+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarRBabies[11+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(16+i).GetComponent<CapsuleCollider>().center;
            avatarRBabies[11+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(16+i).GetComponent<CapsuleCollider>().radius;
            avatarRBabies[11+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(16+i).GetComponent<CapsuleCollider>().height;
            avatarRBabies[11+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(16+i).GetComponent<CapsuleCollider>().direction;
          
          }

          // FINGERTIPS
          for(int i = 1; i < 5; i++)
          {
            avatarRBabies[5*i-1].transform.gameObject.AddComponent<SphereCollider>();
            avatarRBabies[5*i-1].transform.gameObject.AddComponent<Rigidbody>();
            avatarRBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarRBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarRBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarRBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
            avatarRBabies[5*i-1].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;


            realRBabies[5*i-1].transform.gameObject.AddComponent<SphereCollider>();
            realRBabies[5*i-1].transform.gameObject.AddComponent<Rigidbody>();
            realRBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            realRBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            realRBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            realRBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
            realRBabies[5*i-1].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;
            realRBabies[5*i-1].transform.gameObject.GetComponent<SphereCollider>().isTrigger = true;
            realRBabies[5*i-1].transform.gameObject.AddComponent<AttachToFingertips>();


          }
          
          // THUMB TIP
          avatarRBabies[23].transform.gameObject.AddComponent<SphereCollider>();
          avatarRBabies[23].transform.gameObject.AddComponent<Rigidbody>();
          avatarRBabies[23].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
          avatarRBabies[23].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
          avatarRBabies[23].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
          avatarRBabies[23].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
          avatarRBabies[23].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;

          realRBabies[23].transform.gameObject.AddComponent<SphereCollider>();
          realRBabies[23].transform.gameObject.AddComponent<Rigidbody>();
          realRBabies[23].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
          realRBabies[23].transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
          realRBabies[23].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
          realRBabies[23].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
          realRBabies[23].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;
          realRBabies[23].transform.gameObject.GetComponent<SphereCollider>().isTrigger = true;
          realRBabies[23].transform.gameObject.AddComponent<AttachToFingertips>();

          // ADD COLLIDERS TO THE RIGHT AVATAR HAND
          for(int i = 0; i < 3; i++) // thumb
          {
            // THUMB
            avatarLBabies[20+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarLBabies[20+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarLBabies[20+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarLBabies[20+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarLBabies[20+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarLBabies[20+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarLBabies[20+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(4+i).GetComponent<CapsuleCollider>().center;
            avatarLBabies[20+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(4+i).GetComponent<CapsuleCollider>().radius;
            avatarLBabies[20+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(4+i).GetComponent<CapsuleCollider>().height;
            avatarLBabies[20+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(4+i).GetComponent<CapsuleCollider>().direction;
          
            // INDEX
            avatarLBabies[1+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarLBabies[1+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarLBabies[1+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarLBabies[1+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarLBabies[1+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarLBabies[1+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarLBabies[1+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(7+i).GetComponent<CapsuleCollider>().center;
            avatarLBabies[1+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(7+i).GetComponent<CapsuleCollider>().radius;
            avatarLBabies[1+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(7+i).GetComponent<CapsuleCollider>().height;
            avatarLBabies[1+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(7+i).GetComponent<CapsuleCollider>().direction;

            // MIDDLE
            avatarLBabies[6+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarLBabies[6+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarLBabies[6+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarLBabies[6+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarLBabies[6+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarLBabies[6+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarLBabies[6+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(10+i).GetComponent<CapsuleCollider>().center;
            avatarLBabies[6+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(10+i).GetComponent<CapsuleCollider>().radius;
            avatarLBabies[6+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(10+i).GetComponent<CapsuleCollider>().height;
            avatarLBabies[6+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(10+i).GetComponent<CapsuleCollider>().direction;
          
            // RING
            avatarLBabies[16+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarLBabies[16+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarLBabies[16+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarLBabies[16+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarLBabies[16+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarLBabies[16+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarLBabies[16+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(13+i).GetComponent<CapsuleCollider>().center;
            avatarLBabies[16+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(13+i).GetComponent<CapsuleCollider>().radius;
            avatarLBabies[16+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(13+i).GetComponent<CapsuleCollider>().height;
            avatarLBabies[16+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(13+i).GetComponent<CapsuleCollider>().direction;
          

            // PINKY
            avatarLBabies[11+i].transform.gameObject.AddComponent<CapsuleCollider>();
            avatarLBabies[11+i].transform.gameObject.AddComponent<Rigidbody>();
            avatarLBabies[11+i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarLBabies[11+i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarLBabies[11+i].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarLBabies[11+i].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;


            avatarLBabies[11+i].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(16+i).GetComponent<CapsuleCollider>().center;
            avatarLBabies[11+i].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(16+i).GetComponent<CapsuleCollider>().radius;
            avatarLBabies[11+i].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(16+i).GetComponent<CapsuleCollider>().height;
            avatarLBabies[11+i].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(16+i).GetComponent<CapsuleCollider>().direction;
          
          }

          // FINGERTIPS
          for(int i = 1; i < 5; i++)
          {
            avatarLBabies[5*i-1].transform.gameObject.AddComponent<SphereCollider>();
            avatarLBabies[5*i-1].transform.gameObject.AddComponent<Rigidbody>();
            avatarLBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            avatarLBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            avatarLBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            avatarLBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
            avatarLBabies[5*i-1].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;


            realLBabies[5*i-1].transform.gameObject.AddComponent<SphereCollider>();
            realLBabies[5*i-1].transform.gameObject.AddComponent<Rigidbody>();
            realLBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            realLBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            realLBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            realLBabies[5*i-1].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
            realLBabies[5*i-1].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;
            realLBabies[5*i-1].transform.gameObject.GetComponent<SphereCollider>().isTrigger = true;
            realLBabies[5*i-1].transform.gameObject.AddComponent<AttachToFingertips>();
          }
          
          // THUMB TIP
          avatarLBabies[23].transform.gameObject.AddComponent<SphereCollider>();
          avatarLBabies[23].transform.gameObject.AddComponent<Rigidbody>();
          avatarLBabies[23].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
          avatarLBabies[23].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
          avatarLBabies[23].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
          avatarLBabies[23].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
          avatarLBabies[23].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;

          realLBabies[23].transform.gameObject.AddComponent<SphereCollider>();
          realLBabies[23].transform.gameObject.AddComponent<Rigidbody>();
          realLBabies[23].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
          realLBabies[23].transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
          realLBabies[23].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
          realLBabies[23].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
          realLBabies[23].transform.gameObject.GetComponent<SphereCollider>().radius = 0.5f*0.02f;
          realLBabies[23].transform.gameObject.GetComponent<SphereCollider>().isTrigger = true;
          realLBabies[23].transform.gameObject.AddComponent<AttachToFingertips>();

          // BASES
          avatarLBabies[24].transform.gameObject.AddComponent<CapsuleCollider>();
          avatarLBabies[24].transform.gameObject.AddComponent<Rigidbody>();
          avatarLBabies[24].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
          avatarLBabies[24].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
          avatarLBabies[24].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
          avatarLBabies[24].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
          avatarLBabies[24].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(2).GetComponent<CapsuleCollider>().center;
          avatarLBabies[24].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(2).GetComponent<CapsuleCollider>().radius;
          avatarLBabies[24].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(2).GetComponent<CapsuleCollider>().height;
          avatarLBabies[24].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(2).GetComponent<CapsuleCollider>().direction;
          
          avatarLBabies[25].transform.gameObject.AddComponent<CapsuleCollider>();
          avatarLBabies[25].transform.gameObject.AddComponent<Rigidbody>();
          avatarLBabies[25].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
          avatarLBabies[25].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
          avatarLBabies[25].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
          avatarLBabies[25].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
          avatarLBabies[25].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(3).GetComponent<CapsuleCollider>().center;
          avatarLBabies[25].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(3).GetComponent<CapsuleCollider>().radius;
          avatarLBabies[25].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(3).GetComponent<CapsuleCollider>().height;
          avatarLBabies[25].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(3).GetComponent<CapsuleCollider>().direction;
          
          avatarRBabies[24].transform.gameObject.AddComponent<CapsuleCollider>();
          avatarRBabies[24].transform.gameObject.AddComponent<Rigidbody>();
          avatarRBabies[24].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
          avatarRBabies[24].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
          avatarRBabies[24].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
          avatarRBabies[24].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
          avatarRBabies[24].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(2).GetComponent<CapsuleCollider>().center;
          avatarRBabies[24].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(2).GetComponent<CapsuleCollider>().radius;
          avatarRBabies[24].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(2).GetComponent<CapsuleCollider>().height;
          avatarRBabies[24].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(2).GetComponent<CapsuleCollider>().direction;
          
          avatarRBabies[25].transform.gameObject.AddComponent<CapsuleCollider>();
          avatarRBabies[25].transform.gameObject.AddComponent<Rigidbody>();
          avatarRBabies[25].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
          avatarRBabies[25].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
          avatarRBabies[25].transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
          avatarRBabies[25].transform.gameObject.GetComponent<Rigidbody>().mass = 1.0f;
          avatarRBabies[25].transform.gameObject.GetComponent<CapsuleCollider>().center = oculusHandColl.transform.GetChild(3).GetComponent<CapsuleCollider>().center;
          avatarRBabies[25].transform.gameObject.GetComponent<CapsuleCollider>().radius = oculusHandColl.transform.GetChild(3).GetComponent<CapsuleCollider>().radius;
          avatarRBabies[25].transform.gameObject.GetComponent<CapsuleCollider>().height = oculusHandColl.transform.GetChild(3).GetComponent<CapsuleCollider>().height;
          avatarRBabies[25].transform.gameObject.GetComponent<CapsuleCollider>().direction = oculusHandColl.transform.GetChild(3).GetComponent<CapsuleCollider>().direction;
        
          // for(int i = 0; i < 26; i++)
          // {
          //   if(avatarRBabies[i].GetComponent<Collider>() != null)
          //   {
          //     avatarRBabies[i].AddComponent<ClosestFromAvatar>();
          //     avatarLBabies[i].AddComponent<ClosestFromAvatar>();
          //   }
          // }

          for(int i = 0; i < 4; i++)
          {
            avatarRBabies[4 + 5*i].AddComponent<ClosestFromAvatar>();
            
            avatarLBabies[4 + 5*i].AddComponent<ClosestFromAvatar>();
          }
          avatarRBabies[23].AddComponent<ClosestFromAvatar>();
          avatarLBabies[23].AddComponent<ClosestFromAvatar>();

          // for(int i = 0; i < 26; i++)
          // {
          //   avatarRBabies[i].AddComponent<ClosestFromAvatar>();
          //   avatarLBabies[i].AddComponent<ClosestFromAvatar>();
          // }
          numberColliders = GameObject.FindObjectsOfType<ClosestFromAvatar>().Length;
          oculusHandColl.SetActive(false);
          etat = 1;
        break;


        case 1:
          // Define if it's close to the object of interest
          for(int i = 0; i < numberColliders/2; i++)
          {
            zoneMe[i] = GameObject.FindObjectsOfType<ClosestFromAvatar>()[i].inDaZone;
            zoneMe[i+numberColliders/2] = GameObject.FindObjectsOfType<ClosestFromAvatar>()[i].inDaZone;
          }
          for(int i = 0; i < numberColliders/2; i++)
          {
          	if(zoneMe[2*i + 1])
          	{
          		inDaZoneR = true;
          	}
          	if(zoneMe[2*i])
          	{
          		inDaZoneL = true;
          	}
          }

          for(int i = 0; i < 4; i++) // GET CLOSEST POINTS - THESE ARE THE TARGETS FOR IK
          {
            if(avatarRBabies[4 + 5*i].gameObject != null)
            {
              closests[i+5].transform.position = avatarRBabies[4 + 5*i].gameObject.GetComponent<ClosestFromAvatar>().closestPointBoundaries;
            }
            if(avatarLBabies[4 + 5*i].gameObject != null)
            {
              closests[i].transform.position = avatarLBabies[4 + 5*i].gameObject.GetComponent<ClosestFromAvatar>().closestPointBoundaries;
            }
          }
          closests[9].transform.position = avatarRBabies[23].gameObject.GetComponent<ClosestFromAvatar>().closestPointBoundaries;
          closests[4].transform.position = avatarLBabies[23].gameObject.GetComponent<ClosestFromAvatar>().closestPointBoundaries;


          for(int i = 0; i < 26; i++)
          {
            if(i < 25)
            {
              avatarLBabies[i].transform.localPosition = realLBabies[i].transform.localPosition;
              avatarRBabies[i].transform.localPosition = realRBabies[i].transform.localPosition;

            }
           avatarRBabies[i].transform.eulerAngles = realRBabies[i].transform.eulerAngles;
           avatarLBabies[i].transform.eulerAngles = realLBabies[i].transform.eulerAngles;
          
          }

          // if(!inDaZoneR)
          // {
	        for(int i = 0; i < 4; i++) // Fingers 
  		    {
            if(!avatarRBabies[4 + 5*i].GetComponent<ClosestFromAvatar>().inDaZone)
            {
              avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i+5].enabled = false;
            }
            else
            {
              avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i+5].enabled = true;
            }

            if(!avatarLBabies[4 + 5*i].GetComponent<ClosestFromAvatar>().inDaZone)
            {
              avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i].enabled = false;
            }
            else
            {
              avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i].enabled = true;
            }
            
  		    }
          if(!avatarRBabies[23].GetComponent<ClosestFromAvatar>().inDaZone)
          {
            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[9].enabled = false;
          }
          else
          {
            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[9].enabled = true;
          }

          if(!avatarLBabies[23].GetComponent<ClosestFromAvatar>().inDaZone)
          {
            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[4].enabled = false;
          }
          else
          {
            avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[4].enabled = true;
          }
      	  // }
      	  // else
      	  // {
      	  //   for(int i = 0; i < 5; i++) // Fingers 
      		 //  {
      			//   avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i+5].enabled = true;
      		 //  }
      	  // }

         //  if(!inDaZoneL)
         //  {
    	    //   for(int i = 0; i < 5; i++) // Fingers 
    		   //  {
    	    //   	avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i].enabled = false;
    		   //  }
    		      
      	  // }
      	  // else
      	  // {
      	  // 	for(int i = 0; i < 5; i++) // Fingers 
    	    //   {
         //    	avatarHandParent.GetComponent<DefineClosestToFingertips>().fingerIKinematics[i].enabled = true;
    	    //   }
      	  // }
        break;

      }
    }

    void OnDrawGizmos()
    {
      // GameObject.Find("Generic Hand_Left/baseMeshHand_Left_GRP/GenericHand").GetComponent<SkinnedMeshRenderer>().enabled = true;
      // GameObject.Find("Generic Hand_Right/baseMeshHand_Left_GRP/GenericHand").GetComponent<SkinnedMeshRenderer>().enabled = true;

    }
}
