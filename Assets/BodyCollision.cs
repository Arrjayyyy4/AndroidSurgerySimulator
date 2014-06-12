using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;

public class BodyCollision : MonoBehaviour {

	static private int count;
	static private bool firstLineDrawn;
	static private Vector3 firstLineMidpoint;
	static private Vector3 secondLineMidpoint;
	static private List<Vector3> firstLinePoints;
	static private List<Vector3> secondLinePoints;

	// Use this for initialization
	void Start () {
		count = 0;
		firstLineDrawn = false;
		firstLinePoints=  new List<Vector3>();
		secondLinePoints=  new List<Vector3>();

	}
	
	// Update is called once per frame
	void Update ()
	{	
		foreach (Gesture g in LeapInputEx.Controller.Frame().Gestures())
		{
			if (g.Type == Gesture.GestureType.TYPECIRCLE)
			{
				CircleGesture cgesture = new CircleGesture(g);
				
				if (cgesture.Progress >= 2)
				{
					count = 0;
					ResetLine ();

					int x = 1;
				}
			}
		}
	}

	void OnTriggerEnter (Collider collider) 
	{/*
		Debug.Log(collider.gameObject.name);
		if(collider.gameObject.name == "Hemostat 1")
		{
			Vector3 collisionPoint = this.transform.position;
			collider.ClosestPointOnBounds(collisionPoint);
			Debug.Log ("We have a hit!");
			Debug.Log (collisionPoint);
			Debug.DrawLine(collisionPoint, collisionPoint + collider.ClosestPointOnBounds(this.transform.position).normalized, Color.green, 2, false);
		}*/
	}

	void OnCollisionEnter(Collision collision) 
	{ 
		if ( this.gameObject.active == false )
			return;

		//one line from first two points
		//second line from second two points

		//Debug.Log ("Collider: " + collision.collider.name);

		//Debug.Log("Collision Body Start:" + collision.gameObject.name);

		if(collision.gameObject.tag == "Tool")
		{

			GameObject line = GameObject.Find("mergedbody/Line");
			LineRenderer lineRenderer = (LineRenderer) line.GetComponent("LineRenderer");

			GameObject line2 = GameObject.Find("mergedbody/Line2");
			LineRenderer lineRenderer2 = (LineRenderer) line2.GetComponent("LineRenderer");

			Debug.Log (lineRenderer.name);
			Color color1 = Color.red;
			Color color2 = Color.blue;
			lineRenderer.SetColors(color1,color1);
			lineRenderer2.SetColors(color2,color2);

			Vector3 point = collision.contacts[0].point;
			Vector3 pointNormal = collision.contacts[0].normal;

			//if not an empty point, and the collision happened on the way down, not on the way up
			if(!point.Equals(Vector3.zero) && pointNormal.z < 0)
			{

				//if first set of points
				if(count < 2)
				{
					if(!firstLineDrawn)
					{
						lineRenderer.SetVertexCount(count+1);
						lineRenderer.SetPosition(count++,point);
						firstLinePoints.Add(point);

						if(count == 2)
						{
							count = 0;
							firstLineDrawn = true;

						}
						//reset count
						//move onto second line next time through
					}
					else if(firstLineDrawn)
					{
						lineRenderer2.SetVertexCount(count+1);
						lineRenderer2.SetPosition(count++,point);
						secondLinePoints.Add(point);
					}
					if(count == 2) //after second line is drawn
					{
						//GameObject.CreatePrimitive(PrimitiveType.Quad); // make cube with line points
						Vector3 midpoint1 = Vector3.Lerp(firstLinePoints[0], firstLinePoints[1],0.5f);
						Vector3 midpoint2 = Vector3.Lerp(secondLinePoints[0], secondLinePoints[1],0.5f);
						Physics.Raycast(midpoint1,Vector3.up);
						Physics.Raycast(midpoint2,Vector3.up);
					}
				}
			}


			/*
			if(!point.Equals(Vector3.zero) && pointNormal.z > 0)
			{
				lineRenderer.SetVertexCount(count+1);
				lineRenderer.SetPosition(count++,point);

			}*/

			/*
			for( int x = 0; x < collision.contacts.Length-1; x++)
			{
				Vector3 firstPoint = collision.contacts[x].point;
				Vector3 secondPoint = collision.contacts[x+1].point;
				Debug.DrawLine(firstPoint, secondPoint, Color.green, 2, false);
			}*/
		/*
		foreach (ContactPoint contact in collision.contacts)
		{
			Debug.DrawLine(contact.point, contact.point + contact.normal, Color.green, 2, false);
		}*/

		}
	}

	void ResetLine()
	{
		GameObject line = GameObject.Find("mergedbody/Line");
		LineRenderer lineRenderer = (LineRenderer) line.GetComponent("LineRenderer");
		lineRenderer.SetVertexCount(0);

		GameObject line2 = GameObject.Find("mergedbody/Line2");
		LineRenderer lineRenderer2 = (LineRenderer) line2.GetComponent("LineRenderer");
		lineRenderer2.SetVertexCount(0);

	}

	//no longer used
	/*
	void OnCollisionStay(Collision collision)
	{

		//Debug.Log ("Collider: " + collision.collider.name);
		
		Debug.Log("Collision Body Stay:" + collision.gameObject.name);
		
		// if gameobject is hand, disable old collider, enable new one
		if(collision.gameObject.tag == "Tool")
		{
			/*
			//for( int x = 0; x < collision.contacts.Length-1; x++)
			for( int x = 0; x < collision.contacts.Length/5; x++)
			{
				Vector3 firstPoint = collision.contacts[x].point;
				Vector3 secondPoint = collision.contacts[x+1].point;
				Debug.DrawLine(firstPoint, secondPoint, Color.cyan, 2, false);
			}*/
			/*
			GameObject line = GameObject.Find("mergedbody/Line");
			Debug.Log (line.name);
			LineRenderer lineRenderer = (LineRenderer) line.GetComponent("LineRenderer");
			Debug.Log (lineRenderer.name);
			Color color1 = Color.yellow;
			Color color2 = Color.red;
			lineRenderer.SetColors(color1,color2);
			
			//lineRenderer.SetWidth(0.5f, 0.5f);
			lineRenderer.SetVertexCount(collision.contacts.Length);

			for(int x = 0; x < collision.contacts.Length-1; x++)
			{
				Vector3 point = collision.contacts[x].point;
				if(!point.Equals(Vector3.zero))
				{
					lineRenderer.SetPosition(x,point);
				}
			}

		}


		
	}*/
//
//	void OnCollisionExit(Collision collision)
//	{
//		if(collision.gameObject.name == "Hemostat 1")
//		{
//			/*
//			//for( int x = 0; x < collision.contacts.Length-1; x++)
//			for( int x = 0; x < collision.contacts.Length/5; x++)
//			{
//				Vector3 firstPoint = collision.contacts[x].point;
//				Vector3 secondPoint = collision.contacts[x+1].point;
//				Debug.DrawLine(firstPoint, secondPoint, Color.cyan, 2, false);
//			}*/
//			
//			GameObject line = GameObject.Find("mergedbody/Line");
//			Debug.Log (line.name);
//			LineRenderer lineRenderer = (LineRenderer) line.GetComponent("LineRenderer");
//			Debug.Log (lineRenderer.name);
//			Color color1 = Color.yellow;
//			Color color2 = Color.red;
//			lineRenderer.SetColors(color1,color2);
//			/*
//			lineRenderer.SetWidth(0.5f, 0.5f);*/
//			lineRenderer.SetVertexCount(collision.contacts.Length);
//			
//			for(int x = 0; x < collision.contacts.Length-1; x++)
//			{
//				lineRenderer.SetPosition(x,collision.contacts[x].point);
//			}
//			
//		}
//	}

}
