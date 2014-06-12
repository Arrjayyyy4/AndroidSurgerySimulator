using UnityEngine;
using System.Collections;


public class Menu : VRGUI 
{
	public GUISkin skin;
	public static string text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnVRGUI()
	{
		GUI.skin = skin;
		GUILayout.BeginArea(new Rect(10f, 100f, Screen.width/2, Screen.height/2));

		GUILayout.Label(text);

		GUILayout.EndArea();
	}
}
