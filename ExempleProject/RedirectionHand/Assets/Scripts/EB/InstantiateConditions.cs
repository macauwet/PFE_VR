using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using Deform;
using Valve.VR;

public class InstantiateConditions : MonoBehaviour
{
	public bool boolStart;
	public string userName;
	private string time0;

	public bool pilot;
	public bool paramImage;
 	public int conditionPilot = 0;

 	public Slider minimumSlider, maximumSlider;

	public bool pseudo;//, trackerOn;
	public Vector3 offsetPos, offsetPosS;
	public bool quest;

	[Serializable]	
 	public struct StiffnessInterval {
	    public int minStiffness;
	    public int maxStiffness;
	    public int increment;
 	}
	[Header("Stiffness (N/cm)")]
	public StiffnessInterval[] stiffnessRange;

	[Serializable]	
 	public struct TasksChoice {
	    public string nameTask;
	    public string questionMark;
	    public int nbTask;
 	}

 	[Tooltip("Squeeze")]
 	[Header("Tasks")]
 	public TasksChoice[] declaredTasks;

	[Serializable]	
 	public struct ObjectChoice {
	    public string objectName;
	    public int nbObject;
 	}
 	
 	[Header("Objects")]
 	public ObjectChoice[] declaredObjects;

	[Header("Scales")]
	public Vector3[] scales;

	private string[] tasks;
    [HideInInspector]
	public GameObject[] objectTypes;
	public List<int> configException;
	public List<float> allStiffness;


    [Range(0, 127)]
    public int config;
    public Vector3 scaleObj;
    public GameObject objToCatch;
    public string taskToDo;
    public float stiffnessToTouch;
    private int taskNumber, objId;
    private GameObject conditionsObj, conditionsObj2;
    private float stiffnessObj;

    public int nbBloc = 0;
    public int nbBlocMax = 1;
    public int state;

    private Transform[] walls;
    private TextMeshPro[] otherwalls;
    [HideInInspector]
	public GameObject Text3D;

	private GameObject panelQuestion;
	public int nbTouch = 0;

	private int correctBug = 0;
	private bool startOver = false;
	// private int secondBug = 0;

 	private Vector3 posOrigin, rotOrigin;
 	private Vector3 rotCylinder;
 	private int sliderRate;

 	private int[] results; // Array: results are list of 4 conditions/ array size is config size
 	public int[] resultsToWrite;
 	private GameObject slideOnMe;
 	private GameObject panelSliderTxt;
 	private bool showPanel, increase, decrease, leftOrRight, confidenceLevel;
 	private int[] forcedChoice;
 	public int[] forcedChoiceToWrite;
 	private float[] maxForces;
 	public float[] maxForcesToWrite;

 	private int etatIntro;
 	private bool finishIntro, finishedRating, firstChoice, validatedChoice;

 	private int introChoice = 0;
 	private int etatPilot = 0;
 	private int totalTrialPilot = 0;
 	private float incrementPilot = 10;
 	private float twoConditions = 0;
 	private float currentMaxPilot, currentMinPilot;
 	private List<float> staircaseList;
 	private bool pilotUp;
 	private ControllerUI controllerUI;
 	private GameObject calib;
 	private FSRConversion fsrConvert;

 	public Material materialChoosing, materialNoChoice, materialValidation;

	private string path;
	private StreamWriter writer;

	private Vector3[] scaleImage;
	public float stiffnessVideo;

    void Start()
    {
    	posOrigin = new Vector3();
    	rotCylinder = new Vector3();

		// TASKS
		tasks = new string[declaredTasks.Length];
		for(int i = 0; i < declaredTasks.Length; i++)
		{
			tasks[declaredTasks[i].nbTask] = declaredTasks[i].nameTask;
		}

		// OBJECTS
		objectTypes = new GameObject[declaredObjects.Length];
		for(int i = 0; i < declaredObjects.Length; i++)
		{
			objectTypes[declaredObjects[i].nbObject] = GameObject.Find("Primitives/" + declaredObjects[i].objectName);
		}

		scaleImage = new Vector3[objectTypes.Length];
		for(int i = 0; i < objectTypes.Length; i++)
		{

			scaleImage[i] = GameObject.Find("Primitives/" + declaredObjects[i].objectName).transform.localScale;
		}


    	for(int i = 0; i < objectTypes.Length; i++)
    	{
	    	objectTypes[i].SetActive(false);
    	}

    	// STIFFNESS
    	for(int i = 0; i < stiffnessRange.Length; i++)
    	{
    		for(int j = stiffnessRange[i].minStiffness; j <= stiffnessRange[i].maxStiffness; j = j + stiffnessRange[i].increment)
    		{
    			if(!allStiffness.Contains(j))
    			allStiffness.Add(j);
    		}
    	}

		configException = new List<int>();

		config = UnityEngine.Random.Range(0, tasks.Length*objectTypes.Length*scales.Length*allStiffness.Count);
    	panelQuestion = GameObject.Find("PanelQuestion"); //.SetActive(false);

		results = new int[scales.Length*tasks.Length*objectTypes.Length*allStiffness.Count];
		forcedChoice = new int[scales.Length*tasks.Length*objectTypes.Length*allStiffness.Count];
		maxForces = new float[scales.Length*tasks.Length*objectTypes.Length*allStiffness.Count];

		for(int i = 0; i < results.Length; i++)
		{
			results[i] = 3;
		}
		for(int i = 0; i < forcedChoice.Length; i++)
		{
			forcedChoice[i] = 3;
		}
		for(int i = 0; i < maxForces.Length; i++)
		{
			maxForces[i] = 0;
		}

		resultsToWrite = new int[scales.Length*tasks.Length*objectTypes.Length*nbBlocMax*allStiffness.Count];
		forcedChoiceToWrite = new int[scales.Length*tasks.Length*objectTypes.Length*nbBlocMax*allStiffness.Count];
		maxForcesToWrite = new float[scales.Length*tasks.Length*objectTypes.Length*nbBlocMax*allStiffness.Count];
		
		for(int i = 0; i < resultsToWrite.Length; i++)
		{
			resultsToWrite[i] = 3;
		}
		for(int i = 0; i < forcedChoiceToWrite.Length; i++)
		{
			forcedChoiceToWrite[i] = 3;
		}
		for(int i = 0; i < maxForcesToWrite.Length; i++)
		{
			maxForcesToWrite[i] = 0;
		}

		slideOnMe = new GameObject();
		panelSliderTxt = new GameObject();
		sliderRate = new int();

		slideOnMe = GameObject.Find("PanelQuestion/Canvas").transform.GetComponentInChildren<Slider>().gameObject;
		panelSliderTxt =  GameObject.Find("PanelQuestion/Slider_Txt").transform.gameObject;


		walls = GameObject.Find("Walls").GetComponentsInChildren<Transform>();
		Text3D = GameObject.Find("Text (TMP)");
		otherwalls = new TextMeshPro[walls.Length];

    	for (int i = 1; i < otherwalls.Length; i++)
    	{
	    	otherwalls[i] = (TextMeshPro)Instantiate(Text3D, walls[i].gameObject.transform).GetComponent<TextMeshPro>();
	    	otherwalls[i].transform.localPosition = new Vector3(0, 0, -0.6f);
	    	otherwalls[i].transform.eulerAngles = new Vector3(walls[i].gameObject.transform.eulerAngles.x, walls[i].gameObject.transform.eulerAngles.y, walls[i].gameObject.transform.eulerAngles.z);
	    	otherwalls[i].transform.localScale = new Vector3(0.04f, 0.07f, 1f);
	    	otherwalls[i].fontSize = 8f;
	 
    	}
		etatIntro = 0;
		firstChoice = false;
		validatedChoice = false;
		leftOrRight = true;
		confidenceLevel = false;
		// speedaccel = GameObject.FindObjectOfType<SpeedAccelPololu>();
		controllerUI = this.GetComponent<ControllerUI>();


		time0 = System.DateTime.Now.ToString("ddMMyyyy-HHmm");
		calib = GameObject.Find("Calib");

		if(pilot)
		{
			state = -3;
			calib.SetActive(false);
		}
		else
		{

			GameObject.Find("SliderPilot").SetActive(false);
			// path = "Assets/Resources/DataCollection/UX/" + userName + "-" + time0 + "-Configs.csv";
       
			// writer = new StreamWriter(path, true);
			// writer.WriteLine("Config");
			// writer.Close();
			state = -4;
		}
		offsetPos = new Vector3(-0.2187f, 0.012f, 0.0085f);
		offsetPosS = new Vector3(-0.289f, 0.012f, 0.0085f);

		
    }

    // Update is called once per frame
    void Update()
    {
    	pseudo = true;
		// for(int i = 0; i < allStiffness.Count; i++)
		// {
	 //    	print(allStiffness[i]);

		// }
		
		if(!quest)
		{
			if(Input.GetKeyDown(KeyCode.Keypad1))
	    	{
	    		offsetPos.y = GameObject.Find("topCover").transform.position.y;
	    		posOrigin = new Vector3(GameObject.Find("Controller (right)").transform.position.x + offsetPos.x, offsetPos.y, GameObject.Find("Controller (right)").transform.position.z + offsetPos.z);
	    		// offsetPos = new Vector3(0.06f, -0.05f, -0.1f);
	    		if(conditionsObj != null)
	    		{
	    			conditionsObj.transform.position = posOrigin;
	    		}
			}

		}
		else
		{
			if(Input.GetKeyDown(KeyCode.Z))
	    	{
	    		// posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPos.y, GameObject.Find("CalibrateMe").transform.position.z + offsetPos.z);
	    		// rotOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.eulerAngles.x, GameObject.Find("CalibrateMe").transform.eulerAngles.y, GameObject.Find("CalibrateMe").transform.eulerAngles.z);

	    		rotOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.eulerAngles.x, GameObject.Find("CalibrateMe").transform.eulerAngles.y, GameObject.Find("CalibrateMe").transform.eulerAngles.z);


	    		if(conditionsObj != null)
	    		{
	    			offsetPos = new Vector3(-0.2187f, 0.012f, 0.009f);
	    			offsetPosS = new Vector3(-0.289f, 0.012f, 0.009f);

	    			if(!pilot)
	    			{
	    				if(scaleObj == scales[0])
		    			{
				    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.z + offsetPos.z);
		    			}
		    			else
		    			{
				    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPosS.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPosS.x, GameObject.Find("CalibrateMe").transform.position.z + offsetPosS.z);

		    			}
	    			}
	    			else
	    			{
	    				if(scales[Mathf.FloorToInt(conditionPilot/2)] == scales[0])
		    			{
				    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.y + 0.013f, GameObject.Find("CalibrateMe").transform.position.z + offsetPos.z);
		    
		    			}
		    			else
		    			{
				    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPosS.x, GameObject.Find("CalibrateMe").transform.position.y + 0.013f, GameObject.Find("CalibrateMe").transform.position.z + offsetPosS.z);

		    			}
	    			}
	    			
	    			conditionsObj.transform.position = posOrigin;
	    		}

			}
		}

    	
		controllerUI = this.GetComponent<ControllerUI>();

		

    	// Going through i, j, k, l and assigning tasks/objects/scales/stiffness to configurations numbers.
    	if(!pilot)
		{

	    	for(int l = 0; l < allStiffness.Count; l++)
	    	{
	    		if((config < tasks.Length*objectTypes.Length*scales.Length*(l+1)) && (config >= tasks.Length*objectTypes.Length*scales.Length*l))
				{
					stiffnessToTouch = allStiffness[l];
					// Debug.Log("Stiffness range: " + tasks.Length*objectTypes.Length*scales.Length*(l+1) + "-" + tasks.Length*objectTypes.Length*scales.Length*l + "\n Stiffness: " + allStiffness[l]);
		    		for(int k = 0; k < scales.Length; k++)
					{
						if(config < (tasks.Length*objectTypes.Length*scales.Length*l + (k+1)*tasks.Length*objectTypes.Length) && config >= (tasks.Length*objectTypes.Length*scales.Length*l + k*tasks.Length*objectTypes.Length))
						{
							scaleObj = scales[k];
							for(int j = 0; j < objectTypes.Length; j++)
							{
								// Debug.Log("Range Obj: " + (tasks.Length*objectTypes.Length*scales.Length*l + k*tasks.Length*objectTypes.Length + (j+1)*tasks.Length) + "-" + (tasks.Length*objectTypes.Length*scales.Length*l + k*tasks.Length*objectTypes.Length + j*tasks.Length));
								if((config < (l*tasks.Length*objectTypes.Length*scales.Length + k*tasks.Length*objectTypes.Length + (j+1)*tasks.Length)) && (config >= (tasks.Length*objectTypes.Length*scales.Length*l + k*tasks.Length*objectTypes.Length + j*tasks.Length)))
								{
									objToCatch = objectTypes[j];
									objId = j;
									for(int m = 0; m < tasks.Length; m++)
									{
										if((config < (tasks.Length*objectTypes.Length*scales.Length*l + k*tasks.Length*objectTypes.Length + j*tasks.Length + m+1)) && (config >= (tasks.Length*objectTypes.Length*scales.Length*l + k*tasks.Length*objectTypes.Length + j*tasks.Length + m)))
										{
											taskToDo = tasks[m];
											taskNumber = m;
										}
									}
								}

							}
						}
							
						// Debug.Log("Config: " + config + "\n Stiffness: " + stiffnessToTouch);
						// Debug.Log("Scale: " + scaleObj);
						// Debug.Log("Obj: " + objToCatch + "\n Task: " + taskToDo);					
					}

				}
	    	}
    	}		

		if(controllerUI.increase)
		{
			increase = true;
		}
		if(controllerUI.decrease)
		{
			decrease = true;
		}

		if(controllerUI.next)
		{
			if(state == 2)
			{
				state = 3;
			}
		}



		for(int m = 0; m < configException.Count; m++)
		{
			for(int p = 0; p < configException.Count; p++)
			{
				if(p != m)
				{
					if(configException[m] == configException[p])
					{
						configException.Remove(configException[m]);
					}
				}
			}
		}
		
		if(correctBug >= 1)
		{
			state = 3;
		}

		startOver = false;

		if(!showPanel)
		{
			for(int k = 0; k < panelQuestion.GetComponentsInChildren<MeshRenderer>().Length; k++)
	    	{
				panelQuestion.GetComponentsInChildren<MeshRenderer>()[k].enabled = false;
	    	}
	    	slideOnMe.SetActive(false);
			panelSliderTxt.GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			if(confidenceLevel)
			{
				for(int k = 0; k < panelQuestion.GetComponentsInChildren<MeshRenderer>().Length; k++)
		    	{
		    		panelQuestion.GetComponentsInChildren<MeshRenderer>()[k].enabled = true;
		    	}
				slideOnMe.SetActive(true);
				panelSliderTxt.GetComponent<MeshRenderer>().enabled = true;
			}
			else
			{
				panelQuestion.GetComponentsInChildren<MeshRenderer>()[0].enabled = true;				
				for(int k = 1; k < 4; k++)
		    	{
		    		panelQuestion.GetComponentsInChildren<MeshRenderer>()[k].enabled = false;
		    	}
		    	for(int k = 4; k < panelQuestion.GetComponentsInChildren<MeshRenderer>().Length; k++)
		    	{
		    		panelQuestion.GetComponentsInChildren<MeshRenderer>()[k].enabled = true;
		    	}
		    	slideOnMe.SetActive(false);
				panelSliderTxt.GetComponent<MeshRenderer>().enabled = false;
			}
			
		}


		if(increase)
		{
			if(confidenceLevel)
			{
				if(slideOnMe.GetComponent<Slider>().value < 5)
				{
					slideOnMe.GetComponent<Slider>().value = slideOnMe.GetComponent<Slider>().value + 1;
		    		results[config] = sliderRate;
					increase = false;
				}
				else
				{
					slideOnMe.GetComponent<Slider>().value = 5;
		    		results[config] = sliderRate;
					increase = false;
				}
			}

		}

		if(decrease)
		{
			if(confidenceLevel)
			{
				if(slideOnMe.GetComponent<Slider>().value > 1)
				{
					slideOnMe.GetComponent<Slider>().value = slideOnMe.GetComponent<Slider>().value - 1;
		    		results[config] = sliderRate;
					decrease = false;
				}
				else
				{
					slideOnMe.GetComponent<Slider>().value = 1;
		    		results[config] = sliderRate;
					decrease = false;
				}
			}
		}



		switch(state)
		{

			case -5: // game master recharges last config
				if(GameObject.FindGameObjectsWithTag("Target").Length != 0)
				{
					for(int i = 0; i < GameObject.FindGameObjectsWithTag("Target").Length; i++)
					{
						Destroy(GameObject.FindGameObjectsWithTag("Target")[i]);
					}
					showPanel = false;
					configException.Remove(config);
				}
				config = configException[configException.Count-1];

				state = 0;

				
			break;

			case -4: // CALIBRATION
				calib.SetActive(true);
				offsetPos = new Vector3(-0.2187f, 0.012f, 0.0085f);

	    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPos.y, GameObject.Find("CalibrateMe").transform.position.z + 0.0095f);
	    		rotOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.eulerAngles.x, GameObject.Find("CalibrateMe").transform.eulerAngles.y, GameObject.Find("CalibrateMe").transform.eulerAngles.z);

				calib.transform.position = posOrigin;
				calib.transform.eulerAngles = rotOrigin;
				controllerUI = this.GetComponent<ControllerUI>();
				showPanel = true;

				config = 0;

				StartCoroutine(ConsignesIntro());
				switch(etatIntro)
				{
					case 0:
						if(this.GetComponent<FSRConversion>().isTouched == true)
						{
							etatIntro = 1;
						}
					break;

					case 1:
						if(leftOrRight)
						{
							if(controllerUI.increase || controllerUI.decrease)
							{
								firstChoice = true;
							}
							if(firstChoice)
							{
								confidenceLevel = false;
								if(controllerUI.decrease)
								{					
									introChoice = 0;	
									GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
									GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialChoosing;	
								}
								if(controllerUI.increase)
								{			
									introChoice = 1;			
									GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialChoosing;
									GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;	
								}

								
								if(controllerUI.next)
								{
									validatedChoice = true;
									leftOrRight = false;
									etatIntro = 2;
								}
							}
							else
							{
								GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
								GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;	
							}
						}

					break;

					case 2:
						if(controllerUI.reset)
						{
							firstChoice = false;
							validatedChoice = false;
							leftOrRight = true;
							confidenceLevel = false;
							etatIntro = 3;
						}
					break;

					case 3:
						// config = 0;
						if(leftOrRight)
						{
							if(controllerUI.increase || controllerUI.decrease)
							{
								firstChoice = true;
							}
							if(firstChoice)
							{
								confidenceLevel = false;
								if(controllerUI.decrease)
								{					
									introChoice = 0;	
									GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
									GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialChoosing;	
								}
								if(controllerUI.increase)
								{			
									introChoice = 1;			
									GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialChoosing;
									GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;	
								}

								
								if(controllerUI.next)
								{
									validatedChoice = true;
									leftOrRight = false;
									etatIntro = 4;
								}
							}
							else
							{
								GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
								GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;	
							}
						}


					break;

					case 4:
						if(confidenceLevel)
						{
							StartCoroutine(WaitInIntro());
						}
					break;

					case 5:
						if(controllerUI.next)
						{
							StartCoroutine(WaitABit());
						}
					break;
				}

				if(validatedChoice)
				{
					if(introChoice == 0)
					{
						GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
						GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialValidation;
					}
					else
					{
						GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialValidation;
						GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;
					}
					confidenceLevel = true;
				}

				if(Input.GetKeyDown(KeyCode.Space))
				{
					Destroy(calib);
					state = -1;
				}

			break;

			case -3: // Pilot Study
				StartCoroutine(ConsignesPilot());

				switch(etatPilot)
				{
					case -1:
						totalTrialPilot = totalTrialPilot + 1;
						//Write results
						path = "Assets/Resources/DataCollection/Pilot/" + userName + "-" + time0 + "-" + conditionPilot + ".csv";
						writer = new StreamWriter(path, true);
						writer.WriteLine("Stiffness Staircase");
						for(int i = 0; i < staircaseList.Count; i++)
						{
							writer.WriteLine(staircaseList[i]);
						}
						writer.Close();
						staircaseList.Clear();
						staircaseList = new List<float>();

						if(totalTrialPilot == scales.Length*2)
						{
							StartCoroutine(EndOfTheGame());
						}

						conditionPilot = conditionPilot + 1;
						if(conditionPilot == scales.Length*2) // end of latin square
						{
							conditionPilot = 0;
						}
						etatPilot = 0;

					break;

					case 0:
						conditionsObj = (GameObject)Instantiate((objectTypes[0]), GameObject.Find("ObjectsOfInterest").transform);
						conditionsObj.transform.localScale = scales[Mathf.FloorToInt(conditionPilot/2)];

						if(scales[Mathf.FloorToInt(conditionPilot/2)] == scales[0])
		    			{
				    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.y + 0.013f, GameObject.Find("CalibrateMe").transform.position.z + offsetPos.z);
		    			}
		    			else
		    			{
				    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPosS.x, GameObject.Find("CalibrateMe").transform.position.y + 0.013f, GameObject.Find("CalibrateMe").transform.position.z + offsetPosS.z);
		    			}

		    			conditionsObj.transform.position = posOrigin;
						conditionsObj.transform.eulerAngles = rotOrigin;

						conditionsObj.tag = "Target";
						conditionsObj.AddComponent<Deformable>();
						conditionsObj.AddComponent<SquashAndStretchDeformer>();
						conditionsObj.GetComponent<Deformable>().AddDeformer(conditionsObj.GetComponent<SquashAndStretchDeformer>());// = GameObject.FindObjectOfType<Deformer>();
						
						// for(int i = 0; i < 2; i++)
						// {
						// 	conditionsObj.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
						// }
						// conditionsObj.GetComponent<MeshRenderer>().material.color = Color.blue;

						conditionsObj.SetActive(true);
						GameObject.FindObjectOfType<FSRConversion>().force = 0;

						conditionsObj.AddComponent<Rigidbody>();
						conditionsObj.GetComponent<Rigidbody>().mass = 100000f;
						conditionsObj.GetComponent<Rigidbody>().drag = 100000f;
						conditionsObj.GetComponent<Rigidbody>().angularDrag = 100000f;
						conditionsObj.gameObject.isStatic = true;

						pilotUp = (conditionPilot % 2 == 0);

						if(pilotUp)
						{
							stiffnessToTouch = 1;
						}
						else
						{
							stiffnessToTouch = 70;
						}
						currentMaxPilot = 70;
						currentMinPilot = 1;
						incrementPilot = 6;
						minimumSlider.value = currentMinPilot;
						maximumSlider.value = 71 - currentMaxPilot;

						staircaseList = new List<float>();
						twoConditions = 0;
						etatPilot = 1;
					break;

					case 1:
						controllerUI = this.GetComponent<ControllerUI>();
						fsrConvert = GameObject.FindObjectOfType<FSRConversion>();

						conditionsObj.transform.position = posOrigin;
						conditionsObj.transform.eulerAngles = rotOrigin;

						if(pilotUp)
						{
							GameObject.Find("SliderPilot").transform.GetChild(0).GetComponent<TextMeshPro>().text = "Find the minimum of the range of visual/physical \n deformation consistency using L/R.";
							minimumSlider.value = stiffnessToTouch;

						}
						else
						{
							GameObject.Find("SliderPilot").transform.GetChild(0).GetComponent<TextMeshPro>().text = "Find the maximum of the range of visual/physical \n deformation consistency using L/R.";
							maximumSlider.value = 71 - stiffnessToTouch;

						}
						GameObject.Find("SliderPilot").transform.GetChild(3).GetComponent<TextMeshPro>().text = "Current Max: " + currentMaxPilot.ToString() + "N/cm.";
						GameObject.Find("SliderPilot").transform.GetChild(4).GetComponent<TextMeshPro>().text = "Current Min: " + currentMinPilot.ToString() + "N/cm.";
						GameObject.Find("SliderPilot").transform.GetChild(5).GetComponent<TextMeshPro>().text = "Increment: " + incrementPilot.ToString() + "N/cm.";
						GameObject.Find("SliderPilot").transform.GetChild(6).GetComponent<TextMeshPro>().text = "Current Value: " + stiffnessToTouch.ToString() + "N/cm.";


						if(controllerUI.decrease)
						{

							if(stiffnessToTouch - incrementPilot > 1)
							{
								stiffnessToTouch = stiffnessToTouch - incrementPilot;
							}
							else
							{
								stiffnessToTouch = 1;
							}
							// if(stiffnessToTouch - incrementPilot > currentMinPilot)
							// {
							// 	stiffnessToTouch = stiffnessToTouch - incrementPilot;
							// }
							// else
							// {
							// 	stiffnessToTouch = currentMinPilot;
							// }
						}

						if(controllerUI.increase)
						{

							if(stiffnessToTouch + incrementPilot < 70)
							{
								stiffnessToTouch = stiffnessToTouch + incrementPilot;
							}
							else
							{
								stiffnessToTouch = 70;
							}							
							// if(stiffnessToTouch + incrementPilot < currentMaxPilot)
							// {
							// 	stiffnessToTouch = stiffnessToTouch + incrementPilot;
							// }
							// else
							// {
							// 	stiffnessToTouch = currentMaxPilot;
							// }
						}
						
						if(controllerUI.next)
						{
							twoConditions = twoConditions + 1;
							if((twoConditions % 2) == 0)
							{
								if(incrementPilot > 3)
								{
									incrementPilot = incrementPilot - 2;
									twoConditions = 0;
								}
								else
								{
									incrementPilot = incrementPilot - 1;
									twoConditions = 0;
								}
							}
							
							staircaseList.Add(stiffnessToTouch);
							if(pilotUp)
							{
								currentMinPilot = stiffnessToTouch;
								stiffnessToTouch = currentMaxPilot;
							}
							else
							{
								currentMaxPilot = stiffnessToTouch;
								stiffnessToTouch = currentMinPilot;
							}
							pilotUp = !pilotUp;
						}
						if(staircaseList.Count != 0)
						{
							Debug.Log("TwoCond: " + twoConditions);
						}
						
						if(controllerUI.reset)
						{
							if(staircaseList.Count > 2)
							{
								twoConditions = Mathf.Abs((twoConditions - 1) % 2);
								if((twoConditions % 2) != 0)
								{
									if(incrementPilot > 3)
									{
										incrementPilot = incrementPilot + 2;
									}
									else
									{
										incrementPilot = incrementPilot + 1;
									}
								}
								
								if(!pilotUp)
								{
									currentMinPilot = staircaseList[staircaseList.Count - 3];
									stiffnessToTouch = currentMinPilot;
								}
								else
								{
									currentMaxPilot = staircaseList[staircaseList.Count - 3];
									stiffnessToTouch = currentMaxPilot;
								}
								pilotUp = !pilotUp;
								staircaseList.RemoveAt(staircaseList.Count - 1);
							}
							else
							{
								if(!pilotUp)
								{
									currentMaxPilot = 70;
								}
								else
								{
									currentMinPilot = 1;
								}
								if(staircaseList.Count != 0)
								{
									twoConditions = (twoConditions - 1) % 2;
									if((twoConditions % 2) != 0)
									{
										if(incrementPilot > 3)
										{
											incrementPilot = incrementPilot + 2;
										}
										else
										{
											incrementPilot = incrementPilot + 1;
										}
									}
									
									staircaseList.RemoveAt(staircaseList.Count - 1);
									pilotUp = !pilotUp;
								}

							}
						}

						if(incrementPilot < 1f)
						{
							etatPilot = 2;
						}

						if(fsrConvert.deltaDist >= (scales[Mathf.FloorToInt(conditionPilot/2)].z - 0.005f))
						{
							fsrConvert.deltaDist = scales[Mathf.FloorToInt(conditionPilot/2)].z - 0.005f;
						}
						
						conditionsObj.GetComponent<SquashAndStretchDeformer>().Factor = -Mathf.Lerp(0, 1, fsrConvert.deltaDist);
						conditionsObj.gameObject.transform.localScale = new Vector3(scales[Mathf.FloorToInt(conditionPilot/2)].x, scales[Mathf.FloorToInt(conditionPilot/2)].y, scales[Mathf.FloorToInt(conditionPilot/2)].z - fsrConvert.deltaDist);


						int forceMax = 18;
						if(fsrConvert.deltaDist >= (forceMax / (stiffnessToTouch * Mathf.Pow(10, 2))) )
						{
							conditionsObj.GetComponent<MeshRenderer>().material.color = Color.green;
						}
						else
						{
							conditionsObj.GetComponent<MeshRenderer>().material.color = Color.blue;
						}

					break;

					case 2:

						if(GameObject.FindGameObjectWithTag("Target") != null)
						{
							Destroy(GameObject.FindGameObjectWithTag("Target"));
						}

						if(controllerUI.next)
						{
							etatPilot = -1;
						}

					break;


				}


			break;
			
			case -2: // Write results at the end of a block

				nbTouch = nbTouch + 1;
				if(configException.Count >= (tasks.Length*objectTypes.Length*scales.Length*allStiffness.Count))
				{
					nbBloc = nbBloc + 1;
					if(nbBloc >= nbBlocMax)
					{
						//Write results
						path = "Assets/Resources/DataCollection/UX/" + userName + "-" + time0 + ".csv";
						writer = new StreamWriter(path, true);
						writer.WriteLine("2-Forced Choice");
						for(int i = 0; i < forcedChoiceToWrite.Length; i++)
						{
							writer.WriteLine(forcedChoiceToWrite[i]);
						}
						writer.Close();
						path = "Assets/Resources/DataCollection/UX/" + userName + "-" + time0 + "-Confidence.csv";
						writer = new StreamWriter(path, true);
						writer.WriteLine("Results");
						for(int i = 0; i < resultsToWrite.Length; i++)
						{
							writer.WriteLine(resultsToWrite[i]);
						}
						writer.Close();
						path = "Assets/Resources/DataCollection/UX/" + userName + "-" + time0 + "-Configs.csv";
						writer = new StreamWriter(path, true);
						writer.WriteLine("Config");
						for(int i = 0; i < configException.Count; i++)
						{
							writer.WriteLine(configException[i]);
						}
						writer.Close();
						path = "Assets/Resources/DataCollection/UX/" + userName + "-" + time0 + "-MaxForces.csv";
						writer = new StreamWriter(path, true);
						writer.WriteLine("Maximum Force");
						for(int i = 0; i < maxForcesToWrite.Length; i++)
						{
							writer.WriteLine(maxForcesToWrite[i]);
						}
						writer.Close();
						state = 2;
					}
					else
					{
						startOver = true;
					}
				}
				else
				{
					Debug.Log("NBTOUCH +1");
					state = -1;
				}

				if(startOver)
				{
					path = "Assets/Resources/DataCollection/UX/" + userName + "-" + time0 + "-Configs.csv";
			        
					writer = new StreamWriter(path, true);
					for(int i = 0; i < configException.Count; i++)
					{
						writer.WriteLine(configException[i]);
					}
					writer.Close();

					configException.Clear();
					configException = new List<int>();

					Debug.Log("NBTOUCH -1");
					nbTouch = -1;
				}
			break;
		
			case -1: // Pick random configuration

				config = UnityEngine.Random.Range(0, tasks.Length*objectTypes.Length*scales.Length*allStiffness.Count);
				while(configException.Contains(config))
				{
					config = UnityEngine.Random.Range(0, tasks.Length*objectTypes.Length*scales.Length*allStiffness.Count);
				}

				for(int i = 0; i < objectTypes.Length; i++)
				{
					objectTypes[i].SetActive(false);
				}

				state = 0;

			break;

			case 0:
				configException.Add(config);				
				rotCylinder = new Vector3(0, 90f, 0);

				conditionsObj = (GameObject)Instantiate((objectTypes[objId]), GameObject.Find("ObjectsOfInterest").transform);
				conditionsObj.transform.localScale = scaleObj;
	    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPos.y, GameObject.Find("CalibrateMe").transform.position.z + offsetPos.z);

	    		
	    		conditionsObj2 = (GameObject)Instantiate((objectTypes[objId]), GameObject.Find("ObjectsOfInterest").transform);
				conditionsObj2.transform.localScale = scaleObj;
				// if(scaleObj == scales[0])
    // 			{
		  //   		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPos.y, GameObject.Find("CalibrateMe").transform.position.z + offsetPos.z);
    // 			}
    // 			else
    // 			{
		  //   		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPosS.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPosS.y, GameObject.Find("CalibrateMe").transform.position.z + offsetPosS.z);

    // 			}

				conditionsObj.transform.position = posOrigin;
				conditionsObj.transform.eulerAngles = rotOrigin;

				conditionsObj2.transform.position = posOrigin;
				conditionsObj2.transform.eulerAngles = rotOrigin;

								
				conditionsObj.tag = "Target";
				conditionsObj.AddComponent<Deformable>();
				conditionsObj.AddComponent<SquashAndStretchDeformer>();
				conditionsObj.GetComponent<Deformable>().AddDeformer(conditionsObj.GetComponent<SquashAndStretchDeformer>());// = GameObject.FindObjectOfType<Deformer>();
				


				conditionsObj.SetActive(true);

				conditionsObj2.tag = "Target";
				conditionsObj2.AddComponent<Deformable>();
				conditionsObj2.AddComponent<SquashAndStretchDeformer>();
				conditionsObj2.GetComponent<Deformable>().AddDeformer(conditionsObj.GetComponent<SquashAndStretchDeformer>());// = GameObject.FindObjectOfType<Deformer>();
				
				

				conditionsObj2.SetActive(true);



				GameObject.FindObjectOfType<FSRConversion>().force = 0;

				showPanel = true;
				slideOnMe.GetComponent<Slider>().value = 3;

				firstChoice = false;
				validatedChoice = false;
				confidenceLevel = false;

				if(taskToDo != "Compress")
				{
					leftOrRight = true;
				}  
				else
				{
					leftOrRight = false;
				}


				conditionsObj.AddComponent<Rigidbody>();
				conditionsObj.GetComponent<Rigidbody>().mass = 100000f;
				conditionsObj.GetComponent<Rigidbody>().drag = 100000f;
				conditionsObj.GetComponent<Rigidbody>().angularDrag = 100000f;
				conditionsObj.gameObject.isStatic = true;


				conditionsObj2.AddComponent<Rigidbody>();
				conditionsObj2.GetComponent<Rigidbody>().mass = 100000f;
				conditionsObj2.GetComponent<Rigidbody>().drag = 100000f;
				conditionsObj2.GetComponent<Rigidbody>().angularDrag = 100000f;
				conditionsObj2.gameObject.isStatic = true;


				state = 1;
			break;

			case 1:
				correctBug = 0;
				fsrConvert = GameObject.FindObjectOfType<FSRConversion>();

				if(fsrConvert.force > maxForces[config])
				{
					maxForces[config] = fsrConvert.force;
				}
				
				if(scaleObj == scales[0])
    			{
		    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPos.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPos.y, GameObject.Find("CalibrateMe").transform.position.z + offsetPos.z);
// Positions differentes pour obj1 et obj2; posOrigin est pos real object
    			}
    			else
    			{
		    		posOrigin = new Vector3(GameObject.Find("CalibrateMe").transform.position.x + offsetPosS.x, GameObject.Find("CalibrateMe").transform.position.y + offsetPosS.y, GameObject.Find("CalibrateMe").transform.position.z + offsetPosS.z);
    			}				
	    		
	    		
	    		if(boolStart)
	    		{
	    			StartCoroutine(Countdown());

	    		}
	    		

				conditionsObj.transform.position = posOrigin;
				conditionsObj.transform.eulerAngles = rotOrigin;
				conditionsObj.transform.localScale = scaleObj;

				// StartCoroutine(Consignes());

				if(Input.GetKeyDown(KeyCode.Space))
				{
					StartCoroutine(WaitForNext());
				}
				

				if(controllerUI.reset)
				{
					firstChoice = false;
					validatedChoice = false;
					leftOrRight = true;
					confidenceLevel = false;
				}

				if(fsrConvert.deltaDist >= (scaleObj.z - 0.005f))
				{
					fsrConvert.deltaDist = scaleObj.z - 0.005f;
				}

				// if(warpRight)
				// {
				// 	conditionsObj.GetComponent<SquashAndStretchDeformer>().Factor = -Mathf.Lerp(0, 1, fsrConvert.deltaDist);
				// 	conditionsObj.gameObject.transform.localScale = new Vector3(scaleObj.x, scaleObj.y, scaleObj.z - fsrConvert.deltaDist);
				// }
				// if(warpLeft)
				// {
				// 	conditionsObj2.GetComponent<SquashAndStretchDeformer>().Factor = -Mathf.Lerp(0, 1, fsrConvert.deltaDist);
				// 	conditionsObj2.gameObject.transform.localScale = new Vector3(scaleObj.x, scaleObj.y, scaleObj.z - fsrConvert.deltaDist);

				// }

				
				// conditionsObj.GetComponent<SquashAndStretchDeformer>().Factor = -Mathf.Lerp(0, 1, fsrConvert.deltaDist);
				// conditionsObj.gameObject.transform.localScale = new Vector3(scaleObj.x, scaleObj.y, scaleObj.z - fsrConvert.deltaDist);

				if(taskToDo == "Compress")
				{
					int forceMax = 13;
					// if(fsrConvert.deltaDist >= (forceMax / (stiffnessToTouch * Mathf.Pow(10, 2))) )
					if(fsrConvert.force >= forceMax)
					{
						leftOrRight = true;
						// if(warpRight)
						// {
						// 	conditionsObj.GetComponent<MeshRenderer>().material.color = Color.green;
						// }
						// else
						// {
						// 	conditionsObj.GetComponent<MeshRenderer>().material.color = Color.blue;
						// }
						// if(warpLeft)
						// {
						// 	conditionsObj2.GetComponent<MeshRenderer>().material.color = Color.green;
						// }
						// else
						// {
						// 	conditionsObj2.GetComponent<MeshRenderer>().material.color = Color.blue;
						// }
					}
					else
					{
						conditionsObj.GetComponent<MeshRenderer>().material.color = Color.blue;
						conditionsObj2.GetComponent<MeshRenderer>().material.color = Color.blue;
					}
				}

				if(leftOrRight)
				{
					if(controllerUI.increase || controllerUI.decrease)
					{
						firstChoice = true;
					}
					if(firstChoice)
					{
						confidenceLevel = false;
						if(controllerUI.decrease)
						{					
							forcedChoice[config] = 0;	
							GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
							GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialChoosing;	
						}
						if(controllerUI.increase)
						{			
							forcedChoice[config] = 1;			
							GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialChoosing;
							GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;	
						}

						
						if(controllerUI.next)
						{
							validatedChoice = true;
							leftOrRight = false;
						}
					}
					else
					{
						GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
						GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;	
					}
				}

				if(validatedChoice)
				{
					if(forcedChoice[config] == 0)
					{
						GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
						GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialValidation;
					}
					else
					{
						GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialValidation;
						GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;
					}
					confidenceLevel = true;
				}

				if(confidenceLevel)
				{
					StartCoroutine(WaitForTriggerOff());
				}

				if(Input.GetKeyDown(KeyCode.B))
				{
					state = -5;
				}

			break;

			case 2:
				StartCoroutine(EndOfTheGame());
			break;

			case 3:
				correctBug = 0;
				// secondBug = 0;

				Debug.Log("NbTouch: " + nbTouch + "; ConfigCount: " + configException.Count + "\nBloc: " + nbBloc + "/" + nbBlocMax);
				
				if(GameObject.FindGameObjectWithTag("Target") != null)
				{
					Destroy(GameObject.FindGameObjectWithTag("Target"));
				}

				// panels.SetActive(false);
				firstChoice = false;
				validatedChoice = false;
				leftOrRight = false;
				confidenceLevel = false;

				showPanel = false;
				GameObject.Find("ButtonRight").GetComponent<MeshRenderer>().material = materialNoChoice;
				GameObject.Find("ButtonLeft").GetComponent<MeshRenderer>().material = materialNoChoice;	

				for (int i = 1; i < otherwalls.Length; i++)
		    	{
			    	otherwalls[i].text = "Press Next Button to Get Started Again. ";
		    	}
		    	

				if(controllerUI.next)
				{
					for (int j = 1; j < otherwalls.Length; j++)
			    	{
				    	otherwalls[j].text = "";
			    	}
			    	StartCoroutine(CollisionWait());
					state = -2;
				}
				if(controllerUI.reset)
				{
					state = -5;
				}

				if(Input.GetKeyDown(KeyCode.Space))
				{
			    	StartCoroutine(CollisionWait());
					state = -2;
				}

				
				
			break;
		}
    }



    IEnumerator Consignes()
    {
    	if(taskToDo != "Compress")
    	{
    		for (int i = 1; i < otherwalls.Length; i++)
	    	{	
		    	otherwalls[i].text = taskToDo + " the " + objToCatch.gameObject.name + ".\n as much as you want. \n Were the physical and virtual deformations similar? \n Task #" + (configException.Count + tasks.Length*objectTypes.Length*scales.Length*nbBloc*allStiffness.Count) + "/" + (tasks.Length*objectTypes.Length*scales.Length*nbBlocMax*allStiffness.Count);
		    	//Were the physical and virtual deformations similar?

			}
    	}
    	else
    	{
    		for (int i = 1; i < otherwalls.Length; i++)
	    	{	
		    	otherwalls[i].text = taskToDo + " the " + objToCatch.gameObject.name + "\n until it becomes green. \n Were the physical and virtual deformations similar? \n Task #" + (configException.Count + tasks.Length*objectTypes.Length*scales.Length*nbBloc*allStiffness.Count) + "/" + (tasks.Length*objectTypes.Length*scales.Length*nbBlocMax*allStiffness.Count);
		    	//Were the physical and virtual deformations similar?
			}
    	}
		
    	panelQuestion.transform.GetChild(0).GetComponent<TextMeshPro>().text = (declaredTasks[taskNumber].questionMark).ToString();
		panelQuestion.transform.GetChild(2).GetComponent<TextMeshPro>().text = "1: Not Sure at all.";
		panelQuestion.transform.GetChild(1).GetComponent<TextMeshPro>().text = "5: Really Sure.";
		panelQuestion.transform.GetChild(3).GetComponent<TextMeshPro>().text = "My Confidence Level:";


		results[config] =  Mathf.FloorToInt(slideOnMe.GetComponent<Slider>().value);



		panelSliderTxt.GetComponent<TextMeshPro>().text = "Rate: " + results[config].ToString() + "/5.";

    	resultsToWrite[(nbBloc*tasks.Length*objectTypes.Length*scales.Length*allStiffness.Count + config)] = results[config]; 		
    	forcedChoiceToWrite[(nbBloc*tasks.Length*objectTypes.Length*scales.Length*allStiffness.Count + config)] = forcedChoice[config];
    	maxForcesToWrite[(nbBloc*tasks.Length*objectTypes.Length*scales.Length*allStiffness.Count + config)] = maxForces[config];
		
    	yield return new WaitUntil (() => state == 3);
    }

    IEnumerator ConsignesIntro()
    {
    	switch(etatIntro)
    	{
    		case 0:
    			for (int i = 1; i < otherwalls.Length; i++)
			    {	
					otherwalls[i].text = "Grasp the cube with your dominant hand.";
				}
    		break;

    		case 1:
    			for (int i = 1; i < otherwalls.Length; i++)
			    {	
			    	if(!quest)
			    	{
						otherwalls[i].text = "Use the Left and Right on the Trackpad to choose your answer. \n Validate using the Trigger (behind the controller).";
			    	}
			    	else
			    	{
						otherwalls[i].text = "Use the Left (red) and Right (green) on the Keypad to choose your answer. \n Validate using Next (blue).";
			    	}
				}
    		break;

    		case 2:
    			for (int i = 1; i < otherwalls.Length; i++)
			    {	
			    	if(!quest)
			    	{
						otherwalls[i].text = "Alright, now use the Grib (on the side of the controller) \n to reset your answer.";
			    	}
			    	else
			    	{
						otherwalls[i].text = "Alright, now use reset your answer using \n the Reset button (Purple).";		    		
			    	}
				}
    		break;

    		case 3:
    			for (int i = 1; i < otherwalls.Length; i++)
			    {	
			    	if(!quest)
			    	{
						otherwalls[i].text = "You got it! Choose and validate again, \n using the trackpad and trigger.";
			    	}
			    	else
			    	{
						otherwalls[i].text = "You got it! Choose and validate again, \n using L/R and Next.";
			    	}
				}
    		break;

    		case 4:
    			for (int i = 1; i < otherwalls.Length; i++)
			    {	
			    	if(!quest)
			    	{
						otherwalls[i].text = "Define your confidence level, \n using the trackpad and trigger.";
					}
					else
					{
						otherwalls[i].text = "Define your confidence level, \n using L/R and Next.";					
					}
				}
    		break;

    		case 5:
	    		for (int i = 1; i < otherwalls.Length; i++)
			    {	
					otherwalls[i].text = "You're all good to go! \n Press Next to get started.";
				}
    		break;
    	}
    	
    	yield return new WaitUntil (() => state == 3);
    }

    IEnumerator WaitInIntro()
    {
    	yield return new WaitUntil(() => (controllerUI.next == false));
		if(confidenceLevel && controllerUI.next && !leftOrRight && validatedChoice)
		{
			etatIntro = 5;
		}
    }


    IEnumerator CollisionWait()
    {
    	for (int i = 1; i < otherwalls.Length; i++)
    	{
	    	otherwalls[i].text = "";
    	}
        yield return new WaitUntil (() => state == 1);
    }    	

    IEnumerator WaitForNext()
    {
    	yield return new WaitForSeconds(0.5f); // 1sec for tech eval
    	correctBug = correctBug + 1; // This instead of having state = 3 in coroutine.
    	for(int k = 0; k < objectTypes[objId].transform.childCount; k++)
		{
	    	objectTypes[objId].transform.GetChild(k).gameObject.GetComponent<Renderer>().enabled = false;
		}

		panelQuestion.transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
		panelQuestion.transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
		panelQuestion.transform.GetChild(2).GetComponent<TextMeshPro>().text = "";
		panelQuestion.transform.GetChild(3).GetComponent<TextMeshPro>().text = "";


		for(int k = 2; k < panelQuestion.GetComponentsInChildren<MeshRenderer>().Length; k++)
    	{
			panelQuestion.GetComponentsInChildren<MeshRenderer>()[k].enabled = false;
    	}

		slideOnMe.SetActive(false);
		panelSliderTxt.GetComponent<MeshRenderer>().enabled = false;
    }

    IEnumerator WaitForTriggerOff()
    {
    	yield return new WaitUntil(() => (controllerUI.next == false));
		if(confidenceLevel && controllerUI.next && !leftOrRight && validatedChoice)
		{
			StartCoroutine(WaitForNext());
		}
    }

    IEnumerator EndOfTheGame()
    {
    	for(int i = 0; i < objectTypes.Length; i++)
    	{
	    	objectTypes[i].SetActive(false);
    	}
    	for (int i = 1; i < otherwalls.Length; i++)
    	{
	    	otherwalls[i].text = "The Game is Over. Thank you!";
    	}

    	
    	Debug.Log("Fin du Game!");
    	yield return new WaitForSeconds(2);
    	Debug.Break();

    }

    IEnumerator WaitABit()
    {
    	yield return new WaitForSeconds(0.2f);
		Destroy(calib);
		firstChoice = false;
		validatedChoice = false;
		leftOrRight = false;
		confidenceLevel = false;
		state = -1;
    }

    IEnumerator ConsignesPilot()
    {
    	switch(etatPilot)
    	{
    		case 1:
    			for (int i = 1; i < otherwalls.Length; i++)
		    	{	
		    		if(totalTrialPilot < 4)
		    		{
		    			if(pilotUp)
			    		{
					    	otherwalls[i].text = "Compress the cube until it gets green and answer the question. \n Validate using Next.\n \n Task #" + (totalTrialPilot+1).ToString() + "/" + (scales.Length*2).ToString();
			    		}
			    		else
			    		{
					    	// otherwalls[i].text = "Compress the cube until it gets green.  \n Reduce the stiffness using L until the visual/physical deformations are incoherent. \n Validate using Next.\n Current Stiffness:" + stiffnessToTouch.ToString() + " N/cm. Increment: " + incrementPilot.ToString() + "N/cm. \nMin: " + currentMinPilot.ToString() + " N/cm. Max: " + currentMaxPilot.ToString() + " N/cm. \n Task #" + (totalTrialPilot+1).ToString() + "/" + (scales.Length*2).ToString();
					    	otherwalls[i].text = "Compress the cube until it gets green and answer the question. \n Validate using Next.\n \n Task #" + (totalTrialPilot+1).ToString() + "/" + (scales.Length*2).ToString();

			    		}	
		    		}
		    		else
		    		{
		    			otherwalls[i].text = "Game Over! Thank you!";
		    		}
		    		
				}

    		break;

    		case 2:
    			for (int i = 1; i < otherwalls.Length; i++)
		    	{
			    	otherwalls[i].text = "Press Next Button to Get Started Again. ";
		    	}
    		break;
    		case -1:
    			if(totalTrialPilot != scales.Length*2)
    			{
    				for (int i = 1; i < otherwalls.Length; i++)
			    	{
				    	otherwalls[i].text = " ";
			    	}	
    			}
    		break;
    		default:
    			if(totalTrialPilot != scales.Length*2)
    			{
    				for (int i = 1; i < otherwalls.Length; i++)
			    	{
				    	otherwalls[i].text = " ";
			    	}	
    			}
				
    		break;

    	}

    	yield return new WaitUntil (() => (totalTrialPilot == scales.Length*2));
    }

    IEnumerator Countdown()
    {
    	float newStiffness = stiffnessVideo + 1;

    	if(!boolStart)
    	{
    		newStiffness = 0;
    		stiffnessVideo = 0;
    	}
    	yield return new WaitForSeconds(5f);
    	stiffnessVideo = newStiffness;
    }
}

