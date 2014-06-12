using UnityEngine;
using System.Collections;

public class SwitchCamera : MonoBehaviour {

	public Camera camPlayerLeft;
	public Camera camPlayerRight;
	public Camera camTopLeft;
	public Camera camTopRight;
	public Camera simulationLeft;
	public Camera simulationRight;
	public GameObject leapController;
	public GameObject player;
	public GameObject simulationCamera;
	public UnityHand hand;
	public LeapGameObject scalpel;
	public GameObject textBackground;
	public TextMesh questionText;

	int count;


	// Use this for initialization
	void Start () {
		camPlayerLeft.enabled = true;
		camPlayerRight.enabled = true;
		camTopLeft.enabled = false;
		camTopRight.enabled = false;
		simulationRight.enabled = false;
		simulationLeft.enabled = false;
		textBackground.renderer.enabled = false;
		questionText.renderer.enabled = false;
		count = 0;
		/*
		if(this.tag == "Player")
		{
			foreach(Camera cam in this.GetComponentsInChildren<Camera>())
				cam.enabled = true;
			//this.GetComponentsInChildren<Camera>().enabled = true;
			GameObject topCamera =  GameObject.Find("TopCamera/OVRCameraController");
			foreach(Camera cam in topCamera.GetComponentsInChildren<Camera>())
				cam.enabled = false;

		}*/
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(Input.GetKeyUp(KeyCode.C))
		{
			SwitchCam();
			if(count == 2)
				count = 0;
			else
				count++;
		}
	
	}

	void SwitchCam()
	{


		//switch to top down view
		if(count == 0)
		{
			camPlayerLeft.enabled = false;
			camPlayerRight.enabled =  false;
			
			
			camTopLeft.enabled = true;
			camTopRight.enabled = true;

			questionText.transform.localPosition = new Vector3(-72.428f, -15.839f, -26.502f);
			questionText.transform.localRotation = Quaternion.Euler(90f,0,0);
			questionText.fontSize = 40;
			
			textBackground.transform.localPosition = new Vector3(-55.419f, 5.364f, -17.121f);
			textBackground.transform.localRotation = Quaternion.Euler(90f,0,0);
			textBackground.transform.localScale = new Vector3(1.4f, 0.257f, -.0226f);

			textBackground.renderer.enabled = true;
			questionText.renderer.enabled = true;
		
		}
		//switch to simulation view
		else if(count == 1)
		{
			camTopLeft.enabled = false;
			camTopRight.enabled = false;
			
			simulationLeft.enabled = true;
			simulationRight.enabled = true;

			//change parent
			leapController.transform.parent = simulationCamera.transform;
			leapController.transform.localPosition = new Vector3(0f, -0.75f, 0.25f);
			
			//no rotation
			leapController.transform.rotation = Quaternion.identity;
			
			//put scalpel in hand
			scalpel.gameObject.SetActive(true);
			hand.handType.ChangeState(scalpel.Activate(hand.handType));
			
			//show question background
			questionText.transform.localPosition = new Vector3(-64.054f, 3.057f, .08837f);
			questionText.transform.localRotation = Quaternion.Euler(0f,0,0);
			questionText.fontSize = 24;

			textBackground.transform.localPosition = new Vector3(-55.419f, 5.419f, -14.358f);
			textBackground.transform.localRotation = Quaternion.Euler(0f,0,0);
			textBackground.transform.localScale = new Vector3(4.435f, 1f, -.0226f);

			textBackground.renderer.enabled = true;
			questionText.renderer.enabled = true;


		}
		//switch to player view
		else if(count == 2)
		{
			simulationLeft.enabled = false;
			simulationRight.enabled = false;

			camPlayerLeft.enabled = true;
			camPlayerRight.enabled =  true;

			//change parent
			leapController.transform.parent = player.transform;
			leapController.transform.localPosition = new Vector3(0f, -0.5f, 1f);
			
			//no rotation
			leapController.transform.rotation = Quaternion.identity;
			
			//scalpel.gameObject.SetActive(false);
			//hand.handType.ChangeState(scalpel.Release(hand.handType));
			scalpel.canRelease = true;
			scalpel.DeSelect();
			scalpel.isStatePersistent = false;
			scalpel.Release(scalpel.owner);
			
			//disable question/background

			textBackground.renderer.enabled = false;
			questionText.renderer.enabled = false;


		}

		/*
		if(this.tag == "Player")
		{
			Debug.Log ("here");
			GameObject topCamera =  GameObject.Find("TopCamera/OVRCameraController");
			foreach(Camera cam in topCamera.GetComponentsInChildren<Camera>())
				cam.enabled = true;

			foreach(Camera cam in this.GetComponentsInChildren<Camera>())
				cam.enabled = false;
		}
		else
		{
			GameObject playerCamera = GameObject.Find("PlayerController/OVRCameraController");
			foreach(Camera cam in playerCamera.GetComponentsInChildren<Camera>())
				cam.enabled = true;
			this.GetComponentInChildren<Camera>().enabled = false;
			foreach(Camera cam in this.GetComponentsInChildren<Camera>())
				cam.enabled = false;

		}*/
	}
}
