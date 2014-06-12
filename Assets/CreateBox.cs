using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class CreateBox : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		Leap.Frame frame  = LeapInputEx.Frame;
		if( frame == null ) 
			return;	
		//Debug.Log(frame.Gestures().Count);
		foreach(Gesture gesture in frame.Gestures())
		{
			/*
			if(gesture.Type == Leap.Gesture.GestureType.TYPESWIPE)
			{
				//frozen = true;
				
    			//SwipeGesture swipeGesture = SwipeGesture(gesture);
				//if this is a new swipe
				if(gestureID != gesture.Id)
				{
					if(frozen)
					{
						Destroy(selectionBox);
						selectionBox = null;
					}
					//frozen = !frozen;
					/*
					//destroy old box, so if this isn't first box you don't just move old one, creates problems
					if(frozen == false)
					{
						Destroy(selectionBox);
						selectionBox = null;	
					}
					Debug.Log("Swiper no swiping!");
				}
			}*/

			if(gesture.Type == Leap.Gesture.GestureType.TYPECIRCLE)
			{
				if(gestureID != gesture.Id)
				{
					
					CircleGesture circleGesture = new CircleGesture(gesture);
					if(circleGesture.Progress >=2)
					{
						frozen = !frozen;
						//destroy old box, so if this isn't first box you don't just move old one, creates problems
						if(frozen == false)
						{
							Destroy(selectionBox);
							selectionBox = null;	
						}
						Debug.Log("Circling!");
					}
				}
			}
			
			if(gesture.Type == Leap.Gesture.GestureType.TYPESCREENTAP)
			{
				if(gestureID != gesture.Id )
				{				
					ScreenTapGesture screenTapGesture = new ScreenTapGesture(gesture);
//					Debug.Log (screenTapGesture.Direction);
					//Debug.Log(screenTapGesture.DurationSeconds);
					if(screenTapGesture.Direction.z < 0)
					{
						frozen = !frozen; // if frozen, get rid of old box.  
						//destroy old box, so if this isn't first box you don't just move old one, creates problems
						if(frozen == false)
						{
							Destroy(selectionBox);
							selectionBox = null;	
						}
						Debug.Log("Tapped!");
						//Debug.Log (gesture);
					}
				}
			}
						
		}
		
		HandList hands = frame.Hands;
		FingerList fingers = frame.Fingers;
				
        Hand lh = hands.Leftmost;
        Hand rh = hands.Rightmost;
		Vector3 rpos = rh.PalmPosition.ToUnityScaled();
		Vector3 rdir = rh.Direction.ToUnity();
		Vector3 rnorm = rh.PalmNormal.ToUnity();
		Vector3 lpos = lh.PalmPosition.ToUnityScaled();
		
		float farthestZ = -10000f;
		int thumbID = -1;
		int otherFingerID = -1;
		int count = 0;
		
		GameObject hand = GameObject.Find("right");
		
		#region defining thumb/other fingers
		//for(int x = 0; x < m.fin	
		//foreach(Finger finger in fingers)
		foreach(Finger finger in rh.Fingers)	
		{
			count+=1;
			if(count == 1)
			{
				otherFingerID = finger.Id;	
			}
			if(finger.TipPosition.z > farthestZ)
			{
				thumbID = finger.Id;
				farthestZ = finger.TipPosition.z;
				
			}
			else
			{
				otherFingerID = finger.Id;
			}
		}
		
		Vector3 rightHandPosition = (frame.Finger(thumbID).TipPosition.ToUnityScaled() + frame.Finger (otherFingerID).TipPosition.ToUnityScaled()) /2;
		count = 0;
		foreach(Finger finger in lh.Fingers)	
		{
			count+=1;
			if(count == 1)
			{
				otherFingerID = finger.Id;	
			}
			if(finger.TipPosition.z > farthestZ)
			{
				thumbID = finger.Id;
				farthestZ = finger.TipPosition.z;
				
			}
			else
			{
				otherFingerID = finger.Id;
			}
		}
		Vector3 leftHandPosition = (frame.Finger(thumbID).TipPosition.ToUnityScaled() + frame.Finger (otherFingerID).TipPosition.ToUnityScaled()) /2;
		
		#endregion

		//get positions of left and right hand models
		if(GameObject.Find("RightRiggedHand(Clone)"))
		{
			rpos = GameObject.Find("RightRiggedHand(Clone)").transform.position;
			handsObject = GameObject.Find("RightRiggedHand(Clone)");
		}
		if(GameObject.Find("LeftRiggedHand(Clone)"))
		{
			lpos = GameObject.Find("LeftRiggedHand(Clone)").transform.position;
		}

		//box position is in the middle of two hand models
		Vector3 boxPosition = Vector3.Lerp(lpos,rpos, 0.5f);	

		if(!frozen)
		{
			//Debug.Log ("boxPosition: " + boxPosition);
			if(selectionBox == null && GameObject.Find("RightRiggedHand(Clone)"))
			{
				Transform box = Instantiate(selectionBoxObject,new Vector3(0,0,0),Quaternion.identity) as Transform;
				selectionBox = box.gameObject;//(GameObject)(selectionBox);
				selectionBox.renderer.material.color = Color.blue;
				//GameObject handsObject = GameObject.Find("RightRiggedHand(Clone)");
				//handsObject.transform.position = new Vector3(0,0,0);

				//set position of box
				selectionBox.transform.localPosition = boxPosition;// + new Vector3(boxPosition.x,0,0);
				//selectionBox.transform.parent = handsObject.transform;


				//don't rotate box based on hands rotation
				selectionBox.transform.rotation =  Quaternion.identity;
				//selectionBox.transform.rotation = selectionBox.transform.parent.rotation;
				//selectionBox.transform.Rotate(new Vector3(0,0,90f));
				//selectionBox.AddComponent("BoxCollider");
				//Debug.Log("not frozen, doesn't exist");
			}
			if(selectionBox != null && GameObject.Find("RightRiggedHand(Clone)"))
			{
				/*
				Destroy(selectionBox);
				selectionBox = null;

				Transform box = Instantiate(selectionBoxObject,new Vector3(0,0,0),Quaternion.identity) as Transform;
				selectionBox = box.gameObject;//(GameObject)(selectionBox);
				selectionBox.renderer.material.color = Color.blue;
				GameObject handsObject = GameObject.Find("Primary Hand");
				//handsObject.transform.position = new Vector3(0,0,0);
				selectionBox.transform.localPosition = new Vector3(0f,-2f,2.5f) + boxPosition;
				selectionBox.transform.parent = handsObject.transform;*/
				
				
				//GameObject handsObject = GameObject.Find("RightRiggedHand(Clone)");

				selectionBox.transform.localPosition = boxPosition;// + new Vector3(boxPosition.x,0,0);
				//selectionBox.transform.parent = handsObject.transform;
					
				selectionBox.transform.rotation = Quaternion.identity;
				//selectionBox.transform.rotation = selectionBox.transform.parent.rotation;

				Vector3 handDiff = lpos- rpos;
				float xDiff = (float)(Math.Sqrt(Math.Pow(handDiff.x, 2f)))/2f;
				float yDiff = (float)(Math.Sqrt(Math.Pow (handDiff.y, 2f)))*.75f;
				float zDiff = (float)(Math.Sqrt(Math.Pow (handDiff.z, 2f)))*1f;


				//limit how small box can be
				float minScale = 0.1f;

				if(xDiff < minScale)
				{
					xDiff = minScale;
				}
				if(yDiff < minScale)
				{
					yDiff = minScale;	
				}
				if(zDiff < minScale)
				{
					zDiff = minScale;
				}

				//limit how large box can be
				float maxScale = 1f;

				if(xDiff > maxScale)
				{
					xDiff = maxScale;
				}
				if(yDiff > maxScale)
				{
					yDiff = maxScale;	
				}
				if(zDiff > maxScale)
				{
					zDiff = maxScale;
				}


				Vector3 newScale = new Vector3(xDiff,yDiff,zDiff);
				//Debug.Log (newScale);
				
				selectionBox.transform.localScale = newScale;

				/*
				if(xDiff > 1 && yDiff > 1 && zDiff > 1) // minimum scale
				{
					selectionBox.transform.localScale = newScale;
				}
				else
				{
					selectionBox.transform.localScale = Vector3.one;
				}
				*/
				//Debug.Log("not frozen, but exists");
			}
		}
		else
		{
			if(selectionBox != null)
			{
				selectionBox.renderer.material.color = Color.red;
				Vector3 temp = selectionBox.transform.position;			
				selectionBox.transform.parent = null;
				selectionBox.transform.position = temp;
				//Debug.Log("frozen and exists");
				//Destroy(selectionBox);
				//selectionBox = null;
			}
		}
		 
	}
	
	bool CheckGesture()
	{
		
		return true; //comment out to enable attempted gesture recognition
		
		Leap.Frame frame  = LeapInputEx.Frame;
		if( frame == null ) 
			return false;	
		
		HandList hands = frame.Hands;
		FingerList fingers = frame.Fingers;
				
        Hand lh = hands.Leftmost;
        Hand rh = hands.Rightmost;
		Vector3 rpos = rh.PalmPosition.ToUnityScaled();
		Vector3 rdir = rh.Direction.ToUnity();
		Vector3 rnorm = rh.PalmNormal.ToUnity();
		
		float farthestZ = -10000f;
		int thumbID = -1;
		int otherFingerID = -1;
		int count = 0;
		
		bool rightHandMakingGesture = false;

		
		//if( making gesture)
		GameObject hand = GameObject.Find("right");

		//for(int x = 0; x < m.fin	
		//foreach(Finger finger in fingers)
		foreach(Finger finger in rh.Fingers)	
		{
			count+=1;
			if(count == 1)
			{
				otherFingerID = finger.Id;	
			}
			if(finger.TipPosition.z > farthestZ)
			{
				thumbID = finger.Id;
				farthestZ = finger.TipPosition.z;
				
			}
			else
			{
				otherFingerID = finger.Id;
			}
		}
		
		float palmFingerAngle = frame.Finger(otherFingerID).Direction.AngleTo(rh.Direction);
		//Debug.Log(palmFingerAngle);
		
		if(palmFingerAngle < .6 && palmFingerAngle > .35)
		{
			return true;	
		}
		else
			return false;
		
		/*
		//frame.Finger(thumbID).TipPosition.ToUnityScaled())
		//Debug.DrawRay(frame.Finger(thumbID).TipPosition.ToUnityScaled(),Vector3.forward * 1000f, Color.yellow);
		//Debug.Log(frame.Finger(thumbID).TipPosition.ToUnityScaled());
		
		if(Physics.Raycast(rh.Fingers[thumbID].StabilizedTipPosition.ToUnityScaled(),Vector3.forward))
		{
			rightHandMakingGesture = true;
			return true;
		}
		
		//if(count == 2 && rh.Fingers[thumbID].StabilizedTipPosition.ToUnityScaled().
		//Vector3 rightHandPosition = (frame.Finger(thumbID).TipPosition.ToUnityScaled() + frame.Finger (otherFingerID).TipPosition.ToUnityScaled()) /2;
		
		foreach(Finger finger in lh.Fingers)	
		{
			count+=1;
			if(count == 1)
			{
				otherFingerID = finger.Id;	
			}
			if(finger.TipPosition.z > farthestZ)
			{
				thumbID = finger.Id;
				farthestZ = finger.TipPosition.z;
				
			}
			else
			{
				otherFingerID = finger.Id;
			}
		}
			
			//return true;
		//else
			return false;
			*/
	}
			
	public Transform selectionBoxObject = null;
	private GameObject selectionBox = null;
	private bool frozen = false;
	private int gestureID = -1;
	GameObject handsObject = null;
}
