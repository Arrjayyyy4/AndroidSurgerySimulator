using UnityEngine;
using System.Collections;
using Leap;

//Resets objects location to original location when scene loads
public class ResetLocation : MonoBehaviour {
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	private Vector3 originalScale;


	// Use this for initialization
	void Start () {
		originalPosition = transform.position;
		originalRotation = transform.localRotation;
		originalScale = transform.localScale;	
	}
	
	// Update is called once per frame
	void Update () {

		/*
		foreach (Gesture g in LeapInputEx.Controller.Frame().Gestures())
		{
			if (g.Type == Gesture.GestureType.TYPECIRCLE)
			{
				CircleGesture cgesture = new CircleGesture(g);

				if (cgesture.Progress >= 2)
				{
					ResetLoc ();
				}
			}
		}
		*/
		if(Input.GetKeyUp(KeyCode.R))
		{
			ResetLoc();
		}
	
	}

	void ResetLoc()
	{
		if(gameObject.tag == "Tool")
		{
			LeapGameObject obj = gameObject.GetComponent<LeapGameObject>();
			HandTypeBase owner = obj.owner;
			if(owner != null)
			{
				Debug.Log (obj);
				Debug.Log (obj.owner);
				//obj.owner.ChangeState(handController.activeObj.Release(handController));
				obj.canRelease = true;
				obj.DeSelect();
				obj.isStatePersistent = false;
				obj.Release(obj.owner);

			}
		}

		transform.position = originalPosition;
		transform.localRotation = originalRotation;
		transform.localScale = originalScale;
	}
}
