using UnityEngine;
using System.Collections;
using Leap;

public class CameraZoom : MonoBehaviour {

	private bool zoomedIn = false;
	private bool doneZooming = true;
	private float originalZoom = 110f;
	private float zoomedValue = 60f;
	private float zoomAmount = 0f;

	// Use this for initialization
	void Start () {
		//GameObject cameraObject =  GameObject.Find("OVRCameraController");
		//camera = (OVRCameraController) cameraObject.GetComponent("OVRCameraController");
		//camera.GetVerticalFOV(ref originalZoom);
	}
	
	// Update is called once per frame
	void Update () {


		//had to disable manual calibration in rift
		//to re-enable, found in ovrmainmenu, 	void GUIShowVRVariables(), line 722
		if(Input.GetKeyUp(KeyCode.Z))
		{
			if(zoomedIn) // already zoomed in
			{
				zoomedIn = false; //zoom out
				doneZooming = false;
			}
			else //otherwise, zoom in
			{
				zoomedIn = true;
				doneZooming = false;
			}
		}

		if(Input.GetKeyDown(KeyCode.S))
		{
			if(zoomedIn) // already zoomed in
			{
				zoomedIn = false; //zoom out
				doneZooming = false;
			}
		}



		if(zoomedIn && !doneZooming)
		{
			float lerpTime = 5f; //adjust to change speed of zoom
			float value = 0f;
			//camera.GetVerticalFOV(ref value);

			zoomAmount = Mathf.Lerp(value, zoomedValue, lerpTime * Time.deltaTime);
			//camera.SetVerticalFOV(zoomAmount);

			if((int)zoomAmount == (int)zoomedValue) // if we have zoomed in enough, stop
			{
				doneZooming = true;
			}
		}
		else if(!zoomedIn && !doneZooming)
		{
			//use this to zoom out instantly
			//camera.SetVerticalFOV(originalZoom);
			//doneZooming = true;

			float lerpTime = 5f; //adjust to change speed of zoom
			float value = 0f;
			//camera.GetVerticalFOV(ref value);

			zoomAmount = Mathf.Lerp(value, originalZoom, lerpTime * Time.deltaTime);
			//camera.SetVerticalFOV(zoomAmount);

			if((int)zoomAmount == (int)originalZoom-1) // if we have zoomed in enough, stop
			{
				doneZooming = true;
			}
		}

	}


}
