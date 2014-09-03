using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class used to create object with list of points and text related to those points


public class Surgery : Question
{
	public Dictionary<Vector3, float> surgerySizes;

	public Surgery ()
	{
		points = new List<Vector3>();
		text = "Empty";
		Dictionary<Vector3, float> surgerySizes = new Dictionary<Vector3, float>();

	}
	
	public Surgery(List<Vector3> newpoints, Dictionary<Vector3, float> newsurgerySizes, string displayText)
	{
		points = newpoints;
		text = displayText;
		surgerySizes = newsurgerySizes;
	}

}
