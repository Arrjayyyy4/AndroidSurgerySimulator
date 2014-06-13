using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SimulatorController : MonoBehaviour {

	//trocar model used to create trocars
	public GameObject trocar;
	//scalpel used
	public GameObject scalpel;

	public TextMesh questionText;

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
	
	// Use this for initialization
	void Start () 
	{
		count = 0;
		timer = 0f;
		incisionDelay = 1.5f;
		lastIncisionTime = Time.time;
		canPlaceTrocars = false;
		randomizeQuestions = false;
		questionIndex = 0;
		bodyTransparent = false;
		numCorrect = 0;

		currentTime = System.DateTime.Now;
		Debug.Log (currentTime.ToShortTimeString());
		fileName = currentTime.ToString("MMddHHmmss") + ".txt";


		correctPoints = new List<Vector3>();
		chosenPoints = new List<Vector3>();
		availableCorrectPointSets = new List<Question>();
		visibleTrocars = new List<GameObject>();

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


		randomQuestionsLeft = availableCorrectPointSets.Count;

		/*
		//add all available indices to list for random picking later
		for(int x = 0; x < availableCorrectPointSets.Count; x++)
		{
			randomQuestionIndices.Add (x);
		}*/

		changeQuestion();

		//make skin opaque
		//this.renderer.materials[1].shader = Shader.Find("Diffuse");



	}
	
	// Update is called once per frame
	void Update () 
	{

		/*
		//after 5 seconds pass, show answers
		if(timer!= 0 && timer + 3f < Time.time)
		{
			
			checkTrocars();
			//make skin transparent
			this.renderer.materials[1].shader = Shader.Find("Transparent/VertexLit with Z");
			timer = 0f;
		}*/


		if(Input.GetKeyUp(KeyCode.Alpha2))
		{
			checkTrocars();
			//make skin transparent
			this.renderer.materials[1].shader = Shader.Find("Transparent/VertexLit with Z");
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

	}

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

	void displayTrocars()
	{

		foreach(Vector3 point in correctPoints)
		{
			GameObject newTrocar = (GameObject)Instantiate(trocar,point,Quaternion.Euler(270f,0f,0f));//Quaternion.identity);
			MeshRenderer[] trocarRenderers = newTrocar.GetComponentsInChildren<MeshRenderer>();
			
			visibleTrocars.Add(newTrocar);
			
		}

		this.renderer.materials[1].shader = Shader.Find("Transparent/VertexLit with Z");
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
			if(randomizeQuestions && canPlaceTrocars)
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
		this.renderer.materials[1].shader = Shader.Find("Diffuse");
		bodyTransparent = false;

		//remove the chosen points
		chosenPoints.Clear();

		//remove any points that weren't selected
		correctPoints.Clear();

		timer = 0f;

		numCorrect = 0;
		
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
		this.renderer.materials[1].shader = Shader.Find("Diffuse");

		bodyTransparent = false;

		//remove the chosen points
		chosenPoints.Clear();

		numCorrect = 0;

	}
}
