using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class SimulatorController : MonoBehaviour {
	//TODO bellybutton accuracy
	//TODO turn table
	//roger
	public Dictionary<Vector3, float> tmpSizes = new Dictionary<Vector3, float>();
	public float tmpSize = 0.0f;
	public Vector3 tmpPos = new Vector3(0f,0f,0f);
	float fiveScale = .5f;
	float tenScale = 1.0f;
	float twelveScale = 1.2f;
	public static bool transparent = false;
	public float turn = 0.0F;
	public bool sized = true;
	//gameobject for the empty gameobject patenting the camera
	//this object's coordinate system is similar to that of the patient
	//so that the camera can rotate around the axis of the patient
	//and this functionality can be edited in any way
	public GameObject tilt;

	public TextAsset surgeryList;
	//this checks whether or not user has made a mistake in selecting trocar points
	public int trocarsWrong = 0;
	
	//to fix GUI positions
	public int GUImodifier = 0;
	//add texture

	Texture resetTrocars;
	Texture checkAnswer;
	//public GameObject skin; rogerrr
	public GameObject patient;
	//public GameObject cameraOne;
	public bool positionChanged = false;
	//marker for 'x' for where trocar is placed when user is asked for trocar size
	public GameObject xmarker;
	public GameObject xmarkernew;
	public bool marked = false;
	// /roger
	public bool moved = false;
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
	//list of chosen sizes for the current question
	List<float> chosenSizes;
	//list of all available questions
	List<Surgery> availableCorrectPointSets;
	//list of all visible trocars (so they can be deleted)
	List<GameObject> visibleTrocars;
	
	//Dictionary<string, float> trocarSizes;

	Dictionary<string, string> bodyPositions;

	//Dictionary<string, Dictionary<Vector3, float>> surgerySizes; // for trocar sizes



	bool askingPosandSize = true;
	bool answeredSize = false;
	Rect posAndSizeWindowRect = new Rect(Screen.height * 0.25F,  Screen.width * 0.15F, Screen.height * 0.5F, Screen.width * 0.7F);
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
	Surgery currentQuestion;

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


	//points to use for reference

	Vector3 bellyButton = new Vector3(-55.38f, 5.45f, -16.61f);



	// Use this for initialization
	void Start () 
	{

		//make screen orient to the left
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		//add some textures


		//Color roger = skin.renderer.material.color;
		//roger.a = .25F;
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
		//Debug.Log (currentTime.ToShortTimeString());
		fileName = currentTime.ToString("MMddHHmmss") + ".txt";


		correctPoints = new List<Vector3>();
		chosenPoints = new List<Vector3>();
		chosenSizes = new List<float>();
		availableCorrectPointSets = new List<Surgery>();
		visibleTrocars = new List<GameObject>();

		//Initialize correct torcar sizes and body positions
		//trocar sizes one is unused


		ReadFile ();

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
		// - roger GUI font size will be changed to become more legible on Android
		GUI.skin.box.fontSize = GUI.skin.button.fontSize = 40;
		GUI.skin.label.fontSize = 40;
		// /roger
		int rotateGUImag = 0;
		if(practiceMode) { rotateGUImag = 6; }
		else { rotateGUImag = 4; }
		//debugg roger
		//GUI.Box (new Rect (Screen.width * .5f, 0, 300, 100), "R*" + tilt.transform.rotation.eulerAngles.x);
		//roger

		if(isShowingAlert)
		{
			GUI.depth = -1;
			GUI.Label(new Rect(Screen.width * 0.15F, Screen.height * 0.2F, Screen.width * 0.5F, Screen.height * 0.2F), alertText, alertSkin.label);
		}

		GUI.depth = 0;
		//GUI.skin = simulatorSkin;

		if(practiceMode)
		{
			if(GUI.RepeatButton(new Rect(Screen.width * 0.50f+GUImodifier, rotateGUImag * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), "Rotate Up"))
			{
				tilt.transform.Rotate ( 0.7f, 0, 0);
			}
			if(GUI.RepeatButton(new Rect(Screen.width * 0.50f+GUImodifier, (rotateGUImag+1) * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), "Rotate Down"))
			{
				tilt.transform.Rotate ( -0.7f, 0, 0);
				
			}

			GUI.Box(new Rect(Screen.width * 0.50f+GUImodifier, 5 * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), currentQuestion.points.Count + " trocars expected"); 

			GUI.Box(new Rect(Screen.width * 0.50f+GUImodifier, 4 * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), "Position: " + bodyPositions[currentQuestion.text] + ""); 

			//Show Next Surgery
			if(GUI.Button(new Rect(Screen.width * 0.50f+GUImodifier, 0, Screen.width * 0.3F, Screen.height * 0.10F), "Show Next Surgery"))
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
			if(GUI.Button(new Rect(Screen.width * 0.50f+GUImodifier, Screen.height * 0.10F + 5, Screen.width * 0.3F, Screen.height * 0.10F), "Show/Hide Trocars"))
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
			//toggle transparency
			if(GUI.Button(new Rect(Screen.width * 0.50f+GUImodifier, 3 * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), "Transparent Chest"))
			{
				transparent = !transparent;
				
			}

			if(GUI.Button(new Rect(Screen.width * 0.50f+GUImodifier, 2 * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), "Begin Exam"))
			{
				canPlaceTrocars = !canPlaceTrocars;
				Reset();
				randomizeQuestions = true;
				changeQuestion();
				//roger
				//Debug.Log (currentQuestion.text + " is the current question  Remaining questions: " + availableCorrectPointSets.Count);

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
				if(GUI.Button(new Rect(Screen.height * 0.50f + GUImodifier, 0, Screen.width * 0.3F, Screen.height * 0.10F), "Start Over"))
				{
					//reset the application
					ClearAll ();
					Start();
				}
			}
			else if(answering)
			{
				if(selectTrocarSize && !isShowingAlert)
				{
					//Debug.Log("DJDKJHKDHKJDH  " + selectTrocarSize);

					posAndSizeWindowRect = GUI.Window(0, posAndSizeWindowRect, trocarSizeWindow, " " /* currentQuestion.text */ );
				}

				if(askingPosandSize && !isShowingAlert)
				{
					//Ask Stuff about body position and trocar size in here    
    	 		  	posAndSizeWindowRect = GUI.Window(0, posAndSizeWindowRect, bodyPosWindow, " " /* currentQuestion.text */ );
    	 		  	//posAndSizeWindowRect = GUI.Window(0, posAndSizeWindowRect, trocarSizeWindow, currentQuestion.text);
				}
				else if(!isShowingAlert)
				{
					//toggle transparency
					if(GUI.Button(new Rect(Screen.width * 0.50f+GUImodifier, 2 * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), "Transparent Chest"))
					{
						transparent = !transparent;

					}

					if(GUI.RepeatButton(new Rect(Screen.width * 0.50f+GUImodifier, rotateGUImag * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), "Rotate Up"))
					{
						tilt.transform.Rotate ( 0.7f, 0, 0);
					}
					if(GUI.RepeatButton(new Rect(Screen.width * 0.50f+GUImodifier, (rotateGUImag+1) * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), "Rotate Down"))
					{
						tilt.transform.Rotate ( -0.7f, 0, 0);
						
					}

					GUI.Box(new Rect(Screen.width * 0.50f+GUImodifier, 3 * (Screen.height * 0.10F + 5), Screen.width * 0.3F, Screen.height * 0.10F), currentQuestion.points.Count + " trocars expected"); 


					//Check answer
					if(GUI.Button(new Rect(Screen.width * 0.50f+GUImodifier, 0, Screen.width * 0.3F, Screen.height * 0.10F), "Check answer"))
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
						if(GUI.RepeatButton(new Rect(Screen.width * 0.50f+GUImodifier, Screen.height * 0.10F + 5 + 0, Screen.width * 0.3F, Screen.height * 0.10F), "Reset trocars"))
						{
							//reset trocars so the user can replace them before submitting their answer
							count = 0;
										
							foreach(GameObject trocar in visibleTrocars)
								Destroy(trocar);

							visibleTrocars.Clear();
							chosenPoints.Clear();
							chosenSizes.Clear();
						}
					}
				}
			}
			//Checked answers
			else
			{
				//Next question
				if(GUI.Button(new Rect(Screen.width * 0.50f+GUImodifier,  Screen.height * 0.10F + 5  , Screen.width * 0.3F, Screen.height * 0.10F), "Return to Menu"))
				{
					Reset();
					if(positionChanged)
					{
						//turn camera and patient 90 degrees
						patient.transform.Rotate(315,0,0);
						//cameraOne.transform.Rotate(90,0,0);
						positionChanged = false; //indicates that position has been changed
					}
					ClearAll ();

					Start();

				}

				if(GUI.Button(new Rect(Screen.width * 0.50f+GUImodifier, 0 , Screen.width * 0.3F, Screen.height * 0.10F), "Next Question"))
				{
					//canPlaceTrocars = !canPlaceTrocars;
					Reset();
					randomizeQuestions = true;
				
					//remove previous question so it's not asked again
					availableCorrectPointSets.Remove(currentQuestion);
					ChangeBodyPositon(); //roger - this should change body position back if the position is either left Lateral or right Lateral
					FixPos (); //roger
					changeQuestion();
					answering = true;
					answeredSize = false;
					askingPosandSize = true;
					selectTrocarSize = false;
					//Debug.Log (currentQuestion.text + " is the current question  Remaining questions: " + availableCorrectPointSets.Count);
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
			//Debug.Log("This works for " + CheckPosition(lastPlacedTrocar) + "!");

				selectTrocarSize = false;
				lastPlacedTrocar.renderer.enabled = true;
				lastPlacedTrocar.transform.localScale = new Vector3(.5f, .5f,.5f);
				foreach(Renderer rend in lastPlacedTrocar.GetComponentsInChildren<Renderer>())
					rend.enabled = true;
			chosenSizes.Add(lastPlacedTrocar.transform.localScale.x * 10);
			tmpSize = lastPlacedTrocar.transform.localScale.x * 10;
			tmpSizes.Add(tmpPos,tmpSize);
			Debug.Log(tmpPos + " " + tmpSize);
			//Debug.Log("chosensizes = " + chosenSizes.Count + " ->" + chosenSizes[chosenSizes.Count-1] + " " + chosenPoints[chosenPoints.Count-1]);
			//Debug.Log(lastPlacedTrocar.transform.localScale.x);

		}

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.42F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "10mm"))
		{

				selectTrocarSize = false;
				lastPlacedTrocar.renderer.enabled = true;
				lastPlacedTrocar.transform.localScale = new Vector3(1f, 1f,1f);
				foreach(Renderer rend in lastPlacedTrocar.GetComponentsInChildren<Renderer>())
					rend.enabled = true;
			chosenSizes.Add(lastPlacedTrocar.transform.localScale.x * 10);
			tmpSize = lastPlacedTrocar.transform.localScale.x * 10;
			tmpSizes.Add(tmpPos,tmpSize);
			Debug.Log(tmpPos + " " + tmpSize);
			//Debug.Log("chosensizes = " + chosenSizes.Count + " ->" + chosenSizes[chosenSizes.Count-1] + " " + chosenPoints[chosenPoints.Count-1]);
			//Debug.Log(lastPlacedTrocar.transform.localScale.x);
		}

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.67F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "12mm"))
		{
				selectTrocarSize = false;
				lastPlacedTrocar.renderer.enabled = true;
				lastPlacedTrocar.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

				foreach(Renderer rend in lastPlacedTrocar.GetComponentsInChildren<Renderer>())
					rend.enabled = true;
			//Debug.Log(lastPlacedTrocar.transform.localScale.x * 10); //new
			chosenSizes.Add(lastPlacedTrocar.transform.localScale.x * 10);
			tmpSize = lastPlacedTrocar.transform.localScale.x * 10;
			tmpSizes.Add(tmpPos,tmpSize);
			Debug.Log(tmpPos + " " + tmpSize);
			//Debug.Log("chosensizes = " + chosenSizes.Count + " ->" + chosenSizes[chosenSizes.Count-1] + " " + chosenPoints[chosenPoints.Count-1]);
			//Debug.Log(lastPlacedTrocar.transform.localScale.x);
		}
	}

	void bodyPosWindow(int windowIndex)
	{
		GUI.Label(new Rect(posAndSizeWindowRect.width * 0.1F, posAndSizeWindowRect.height * 0.07F, posAndSizeWindowRect.width * 0.9F, posAndSizeWindowRect.height * 0.2F), "What is the patient position?");

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.17F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "Supine"))
		{
			//Debug.Log(bodyPositions[currentQuestion.text]);

			if(bodyPositions[currentQuestion.text] == "Supine")
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

		if(GUI.Button(new Rect(posAndSizeWindowRect.width * 0.25F, posAndSizeWindowRect.height * 0.42F, posAndSizeWindowRect.width * 0.5F, posAndSizeWindowRect.height * 0.2F), "Lateral"))
		{
			if(bodyPositions[currentQuestion.text] == "Lateral")
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

	}

	IEnumerator showAlert(string text)
	{
		float showTime = 0.5F;
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

		//Debug.Log("transparent = " + transparent);

		//resize trocars based on position
		FindObject ();

		//camera now tilts around patient
		MoveAround ();

		//whenever a trocar point is decided by the user,
		//an 'x' is marked in that spot before any trocar is actually placed
		PlaceX();

		//If taking the exam
		if(!practiceMode && !askingPosandSize && !selectTrocarSize  && GUIUtility.hotControl == 0 )
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
						if(hit.transform == patientBody || hit.transform.root.tag == "PatientModel")
						{
							//Debug.Log("hit something!");
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
									tmpPos = lastPlacedTrocar.transform.position;
									Debug.Log(tmpPos);
									//Debug.Log("chosenpoints = " + chosenPoints.Count);
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


	}


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
		Surgery tmpPoints = new Surgery(chosenPoints, tmpSizes, "temp");
		List<Vector3> correctUserAnswers = new List<Vector3>();
		List<Vector3> incorrectUserAnswers = new List<Vector3>();

		//for each correctpoint, check if there is a chosen trocar close enough to it
		//if a chosen trocar is, add cpoint to correctlist, (turn green), else incorrectlist (turn red)
		int j=0;
		foreach(Vector3 chosenPoint in chosenPoints)
		{

			foreach(Vector3 correctPoint in correctPoints)
			{
				//if chosen close enough
				if(distanceMagnitude(correctPoint,chosenPoints[j]) < .07f)
				{
					//Debug.Log(" " + chosenPoint+"->" + currentQuestion.surgerySizes[correctPoint]+"->" + tmpPoints.surgerySizes[chosenPoint]);
					//fixed 
					if(currentQuestion.surgerySizes[correctPoint] == tmpPoints.surgerySizes[chosenPoint])
					{
						//Debug.Log("aweshum!");
						correctUserAnswers.Add(correctPoint); 
						correctPoints.Remove(correctPoint);
					}

					/*
					if(currentQuestion.surgerySizes[correctPoint] == tmpPoints.surgerySizes[chosenPoint])
					{
						Debug.Log("aweshum!");
						correctUserAnswers.Add(correctPoint); 
						correctPoints.Remove(correctPoint);
					}
					*/

					//Debug.Log("^^");

					//don't allow point to be double counted

					break;
				}


			}
			j+=1;
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
			FindObject (newTrocar);

			foreach(MeshRenderer renderer in trocarRenderers)
			{
				renderer.renderer.material.color = Color.green;
			}
			/*
			Vector3 buttonRange = (point - bellyButton);
			if(buttonRange.magnitude > 0.1f && buttonRange.magnitude < 0.55f)
			{
				renderer.renderer.material.color = Color.red;
				Debug.Log("this is red now!");
			}
			*/
			visibleTrocars.Add(newTrocar);
			numCorrect++;

		}


		float timeToAnswer = Time.time -questionStartTime;

		//go through incorrectanswers, instantiate trocar and make texture red
		//add these trocars to list so they can be deleted later when done displaying answer
		foreach(Vector3 point in incorrectUserAnswers)
		{
			GameObject newTrocar = (GameObject)Instantiate(trocar,point,Quaternion.Euler(270f,0f,0f));//Quaternion.identity);
			MeshRenderer[] trocarRenderers = newTrocar.GetComponentsInChildren<MeshRenderer>();
			FindObject(newTrocar); //new roger
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

	//changes body position of the patient to be Lateral and does the same for the camera
	//so that the patient is always in bird's eye view relative to the patient
	void ChangeBodyPositon()
	{
		//check if patient should be Lateral
		if(bodyPositions[currentQuestion.text] == "Lateral")
		{
			//check if the position has already been changed
			if(positionChanged)
			{
				//turn camera and patient 45 degrees
				patient.transform.Rotate(315,0,0);
				//cameraOne.transform.Rotate(90,0,0);
				positionChanged = false; //indicates that position has been changed
			}
			else
			{
				//change position back
				patient.transform.Rotate(-315,0,0);
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
//		Debug.Log("z = " + placedTrocar.transform.position.z + " x = " + placedTrocar.transform.position.x + " !");

		//the if else cases are used to form a range for the trocar positions and returns a string
		if( (placedTrocar.transform.position.z > 30 && placedTrocar.transform.position.z < 40)
		   && (placedTrocar.transform.position.x > 30 && placedTrocar.transform.position.x < 40) )
		{
			return "12";
		}
		else if( (placedTrocar.transform.position.z > 30 && placedTrocar.transform.position.z < 40)
		        && (placedTrocar.transform.position.x > 30 && placedTrocar.transform.position.x < 40) )
		{
			return "10";
		}
		else if( (placedTrocar.transform.position.z > -20 && placedTrocar.transform.position.z < 10)
		   && (placedTrocar.transform.position.x < -10 && placedTrocar.transform.position.x > -60) )
		{
			return "5";
		}
		//this is a default case just in 'case'
		else 
			return "10";
	}

	//this function will mark the spot that the user intends to place a trocar onto the patient
	//this will only happened when they are asked about the size of the specific trocar
	//the spot will be gone whenever the user has correctly answered where the trocar
	//should be placed
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

	//this function will enable the user to turn the camera around the patient
	//so that they may place points on areas not completely visible by camera in its initial state
	void MoveAround()
	{

		//tilt.transform.Rotate (Input.acceleration.y * 0.5F + 0.2F, 0, 0);
		//tilt.transform.rotation.x = Mathf.Clamp (tilt.transform.eulerAngles.x, 20, 89);

		//check if position has been changed
		/* the idea behind this is to ensure that the camera's is properly oriented 
		 * so that the user is not able to move the camera around the entire table
		 * and they are able to see the top and one Lateral of the patient at all times
		 */

		//check is position is changed
		if(!positionChanged)
		{
			//but first check to make sure that the camera has been moved or not
			if(moved)
			{
				//if the camera has been moved, revert the position back
				tilt.transform.Rotate(-30,0,0);
				//set the moved flag to false
				moved = false;
			}
			if(tilt.transform.rotation.eulerAngles.x == 90)
			{
				tilt.transform.rotation = Quaternion.Euler(88,0,0);
			}

			if(tilt.transform.rotation.eulerAngles.x < 65)
			{
				tilt.transform.rotation = Quaternion.Euler(65,0,0);
			}

			if(tilt.transform.rotation.eulerAngles.x > 85)
			{
				tilt.transform.rotation = Quaternion.Euler(85,0,0);
			}
		}

		//if the patient has to be  Lateral, check the moved flag again
		if(positionChanged)
		{
			//if the camera has not yet been moved and is supposed to be
			if(!moved)
			{
				//rotate the camera
				tilt.transform.Rotate(30,0,0);
				//set the flag
				moved = true;
			}
			//since the camera has moved, make sure that the camera does not go beyond the new bounds
			if(tilt.transform.rotation.eulerAngles.x < 60)
			{
				//if the camera has moved beyond the bounds, move it to the new limits
				tilt.transform.rotation = Quaternion.Euler(120,0,0);
			}
			//same applies here as well
			if(tilt.transform.rotation.eulerAngles.x > 85)
			{
				tilt.transform.rotation = Quaternion.Euler(95,0,0);
			}
		}


	}

	void FindObject()
	{

		if( GameObject.Find("trocarPurple(Clone)") )
		{
			GameObject placed = GameObject.Find ("trocarPurple(Clone)");
			//Vector3 belowBellyButton = new Vector3(-55.48f, 5.45f, -16.61f);
			//Vector3 bellyButton = new Vector3(-55.38f, 5.45f, -16.61f);

			//Debug.Log("found it!");

			//get question name
			//for that question name get the position of the trocar
			//get the index in the list for the index of the trocar
			for(int i=0; i< currentQuestion.surgerySizes.Count; ++i)
			{
				if(currentQuestion.points[i] == placed.transform.position)
				{
					if(currentQuestion.surgerySizes[placed.transform.position] == 5f)
					{
						placed.transform.localScale = new Vector3(fiveScale,fiveScale,fiveScale);
						placed.name = "trocarPurple(Clone)1";
					}
					if(currentQuestion.surgerySizes[placed.transform.position] == 10f)
					{
						//Debug.Log("beforescale = " + placed.transform.localScale + "!!");
						placed.transform.localScale = new Vector3(tenScale,tenScale,tenScale);
						//Debug.Log("scale = " + placed.transform.localScale + "<-");
						placed.name = "trocarPurple(Clone)1";
					}
					if(currentQuestion.surgerySizes[placed.transform.position] == 12f)
					{
						placed.transform.localScale = new Vector3(twelveScale,twelveScale,twelveScale);
						placed.name = "trocarPurple(Clone)1";

					}
				}
			}

		}


		//this block will be used to find positions
		/*
		if(GameObject.Find("troc") )
		{
			GameObject placements = GameObject.Find("troc");
			string tens = System.IO.File.ReadAllText("points.txt");

			using (StreamWriter writer = new StreamWriter("points.txt"))
			{
				writer.WriteLine (tens + " ");
				writer.Write("x->" + placements.transform.position.x + " z->" + placements.transform.position.z);
			}
			Destroy(placements);
		}
		*/


	}

	void FindObject(GameObject placed)
	{
		for(int i=0; i< currentQuestion.surgerySizes.Count; ++i)
		{
			if(currentQuestion.points[i] == placed.transform.position)
			{
				if(currentQuestion.surgerySizes[placed.transform.position] == 5f)
				{
					placed.transform.localScale = new Vector3(fiveScale,fiveScale,fiveScale);
					placed.name = "trocarPurple(Clone)1";
					
				}
				if(currentQuestion.surgerySizes[placed.transform.position] == 10f)
				{
					placed.transform.localScale = new Vector3(tenScale,tenScale,tenScale);
					placed.name = "trocarPurple(Clone)1";
					
				}
				if(currentQuestion.surgerySizes[placed.transform.position] == 12f)
				{
					placed.transform.localScale = new Vector3(twelveScale,twelveScale,twelveScale);
					placed.name = "trocarPurple(Clone)1";
					
				}
			}
		}

	}

	void FixPos()
	{
		//changerquestion
		if(bodyPositions[currentQuestion.text] == "Lateral")
		{
			tilt.transform.rotation = Quaternion.Euler(110,0,0);
		}
		else 
		{
			tilt.transform.rotation = Quaternion.Euler(85,0,0);
		}
	}

	void ClearAll()
	{
		foreach(Surgery questions in availableCorrectPointSets)
		{
			questions.Equals(null);
		}
		availableCorrectPointSets.Clear();
	}

	void ReadFile()
	{
	
		availableCorrectPointSets = new List<Surgery>();

		bodyPositions = new Dictionary<string, string>();

		string[] roger = surgeryList.text.Split("\n"[0]);
		//for now I will add the names of the positions that they would be on the body

		string nameSurgery = "none";
		float currentSize = 0.0f;
		int i=1;
		foreach(string contents in roger)
		{
			//Debug.Log(contents);
			if( (i-1)%3 == 0)
			{
				//Debug.Log("surgery names = " + contents);
				nameSurgery = contents;
				nameSurgery.Replace('_',' ');
			}

			if( (i+1)%3 == 0)
			{
				//Debug.Log("2");

				bodyPositions.Add(nameSurgery, contents);
				//Debug.Log(nameSurgery + " " + contents);

			}
			
			if(i%3 == 0)
			{
				List<Vector3> vectors = new List<Vector3>();
				//List<float> trocarValues = new List<float>();

				Dictionary<Vector3, float> surgerySizes = new Dictionary<Vector3, float>();

				//Debug.Log("ct initial = " + vectors.Count + " for surgery " + nameSurgery);
				List<float> coords = new List<float>();

				string [] pts = contents.Split();
				//Debug.Log(pts[0] + " points = " + pts[1]);
				int r = 1;
				foreach( string threes in pts)
				{
					//Debug.Log(r);

					if( (r%4) == 0)
					{
						coords.Add(float.Parse(threes));
						Vector3 tempVector = new Vector3(coords[1],coords[2],coords[3]);
						surgerySizes.Add(tempVector, coords[0]);
						vectors.Add (tempVector);
						//trocarValues.Add(coords[0]);
						currentSize = coords[0];

						coords.Clear();
					}
					else
					{
						coords.Add(float.Parse(threes));
					}

					r++;

				}


				//this.availableCorrectPointSets = new List<Question>()
				availableCorrectPointSets.Add(new Surgery(vectors, surgerySizes, nameSurgery));
				//Debug.Log("ct final = " + vectors.Count + " for surgery " + nameSurgery);
	
				//update question and clear vectors
			}
			i++;
		}


	}

	//public bool stuff = false;

	void MakeQuestion(Vector3 pos, float sizer)
	{
		currentQuestion.surgerySizes.Add(pos, sizer);
	}

	float distanceMagnitude(Vector3 correctPoint, Vector3 chosenPoint)
	{
		Vector3 distanceBetweenPoints = new Vector3(correctPoint.x - chosenPoint.x,
		                                            0,correctPoint.z-chosenPoint.z);
		return distanceBetweenPoints.magnitude;
	}


	// /roger


}
