using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class used to create object with list of points and text related to those points


public class Surgery : Question
{
	public List<float> sizes;
	
	public Surgery ()
	{
		points = new List<Vector3>();
		text = "Empty";
		sizes = new List<float>();
	}
	
	public Surgery(List<Vector3> newpoints, List<float> newsizes, string displayText)
	{
		sizes = newsizes;
		points = newpoints;
		text = displayText;
	}

}
