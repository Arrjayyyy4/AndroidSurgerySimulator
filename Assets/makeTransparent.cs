using UnityEngine;
using System.Collections;

public class makeTransparent : MonoBehaviour {

	// Use this for initialization
	void Start () {

		renderer.material.color = changeAlpha(renderer.material.color);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Color changeAlpha(Color color)
	{
		color.a = 0.5f;
		return color;
	}



}
