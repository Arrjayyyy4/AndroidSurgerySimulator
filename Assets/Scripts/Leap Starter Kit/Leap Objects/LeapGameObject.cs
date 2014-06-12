using UnityEngine;
using System.Collections;
using System.Linq;
using Leap;

/// <summary>
/// All Leap Objects inherit from LeapGameObject
/// </summary>
public abstract class LeapGameObject : MonoBehaviour
{
    public Transform grabCenterOffset;
    public bool isStatePersistent = false; // When object is held, object can leave state
    public bool dropOnLost = true; // When hand is lost, drop the object or hide it
    public bool canRelease = true; // When object is held, object can leave state by opening hand
    public bool canGoThroughGeometry = true; // Object collides with the world
    public bool canHighlight = true; // When hand collides with object, object is highlighted
    public bool handIsVisible = true; // Draw hand when object is interacted with
    public bool canUseBothHands = false; // Usable with multiple hands
	

    [HideInInspector]
    public HandTypeBase owner;
    [HideInInspector]
    public bool isHeld = false;
    [HideInInspector]
    public bool collisionOccurred;

    private Renderer[] renderers;
	private int[] matCount;
	public Material highlight;
	
	private float timeHoldStart = 0f;
	private bool woundFixed = false;

    protected virtual void Start()
    {
        if (canHighlight)
            canHighlight = InitializeHighlights(); // check whether highlight material is created properly, if not disallow highlighting

		if(this.tag == "Wound")
		{
			GameObject bandage = GameObject.Find("Bandage Parent");
			bandage.GetComponentInChildren<MeshRenderer>().enabled = false;
		}
    }

	void Update ()
	{	
		if(this.tag == "Wound")
		{
			foreach (Gesture g in LeapInputEx.Controller.Frame().Gestures())
			{
				if (g.Type == Gesture.GestureType.TYPECIRCLE)
				{
					CircleGesture cgesture = new CircleGesture(g);
					
					if (cgesture.Progress >= 2)
					{
						GameObject bandage = GameObject.Find("Bandage Parent");
						bandage.GetComponentInChildren<MeshRenderer>().enabled = false;
						
						GameObject wound = GameObject.Find("Decals Wound/Decals Mesh Renderer");
						wound.GetComponentInChildren<MeshRenderer>().enabled = true;
						woundFixed = false;
					}
				}
			}
		}
	}


    public virtual void UpdateTransform(HandTypeBase t)
    {
        Vector3 grabOffsetPos = new Vector3();
        Quaternion grabOffsetRot = Quaternion.identity;

        if (grabCenterOffset)
        {
            grabOffsetPos = grabCenterOffset.transform.position - transform.position;
            grabOffsetRot = grabCenterOffset.localRotation;
        }

        transform.position = t.transform.position - grabOffsetPos;
        transform.rotation = t.transform.rotation;
        transform.rotation *= grabOffsetRot;
    }

    /// <summary>
    /// First interaction with HandType
    /// Implement highlighting or other "selection" fx here
    /// </summary>
    public virtual void Select()
    {
		if(canHighlight)
			HighlightObject(true);

    }

    /// <summary>
    /// Called when Leap Object is no longer interacting with Leap Hands
    /// </summary>
    public virtual void DeSelect()
    {
		if (canHighlight)
			HighlightObject(false);
    }

    /// <summary>
    /// Method is triggered by "Grab" or similar method from current state of HandType
    /// </summary>
    /// <returns>new Leapstate or null to remain in current state</returns>
    public virtual LeapState Activate(HandTypeBase h)
    {
        if (!owner)
        {
            owner = h;
        }

		if(this.tag == "Tool")
		{
			this.GetComponent<CapsuleCollider>().enabled = false;
			this.GetComponent<BoxCollider>().enabled = true;
		}

        return new LeapGodHandState(this);
    }

    public virtual LeapState Release(HandTypeBase h)
    {
        if (!isStatePersistent)
        {
            if (h && h.activeObj)
                h.activeObj = null;
            else
                owner.activeObj = null;


			if(this.tag == "Tool")
			{
				this.GetComponent<CapsuleCollider>().enabled = true;
				this.GetComponent<BoxCollider>().enabled = false;
				this.isStatePersistent = true;
				this.canRelease = false;
				owner.ShowHand();
			}

			owner = null;

            return new LeapGodHandState(); // each object should return a state
        }
        return null;
    }

    public virtual void HighlightObject(bool onOff)
    {
		if (onOff) { AddHighlight(); }
		else { RemoveHighlight(); }
    }

	protected virtual void AddHighlight()
	{
		
		for(int i = 0; i < renderers.Length; i++)
		{
			//Debug.Log("renderer.mats.lenght = " + renderers[i].materials.Length + ", matcount = " + matCount[i]);
			if (renderers[i].materials.Length >= matCount[i] + 1) // if we've already added a highlight, do not add another
				break;

			Material[] mats = new Material[matCount[i] + 1];
			for (int j = 0; j < matCount[i]; j++)
				mats[j] = renderers[i].materials[j];

			mats[mats.Length - 1] = highlight;

			renderers[i].materials = mats;
		}
	}

	protected virtual void RemoveHighlight()
	{
		
		for(int i = 0; i < renderers.Length; i++)
		{
			if (renderers[i].materials.Length == matCount[i]) // If for some reason the highlight does not exist, do not attempt to remove it
				continue;

			Material[] mats = new Material[matCount[i]];
			for (int j = 0; j < mats.Length; j++)
				mats[j] = renderers[i].materials[j];

			renderers[i].materials = mats;
		}
	}

    protected virtual bool InitializeHighlights()
    {
        // Add in one extra blank material for highlighting
        // This is to ensure that highlight colors could be properly accessed regardless of pre-existing material / shader types

		renderers = GetComponentsInChildren<Renderer>()
		   .Where(r => r.gameObject.GetComponent<ParticleSystem>() == null && r.gameObject.GetComponent<ParticleRenderer>() == null)
		   .ToArray(); // Highlighting ignores renderers attached to particle systems


		if (renderers == null) // If there are no renderers, disallow highlighting
			return false;

		matCount = new int[renderers.Length];
		for (int i = 0; i < renderers.Length; i++)
			matCount[i] = renderers[i].materials.Length;
		
		if(!highlight)
			highlight = new Material(Shader.Find("Unlit/Texture"));	
        if (!highlight)
            return false; // Material creation did not succeed, disallow highlighting

        return true; // succeeding in creating material, allow highlighting
    }
	
    public virtual void StartTimer()
    {
		if(this.tag == "Wound")
		{
			if(!woundFixed)
			{
				if(!collisionOccurred)
				{
		        	collisionOccurred = true;
					timeHoldStart = Time.time;
					Debug.Log ("initial wound collision detected");
				}
				else
				{

					Debug.Log ("continued wound collision detected");

					float holdTimeNeeded = 3f;

					if(timeHoldStart + holdTimeNeeded < Time.time)
					{
						//make wound disappear
						GameObject wound = GameObject.Find("Decals Wound/Decals Mesh Renderer");
						wound.GetComponentInChildren<MeshRenderer>().enabled = false;
						woundFixed = true;

						GameObject bandage = GameObject.Find("Bandage Parent");
						bandage.GetComponentInChildren<MeshRenderer>().enabled = true;
					}
				}
			}
		
		}
    }

    public virtual void ResetTimer()
    {
		collisionOccurred = false;
		timeHoldStart = 0f;
    }
}
