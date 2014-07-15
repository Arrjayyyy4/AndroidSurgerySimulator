using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SimulatorController : MonoBehaviour {
	//roger

	//add textures
	Texture resetTrocars;
	Texture checkAnswer;

	public GameObject patient;
	//public GameObject cameraOne;
	public bool positionChanged = false;
	//marker for 'x' for where trocar is placed when user is asked for trocar size
	public GameObject xmarker;
	public GameObject xmarkernew;
	public bool marked = false;
	// /roger

	//patient body transform
	public Transform patientBody;
	//

	//trocar model used to create trocars
	public GameObject trocar;
	//scalpel used
	public GameObject scalpel;

	public TextMesh questionText;
	public GUISkin simulatorSkin;
	public GUISkin alertSkin;

	//current count of trocars placed
	int count;
	//total count of trocars allowed
	int maxCount;
	//list of correct points for the current question
	List<Vector3> correctPoints;
	//list of chosen points for the current question
	List<Vector3> chosenPoints;
	//list of all available questions
	List<Question> availableCorrectPointSets;
	//list of all visible trocars (so they can be deleted)
	List<GameObject> visibleTrocars;

	Dictionary<string, float> trocarSizes;
	Dictionary<string, string> bodyPositions;
	bool askingPosandSize = true;
	bool answeredSize = false;
	Rect posAndSizeWindowRect = new Rect(Screen.width * 0.25F, Screen.height * 0.15F, Screen.width * 0.5F, Screen.height * 0.7F);
	string alertText = "";
	bool isShowingAlert = false;
	bool selectTrocarSize = false;
	GameObject lastPlacedTrocar;

	//timer used to delay answers after choosing trocar points
	float timer;

	//time to wait before allowed to make new trocar placement
	//prevents accidental placement
	float incisionDelay;

	//time of last incision
	float lastIncisionTime;
	
	//current question being asked
	Question currentQuestion;

	bool canPlaceTrocars;

	//determines question to be asked
	int questionIndex;

	//determines if questions should be randomized
	bool randomizeQuestions;

	//how many questions are left to ask
	int randomQuestionsLeft;
	
	//keeps track of body transparency
	bool bodyTransparent;

	int numCorrect;

	string fileName;

	System.DateTime currentTime;

	float questionStartTime;

	//Application State
	bool practiceMode = true; //if false, then in quiz taking mode
	bool answering = true;	//if false, then already checked answers and waiting to move to next question
	bool guiHidden = false; //hide/show state of gui

	//list of points for each surgery
	List<Vector3> appendectomyPoints = new List<Vector3>();
	List<Vector3> gallbladderPoints = new List<Vector3>();
	List<Vector3> cholecystectomyPoints = new List<Vector3>();
	List<Vector3> rightRenalPoints = new List<Vector3>();
	List<Vector3> leftNephrectomyPoints = new List<Vector3>();
	
	//points to use for reference
	Vector3 belowBellyButton = new Vector3(-55.48f, 5.45f, -16.61f);
	Vector3 bellyButton = new Vector3(-55.38f, 5.45f, -16.61f);
	Vector3 aboveAndRightBellyButton = new Vector3(-55.31f, 5.48f, -16.65f);
	Vector3 upperLeftStomach = new Vector3(-55.37f, 5.45f, -16.53f);
	Vector3 upperLeftStomachEdge = new Vector3(-55.36f, 5.45f, -16.45f);
	Vector3 bottomSternum = new Vector3(-55.145f, 5.50f, -16.62f);
	Vector3 bellowRightBellyButton = new Vector3(-55.47f, 5.44f, -16.70f);
	Vector3 rightBellyButtonEdge = new Vector3(-55.37f, 5.41f, -16.73f);
	Vector3 lowerLeftStomach = new Vector3(-55.42f, 5.45f, -16.50f);
	Vector3 aboveBellyButton = new Vector3(-55.3f, 5.47f, -16.61f);


	// Use this for initialization
	void Start () 
	{
		//make screen orient to the left
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		//add some textures
		resetTrocars = (Texture)Resources.Load("resets");
		checkAnswer = (Texture)Resources.Load("checks");
		//

		count = 0;
		timer = 0f;
		incisionDelay = 1.5f;
		lastIncisionTime = Time.time;
		canPlaceTrocars = false;
		randomizeQuestions = false;
		questionIndex = 0;
		bodyTransparent = false;
		numCorrect = 0;
		practiceMode = true;
		answering = true;

		currentTime = System.DateTime.Now;
		Debug.Log (currentTime.ToShortTimeString());
		fileName = currentTime.ToString("MMddHHmmss") + ".txt";


		correctPoints = new List<Vector3>();
		chosenPoints = new List<Vector3>();
		availableCorrectPointSets = new List<Question>();
		visibleTrocars = new List<GameObject>();

		/*roger - placed point declarations outside of functions so they can be modified
		//list of points for each surgery
		List<Vector3> appendectomyPoints = new List<Vector3>();
		List<Vector3> gallbladderPoints = new List<Vector3>();
		List<Vector3> cholecystectomyPoints = new List<Vector3>();
		List<Vector3> rightRenalPoints = new List<Vector3>();
		List<Vector3> leftNephrectomyPoints = new List<Vector3>();

		//points to use for reference
		Vector3 belowBellyButton = new Vector3(-55.48f, 5.45f, -16.61f);
		Vector3 bellyButton = new Vector3(-55.38f, 5.45f, -16.61f);
		Vector3 aboveAndRightBellyButton = new Vector3(-55.31f, 5.48f, -16.65f);
		Vector3 upperLeftStomach = new Vector3(-55.37f, 5.45f, -16.53f);
		Vector3 upperLeftStomachEdge = new Vector3(-55.36f, 5.45f, -16.45f);
		Vector3 bottomSternum = new Vector3(-55.145f, 5.50f, -16.62f);
		Vector3 bellowRightBellyButton = new Vector3(-55.47f, 5.44f, -16.70f);
		Vector3 rightBellyButtonEdge = new Vector3(-55.37f, 5.41f, -16.73f);
		Vector3 lowerLeftStomach = new Vector3(-55.42f, 5.45f, -16.50f);
		Vector3 aboveBellyButton = new Vector3(-55.3f, 5.47f, -16.61f);
		*/

		appendectomyPoints.Add(belowBellyButton);
		appendectomyPoints.Add(bellyButton);
		appendectomyPoints.Add (upperLeftStomach);

		gallbladderPoints.Add(bellyButton);
		gallbladderPoints.Add(upperLeftStomach);
		gallbladderPoints.Add(upperLeftStomachEdge);
		gallbladderPoints.Add (aboveAndRightBellyButton);

		cholecystectomyPoints.Add(bellyButton);
		cholecystectomyPoints.Add(upperLeftStomach);
		cholecystectomyPoints.Add(upperLeftStomachEdge);
		cholecystectomyPoints.Add(bottomSternum);

		rightRenalPoints.Add(bellyButton);
		rightRenalPoints.Add(aboveAndRightBellyButton);
		rightRenalPoints.Add(bellowRightBellyButton);
		rightRenalPoints.Add(rightBellyButtonEdge);

		leftNephrectomyPoints.Add(bellyButton);
		leftNephrectomyPoints.Add (belowBellyButton);
		leftNephrectomyPoints.Add (lowerLeftStomach);
		leftNephrectomyPoints.Add (aboveBellyButton);



		availableCorrectPointSets.Add(new Question(appendectomyPoints, "Appendectomy"));
		availableCorrectPointSets.Add(new Question(gallbladderPoints, "Gallbladder"));
		availableCorrectPointSets.Add(new Question(cholecystectomyPoints, "Cholecystectomy"));
		availableCorrectPointSets.Add(new Question(rightRenalPoints, "Right Renal"));
		availableCorrectPointSets.Add(new Question(leftNephrectomyPoints, "Left Nephrectomy"));


		//Initialize correct torcar sizes and body positions
		trocarSizes = new Dictionary<string, float>();
		trocarSizes.Add("Appendectomy", 0.010F);
		trocarSizes.Add("Gallbladder", 0.015F);
		trocarSizes.Add("Cholecystectomy", 0.010F);
		trocarSizes.Add("Right Renal", 0.010F);
		trocarSizes.Add("Left Nephrectomy", 0.010F);

		bodyPositions = new Dictionary<string, string>();
		bodyPositions.Add("Appendectomy", "flat");
		bodyPositions.Add("Gallbladder", "side");
		bodyPositions.Add("Cholecystectomy", "flat");
		bodyPositions.Add("Right Renal", "side");
		bodyPositions.Add("Left Nephrectomy", "side");


		randomQuestionsLeft = availableCorrectPointSets.Count;

		/*
		//add all available indices to list for random picking later
		for(int x = 0; x < availableCorrectPointSets.Count; x++)
		{
			randomQuestionIndices.Add (x);
		}*/

		changeQuestion();
		displayTrocars();

		//make skin opaque
		//this.renderer.materials[1].shader = Shader.Find("Diffuse");
	}

	void OnGUI()
	{		
		if(isShowingAlert)
		{
			GUI.depth = -1;
			GUI.Label(new Rect(Screen.width * 0.15F, Screen.height * 0.2F, Screen.width * 0.5F, Screen.height * 0.2F), alertText, alertSkin.label);
		}

		GUI.depth = 0;
		//GUI.skin = simulatorSkin;

		if(practiceMode)
		{
			//Show Next Surgery
			if(GUI.Button(new Rect(0, 0, Screen.width * 0.3F, Screen.height * 0.15F), "Show Next Surgery"))
			{
				Reset ();

				if(questionIndex > 0)
				{
					//increment this after selecting question
					questionIndex--;
				}
				else
				{
					questionIndex = availableCorrectPointSets.Count-1;
				}
				changeQuestion();
				displayTrocars();
			}

			//Show/Hide Trocars
			if(GUI.Button(new Rect(0, Screen.height * 0.15F + 5, Screen.width * 0.3F, Screen.height * 0.15F), "Show/Hide Trocars"))
			{
				if(!canPlaceTrocars)
				{
					if(!bodyTransparent)
					{
						displayTrocars();
					}
					else
					{
						Reset();
						changeQuestion(questionIndex);
					}
				}
				else
				{
					ResetQuestion();
					changeQuestion(questionIndex);
				}
			}

			if(GUI.Button(new Rect(0, 2 * (Screen.height * 0.15F + 5), Screen.width * 0.3F, Screen.height * 0.15F), "Begin Exam"))
			{
				canPlaceTrocars = !canPlaceTrocars;
				Reset();
				randomizeQuestions = true;
				changeQuestion();
				//roger
				Debug.Log (currentQuestion.text + " is the current question  Remaining questions: " + availableCorrectPointSets.Count);

				practiceMode = false;
			}
		}
		//If Started Exam
		else
		{
			//Show Instructions Here

			//Finished the game
			if(randomQuestionsLeft < 0)
			{
				//Show comprehensive results

				//Restart Quiz
				if(GUI.Button(new Rect(0, 0, Screen.width * 0.3F, Screen.height * 0.15F), "Start Over"))
				{
					//reset the application
					Start();
				}
			}
			else if(answering)
			{
				if(selectTrocarSize && !isShowingAlert)
				{
					//Debug.Log("DJDKJHKDHKJDH  " + selectTrocarSize);

					posAndSizeWindowRect = GUI.Window(0, posAndSizeWindowRect, trocarSizeWindow, currentQuestion.text);
				}

				if(askingPosandSize && !isShowingAlert)
				{
					//Ask Stuff about body position and trocar size in here    
    	 		  	posAndSizeWindowRect = GUI.Window(0, posAndSizeWindowRect, bodyPosWindow, currentQuestion.text);
    	 		  	//posAndSizeWindowRect = GUI.Window(0, posAndSizeWindowRect, trocarSizeWindow, currentQuestion.text);
				}
				else if(!isShowingAlert)
				{
					//Check answer
					if(GUI.Button(new Rect(0, 0, Screen.width * 0.3F, Screen.height * 0.15F), checkAnswer))
					{
						checkTrocars();
						answering = false;
						selectTrocarSize = false;
					}
					//roger - made sure that trocarSize has to be false for the user to be able to reset the question
					//this is primarily because user may (by accident or on purpose) have the trocars destroyed while the size windows is open
					//this causes a null reference error -- that's bad
					if(!selectTrocarSize)
					{
						if(GUI.Button(new Rect(0, Screen.height * 0.15F + 5, Screen.width * 0.3F, Screen.height * 0.15F), resetTrocars))
						{
							//reset trocars so the user can replace them before submitting their answer
							count = 0;
										
							foreach(GameObject trocar in visibleTrocars)
								Destroy(trocar);

							visibleTrocars.Clear();
							chosenPoints.Clear();
						}
					}
				}
			}
			//Checked answers
			else
			{
				//Next question
				if(GUI.Button(new Rect(0, Screen.height * 0.15F + 5, Screen.width * 0.3F, Screen.height * 0.15F), "Next Question"))
				{
					//canPlaceTrocars = !canPlaceTrocars;
					Reset();
					randomizeQuestions = true;
				
					//remove previous question so it's not asked again
					availableCorrectPointSets.Remove(currentQuestion);
					ChangeBodyPositon(); //roger - this should change body position back if the position is either left side or right side
					changeQuestion();
					answering = true;
					answeredSize = false;
					askingPosandSize = true;
					selectTrocarSize = false;
					Debug.Log (currentQuestion.text + " is the current question  Remaining questions: " + availableCorrectPointSets.Count);
				}
			}
		}
		/*
		if(GUI.Button(new Rect(Screen.width * 0.7F, 0, Screen.width * 0.25F, Screen.height * 0.15F), (guiHidden ? "Show " : "Hide ") + "Buttons"))
		{
			guiHidden = !guiHidden;
		}
		*/
	}
	
	void trocarSizeWindow(int windowIndex)
	{
		GUI.Label(new Rect(posAndSizeWindowRect.width * 0.1F, posAndSizeWindowRect.height * 0.07F, posAndSizeWindowRect.width * 0.9F, posAndSizeWindowRect.height * 0.2F), "What is the correct trocar size?");

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.17F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "5mm"))
		{
			Debug.Log("This works for " + CheckPosition(lastPlacedTrocar) + "!");

			//if(trocarSizes[currentQuestion.text] == 0.005)
			if(CheckPosition(lastPlacedTrocar) == "0.005")
			{
				Debug.Log("This works for " + CheckPosition(lastPlacedTrocar) + "!");
				//correct
				StartCoroutine(showAlert("Correct!"));
				selectTrocarSize = false;
				lastPlacedTrocar.renderer.enabled = true;

				//make trocar visible
				//lastPlacedTrocar.transform.scale = ???;
				foreach(Renderer rend in lastPlacedTrocar.GetComponentsInChildren<Renderer>())
					rend.enabled = true;
			}
			else
			{
				StartCoroutine(showAlert("Incorrect!"));
			}
		}

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.42F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "10mm"))
		{
			//if(trocarSizes[currentQuestion.text] == 0.010F)
			if(CheckPosition(lastPlacedTrocar) == "0.010")
			{
				Debug.Log("This works for " + CheckPosition(lastPlacedTrocar) + "!");
				//correct
				StartCoroutine(showAlert("Correct!"));
				selectTrocarSize = false;
				lastPlacedTrocar.renderer.enabled = true;

				//make trocar visible
				//lastPlacedTrocar.transform.scale = ???;
				foreach(Renderer rend in lastPlacedTrocar.GetComponentsInChildren<Renderer>())
					rend.enabled = true;
			}
			else
			{
				StartCoroutine(showAlert("Incorrect!"));
			}
		}

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.67F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "15mm"))
		{

			if(CheckPosition(lastPlacedTrocar) == "0.015")
			{
				Debug.Log("This works for " + CheckPosition(lastPlacedTrocar) + "!");
				StartCoroutine(showAlert("Correct!"));
				selectTrocarSize = false;
				lastPlacedTrocar.renderer.enabled = true;

				//make trocar visible
				//lastPlacedTrocar.transform.scale = ???;
				foreach(Renderer rend in lastPlacedTrocar.GetComponentsInChildren<Renderer>())
					rend.enabled = true;
			}
			else
			{
				StartCoroutine(showAlert("Incorrect!"));	
			}
		}
	}

	void bodyPosWindow(int windowIndex)
	{
		GUI.Label(new Rect(posAndSizeWindowRect.width * 0.1F, posAndSizeWindowRect.height * 0.07F, posAndSizeWindowRect.width * 0.9F, posAndSizeWindowRect.height * 0.2F), "What is the patient position?");

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.17F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "Flat"))
		{
			if(bodyPositions[currentQuestion.text] == "flat")
			{
				//correct
				StartCoroutine(showAlert("Correct!"));
				askingPosandSize = false;
			}
			else
			{
				StartCoroutine(showAlert("Incorrect!"));
			}
		}

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.42F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "Side"))
		{
			if(bodyPositions[currentQuestion.text] == "side")
			{
				//correct
				StartCoroutine(showAlert("Correct!"));
				ChangeBodyPositon();
				askingPosandSize = false;
			}
			else
			{
				StartCoroutine(showAlert("Incorrect!"));
			}
		}
		/* roger - commented out because only "flat" and "side" positions are needed (for now)
		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.67F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "Right Side"))
		{
			if(bodyPositions[currentQuestion.text] == "right side")
			{
				//correct
				StartCoroutine(showAlert("Correct!"));
				ChangeBodyPositon();
				askingPosandSize = false;
			}
			else
			{
				StartCoroutine(showAlert("Incorrect!"));	
			}
		}
		*/
	}

	IEnumerator showAlert(string text)
	{
		float showTime = 2.0F;
		float theTime = Time.time;

		alertText = text;
		isShowingAlert = true;

		while(Time.time - theTime <= showTime){yield return false;}

		isShowingAlert = false;

		yield return true;
	}

	// Update is called once per frame
	void Update () 
	{
		PlaceX();

		//If taking the exam
		if(!practiceMode && !askingPosandSize && !selectTrocarSize && GUIUtility.hotControl == 0)
		{
			//Works for touch and for mouse clicks for testing
			if(Input.touchCount == 1 || Input.GetMouseButtonDown(0))
        	{   
		        //On Touch
				//Switch to on touch when i get a testing device
				//if(Input.GetMouseButtonDown(0))
		        if(Input.GetMouseButtonDown(0) || Input.GetTouch(0).phase == TouchPhase.Began)
				{
					Ray ray;

					if(Input.touchCount == 1)
						ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
					else
						ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					
		       		RaycastHit hit;

		       		//If I hit an object
		        	if (Physics.Raycast(ray, out hit, 5))
		        	{
		        		//If I hit the body
						//if(hit.transform.tag == "PatientModel")
						if(hit.transform == patientBody)
						{
							//instantiate at point of scalpel down instead of on body

							Vector3 point = hit.point;//collision.contacts[0].point;
							Vector3 pointNormal = hit.normal;//collision.contacts[0].normal;
							//Debug.Log ("Normal: " + pointNormal);
							//if we haven't already made enough trocar points
							if(count < maxCount)
							{
								//if not an empty point, and the collision happened on the way down, not on the way up
								if(!point.Equals(Vector3.zero) && lastIncisionTime + incisionDelay < Time.time)// && pointNormal.y < -.1 && pointNormal.y > -1.5)
								{
									//Debug.Log ("Valid Normal"+ pointNormal);
									//point = scalpel.transform.position;
									//Debug.Log (point);

									//get trocar to stick out more
									Vector3 pointAdjusted = point;
									//pointAdjusted.y -= .005f;

									//instantiate trocar at point
									lastPlacedTrocar = (GameObject)Instantiate(trocar,pointAdjusted,Quaternion.Euler(270f,0f,0f));//Quaternion.identity);
									
									//Keep trocar invisible until trocar size is selected, then make it visible in trocarSizeWindow
									foreach(Renderer rend in lastPlacedTrocar.GetComponentsInChildren<Renderer>())
										rend.enabled = false;

									//Debug.Log (newTrocar.transform.position);
									selectTrocarSize = true;

									//add new trocar to list of points chosen
									chosenPoints.Add(lastPlacedTrocar.transform.position);
									count++;
									//keep track of trocars placed
									visibleTrocars.Add(lastPlacedTrocar);

									lastIncisionTime = Time.time;

									if(count >= maxCount)
									{
										// prevent it from resetting if another point is somehow chosen
										if(timer == 0)
										{
											//start the timer
											timer = Time.time;
										}	
									}
								}		
							}
						}
					}
	        	}
        	}
		}

		/*
		//after 5 seconds pass, show answers
		if(timer!= 0 && timer + 3f < Time.time)
		{
			
			checkTrocars();
			//make skin transparent
			this.renderer.materials[1].shader = Shader.Find("Transparent/VertexLit with Z");
			timer = 0f;
		}*/

		/*
		if(Input.GetKeyUp(KeyCode.Alpha2))
		{
			checkTrocars();
			//make skin transparent
			//this.renderer.materials[1].shader = Shader.Find("Transparent/VertexLit with Z");
		}

		if(Input.GetKeyUp(KeyCode.R))
		{
			Reset();
			changeQuestion();
		}

		if(Input.GetKeyUp(KeyCode.Alpha0))
		{
			canPlaceTrocars = !canPlaceTrocars;
			Reset();
			randomizeQuestions = true;
			changeQuestion();
			//roger
			Debug.Log (currentQuestion.text + " is the current question");
			//
		}

		if(Input.GetKeyUp(KeyCode.Alpha1))
		{
			if(!canPlaceTrocars)
			{
				if(!bodyTransparent)
				{
					displayTrocars();
				}
				else
				{
					Reset();
					changeQuestion(questionIndex);
				}
			}
			else
			{
				ResetQuestion();
				changeQuestion(questionIndex);
			}
		}

		if(Input.GetKeyUp(KeyCode.Alpha2) && !canPlaceTrocars)
		{
			Reset ();

			if(questionIndex > 0)
			{
				//increment this after selecting question
				questionIndex--;
			}
			else
			{
				questionIndex = availableCorrectPointSets.Count-1;
			}
			changeQuestion();
		}

		if(Input.GetKeyUp(KeyCode.Alpha3))
		{
			Debug.Log("3 pressed");
			if(!canPlaceTrocars)
			{
				Reset ();
		
				if(questionIndex < availableCorrectPointSets.Count-1)
				{
					//increment this after selecting question
					questionIndex++;
				}
				else
				{
					questionIndex = 0;
				}
				changeQuestion();
			}

			else
			{
				//roger
				Debug.Log (currentQuestion.text + " is the current question");
				//
				Reset();
				if(randomizeQuestions)
				{
					//remove previous question so it's not asked again
					availableCorrectPointSets.Remove(currentQuestion);
				}
				changeQuestion();
			}
		}

		if(Input.GetKeyUp(KeyCode.Q))
		{
			using (StreamWriter writer = File.AppendText(fileName))
			{
				writer.WriteLine("Paper");
			}
		}
		if(Input.GetKeyUp(KeyCode.W))
		{
			using (StreamWriter writer = File.AppendText(fileName))
			{
				writer.WriteLine("Oculus");
			}
		}
		*/
	}

	/*
	void OnCollisionEnter(Collision collision) 
	{ 
		
		if(collision.gameObject.tag == "TrocarTool" && canPlaceTrocars)
		{

			//instantiate at point of scalpel down instead of on body

			Vector3 point = collision.contacts[0].point;
			Vector3 pointNormal = collision.contacts[0].normal;
			//Debug.Log ("Normal: " + pointNormal);
			//if we haven't already made enough trocar points
			if(count < maxCount)
			{
				//if not an empty point, and the collision happened on the way down, not on the way up
				if(!point.Equals(Vector3.zero) && lastIncisionTime + incisionDelay < Time.time)// && pointNormal.y < -.1 && pointNormal.y > -1.5)
				{
					//Debug.Log ("Valid Normal"+ pointNormal);
					//point = scalpel.transform.position;
					//Debug.Log (point);

					//get trocar to stick out more
					Vector3 pointAdjusted = point;
					//pointAdjusted.y -= .005f;

					//instantiate trocar at point
					GameObject newTrocar = (GameObject)Instantiate(trocar,pointAdjusted,Quaternion.Euler(270f,0f,0f));//Quaternion.identity);
					//Debug.Log (newTrocar.transform.position);

					//add new trocar to list of points chosen
					chosenPoints.Add(newTrocar.transform.position);
					count++;
					//keep track of trocars placed
					visibleTrocars.Add(newTrocar);

					lastIncisionTime = Time.time;

					if(count >= maxCount)
					{
						// prevent it from resetting if another point is somehow chosen
						if(timer == 0)
						{
							//start the timer
							timer = Time.time;
						}	
					}
					
				}		
			}

		}
	}
	*/

	void displayTrocars()
	{

		foreach(Vector3 point in correctPoints)
		{
			GameObject newTrocar = (GameObject)Instantiate(trocar,point,Quaternion.Euler(270f,0f,0f));//Quaternion.identity);
			MeshRenderer[] trocarRenderers = newTrocar.GetComponentsInChildren<MeshRenderer>();
			
			visibleTrocars.Add(newTrocar);
			
		}

		//this.renderer.materials[1].shader = Shader.Find("Transparent/VertexLit with Z");
		bodyTransparent = true;
	}

	void checkTrocars()
	{
		List<Vector3> correctUserAnswers = new List<Vector3>();
		List<Vector3> incorrectUserAnswers = new List<Vector3>();

		
		//for each correctpoint, check if there is a chosen trocar close enough to it
		//if a chosen trocar is, add cpoint to correctlist, (turn green), else incorrectlist (turn red)
		foreach(Vector3 chosenPoint in chosenPoints)
		{
			foreach(Vector3 correctPoint in correctPoints)
			{
				//figure out horiztonal distance between points
				Vector3 distanceBetweenPoints = new Vector3(correctPoint.x - chosenPoint.x,
				                                            0,correctPoint.z-chosenPoint.z);
				float distanceMagnitude = distanceBetweenPoints.magnitude;

				//if chosen close enough
				if(distanceMagnitude < .07f)
				{
					Debug.Log (chosenPoint.ToString() +  correctPoint.ToString() + distanceMagnitude.ToString());
					//add them to list of correct points
					correctUserAnswers.Add(correctPoint);

					//don't allow point to be double counted
					correctPoints.Remove(correctPoint);

					break;
				}
			}
		}
		//if didn't find a match for a point, it was left in correctPoints list
		foreach(Vector3 point in correctPoints)
		{
			incorrectUserAnswers.Add (point);
		}

		//go through correctanswers, instantiate trocar and make texture green
		//add these trocars to list so they can be deleted later
		foreach(Vector3 point in correctUserAnswers)
		{
			GameObject newTrocar = (GameObject)Instantiate(trocar,point,Quaternion.Euler(270f,0f,0f));//Quaternion.identity);
			MeshRenderer[] trocarRenderers = newTrocar.GetComponentsInChildren<MeshRenderer>();


			foreach(MeshRenderer renderer in trocarRenderers)
			{
				renderer.renderer.material.color = Color.green;
			}

			visibleTrocars.Add(newTrocar);
			numCorrect++;

		}

		float timeToAnswer = Time.time -questionStartTime;
		
		//open text file
		//save question text, num correct, time to answer
		if(canPlaceTrocars)
		{
			using (StreamWriter writer = File.AppendText(fileName))
			{
				writer.WriteLine(System.DateTime.Now.ToLongTimeString());
				writer.WriteLine(timeToAnswer.ToString());
				Debug.Log (timeToAnswer.ToString());
				writer.WriteLine(currentQuestion.text + ": " + numCorrect.ToString());
				Debug.Log (currentQuestion.text + ": " + numCorrect.ToString());
				writer.WriteLine(" ");
			}
		}

		//go through incorrectanswers, instantiate trocar and make texture red
		//add these trocars to list so they can be deleted later when done displaying answer
		foreach(Vector3 point in incorrectUserAnswers)
		{
			GameObject newTrocar = (GameObject)Instantiate(trocar,point,Quaternion.Euler(270f,0f,0f));//Quaternion.identity);
			MeshRenderer[] trocarRenderers = newTrocar.GetComponentsInChildren<MeshRenderer>();
			
			foreach(MeshRenderer renderer in trocarRenderers)
			{
				renderer.renderer.material.color = Color.red;
			}

			visibleTrocars.Add(newTrocar);
		}



	}

	//change the question
	void changeQuestion(int index = -1)
	{
		//if index has been manually overriden, use that number
		if(index!=-1 && randomQuestionsLeft >= 0)
		{
			currentQuestion = availableCorrectPointSets[index];

			//set correct points to the points for this question
			foreach(Vector3 point in currentQuestion.points)
			{
				correctPoints.Add(point);
			}
			//change display text
			questionText.text = currentQuestion.text;

			//only allow as many trocars as they are correct trocars
			maxCount = correctPoints.Count;
		}
		else
		{
			//if randomizing questions, pick random number
			if(randomizeQuestions)// && canPlaceTrocars)
			{
				//picks a random question from list
				questionIndex = Random.Range(0,randomQuestionsLeft);
				//one less question available to ask
				randomQuestionsLeft--;
			}

			//Debug.Log (questionIndex);
			if(randomQuestionsLeft < 0)
			{
				questionText.text = "Quiz Complete!";
			}
			else
			{
				currentQuestion = availableCorrectPointSets[questionIndex];

				//set correct points to the points for this question
				foreach(Vector3 point in currentQuestion.points)
				{
					correctPoints.Add(point);
				}
				//change display text
				questionText.text = currentQuestion.text;
				
				//only allow as many trocars as they are correct trocars
				maxCount = correctPoints.Count;

				questionStartTime = Time.time;
			}
		}
	}

	void Reset()
	{
		foreach(GameObject trocar in visibleTrocars)
		{
			//delete each visible trocar
			Destroy (trocar);
		}

		//set count to 0 so you can do next scenario
		count = 0;

		//make skin opaque
		//this.renderer.materials[1].shader = Shader.Find("Diffuse");
		bodyTransparent = false;

		//remove the chosen points
		chosenPoints.Clear();

		//remove any points that weren't selected
		correctPoints.Clear();

		timer = 0f;

		numCorrect = 0;

		answeredSize = false;
		askingPosandSize = true;
	}

	//same as reset but dosn't remove points
	void ResetQuestion()
	{
		foreach(GameObject trocar in visibleTrocars)
		{
			//delete each visible trocar
			Destroy (trocar);
		}
		
		//set count to 0 so you can do next scenario
		count = 0;
		
		//make skin opaque
		//this.renderer.materials[1].shader = Shader.Find("Diffuse");

		bodyTransparent = false;

		//remove the chosen points
		chosenPoints.Clear();

		numCorrect = 0;
	}

	//roger

	//changes body position of the patient to lay on the side and does the same for the camera
	//so that the patient is always in bird's eye view relative to the patient
	void ChangeBodyPositon()
	{
		//check if patient should be laying on their side
		if(bodyPositions[currentQuestion.text] == "side")
		{
			//check if the position has already been changed
			if(positionChanged)
			{
				//turn camera and patient 90 degrees
				patient.transform.Rotate(270,0,0);
				//cameraOne.transform.Rotate(90,0,0);
				positionChanged = false; //indicates that position has been changed
			}
			else
			{
				//change position back
				patient.transform.Rotate(-270,0,0);
				//cameraOne.transform.Rotate(-90,0,0);
				positionChanged = true; //indicates that position has been changed back
			}
		}
	}
	/*
	while the .01m is the default for the trocar size, a range can be set up for different trocar sizes
	for instance if (trocarsizes ... ) || ( (z > 4 && z < 6) && (x > 10 && x < 12) )
	the trocar sizes would only be correct for that range of x and z values
	but if the person's head or any unusual part of the body (ie.legs,hands) the size would be default size
	this means that the user is doing something wrong! *oh no!*
	*/
	string CheckPosition(GameObject placedTrocar)
	{
		//print statement for debugging so that if positions are changed,
		//everything can be monitored here
		Debug.Log("z = " + placedTrocar.transform.position.z + " x = " + placedTrocar.transform.position.x + " !");

		//the if else cases are used to form a range for the trocar positions and returns a string
		if( (placedTrocar.transform.position.z > 30 && placedTrocar.transform.position.z < 40)
		   && (placedTrocar.transform.position.x > 30 && placedTrocar.transform.position.x < 40) )
		{
			return "0.015";
		}
		else if( (placedTrocar.transform.position.z > 30 && placedTrocar.transform.position.z < 40)
		        && (placedTrocar.transform.position.x > 30 && placedTrocar.transform.position.x < 40) )
		{
			return "0.010";
		}
		else if( (placedTrocar.transform.position.z > -20 && placedTrocar.transform.position.z < 10)
		   && (placedTrocar.transform.position.x < -10 && placedTrocar.transform.position.x > -60) )
		{
			return "0.005";
		}
		//this is a default case just in 'case'
		else 
			return "0.010";
	}

	void PlaceX()
	{
		if( (!(GameObject.Find("TrocarSpot(Clone)"))) && selectTrocarSize)
		{
			//place an 'x' onto the position where the last trocar is placed
			xmarkernew = (GameObject)Instantiate(xmarker,lastPlacedTrocar.transform.position,Quaternion.identity);
		}
		else if(!selectTrocarSize)
		{
			//destroy any and all markers on patient
			Destroy(GameObject.Find("TrocarSpot(Clone)"));
		}
	}

	// /roger
}
