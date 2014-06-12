using UnityEngine;
using System.Collections;

public class SurgeryQuiz : MonoBehaviour {
	//This quiz will askf or patient body positions and trocar size for each surgery

	float trocarSize = 0.0f;
	string bodyPosition = "";
	bool pos = false;
	bool size = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		string surgeryQuestion = "Function works";
		GUI.Box (new Rect(100,400,100,100),surgeryQuestion);
		if(pos)
		{
			//if() check current surgery question
				if(GUI.Button(new Rect(100,100,100,100),"facing left"))
					bodyPosition = "facing left";
				if(GUI.Button(new Rect(100,200,100,100),"facing right"))
					bodyPosition = "facing right";
				if(GUI.Button(new Rect(100,300,100,100),"facing up"))
					bodyPosition = "facing up";
			//if() check another question ...
		}
			
	}

	void Position()
	{
		pos = true;
	}
	void TrocarSize()
	{
		
	}
}
