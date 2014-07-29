using UnityEngine;
using System.Collections;

public class makeTransparent : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		if(SimulatorController.transparent)
		{
			renderer.material.color = transparent(renderer.material.color);
		}
		else if(!SimulatorController.transparent)
		{
			renderer.material.color = notTransparent(renderer.material.color);
		}

	}

	Color transparent(Color color)
	{
		Debug.Log ("changing color");
		color.a = 0.45f;
		return color;
	}

	Color notTransparent(Color color)
	{
		Debug.Log ("changing back!");
		color.a = 1.0f;
		return color;
	} 



}
