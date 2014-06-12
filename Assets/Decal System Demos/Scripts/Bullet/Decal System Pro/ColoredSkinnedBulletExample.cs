//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2013-2014 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.DecalSystem;

namespace Edelweiss.DecalSystem.Example {

	public class ColoredSkinnedBulletExample : MonoBehaviour {

			// The prefab with the ready to use DS_SkinnedDecals. The material and uv rectangles are set up.
		[SerializeField] private DS_SkinnedDecals m_DecalsPrefab = null;
		
			// The reference to the instantiated prefab's DS_SkinnedDecals instance.
		private DS_SkinnedDecals m_DecalsInstance;
		
			// We keep track of the projectors that we are creating at runtime. If we have more than
			// the maximum number of projectors, we remove the oldest ones.
		private List <SkinnedDecalProjector> m_DecalProjectors = new List <SkinnedDecalProjector> ();
		[SerializeField] private int m_MaximumNumberOfProjectors = 10;
		
			// We need a decals mesh and a cutter to create the decal projectors at runtime.
		private SkinnedDecalsMesh m_DecalsMesh;
		private SkinnedDecalsMeshCutter m_DecalsMeshCutter;
		
			// We can't place the projector exactly at the place where the raycast hits a collider. Instead,
			// it needs to be moved back, such that the projection looks believable. This value indicates how
			// many units the projector needs to be moved back.
		[SerializeField] private float m_DecalProjectorOffset = 0.5f;
		
			// The size, culling angle and mesh offset for the decal projectors for the bullets we are about to
			// create at runtime.
		[SerializeField] private Vector3 m_DecalProjectorScale = new Vector3 (0.2f, 2.0f, 0.2f);
		[SerializeField] private float m_CullingAngle = 80.0f;
		[SerializeField] private float m_MeshOffset = 0.0f;
		
			// We iterate through all the defined uv rectangles. This one inicates which index we are using at
			// the moment.
		private int m_UVRectangleIndex = 0;
		
			// Color iterator.
		private int m_ColorIndex = 0;
		
			// Move on to the next uv rectangle index. We call this method after each projection. In practice
			// you certainly don't do that, but pick an appropriate one depending on the surface that was hit.
		private void NextUVRectangleIndex () {
			m_UVRectangleIndex = m_UVRectangleIndex + 1;
			if (m_UVRectangleIndex >= m_DecalsInstance.CurrentUvRectangles.Length) {
				m_UVRectangleIndex = 0;
			}
		}
		
			// Move on to the next vertex color.
		private void NextColorIndex () {
			m_ColorIndex = m_ColorIndex + 1;
			if (m_ColorIndex > 2) {
				m_ColorIndex = 0;
			}
		}
		
		private Color CurrentColor {
			get {
				Color l_Color;
				if (m_ColorIndex == 0) {
					l_Color = Color.red;
				} else if (m_ColorIndex == 1) {
					l_Color = Color.green;
				} else {
					l_Color = Color.blue;
				}
				return (l_Color);
			}
		}
		
		private void Start () {
			
			if (Edition.IsDecalSystemFree) {
				Debug.Log ("This demo only works with Decal System Pro.");
				enabled = false;
			} else {
				
					// Instantiate the prefab and get its decals instance.
				m_DecalsInstance = Instantiate (m_DecalsPrefab) as DS_SkinnedDecals;
				m_DecalsInstance.UseVertexColors = true;
				
				if (m_DecalsInstance == null) {
					Debug.LogError ("The decals prefab does not contain a DS_SkinnedDecals instance!");
				} else {
					
						// Create the decals mesh and the cutter that are needed to compute the projections.
						// We also cache the world to decals matrix.
					m_DecalsMesh = new SkinnedDecalsMesh (m_DecalsInstance);
					m_DecalsMeshCutter = new SkinnedDecalsMeshCutter ();
				}
			}
		}
		
		private void Update () {
			
			if (Input.GetKeyDown (KeyCode.C)) {
				
					// Remove all projectors.
				while (m_DecalProjectors.Count > 0) {
					m_DecalsMesh.ClearAll ();
					m_DecalProjectors.Clear ();
					
						// Clearing of the decals mesh means we need to initialize it again.
					m_DecalsMesh.Initialize (m_DecalsInstance);
				}
				m_DecalsInstance.UpdateDecalsMeshes (m_DecalsMesh);
			}
			
			if (Input.GetButtonDown ("Fire1")) {
				Ray l_Ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
				RaycastHit l_RaycastHit;
				if (Physics.Raycast (l_Ray, out l_RaycastHit, Mathf.Infinity)) {
					
						// Collider hit.
						
						// Make sure there are not too many projectors.
					if (m_DecalProjectors.Count >= m_MaximumNumberOfProjectors) {
						
							// If there are more than the maximum number of projectors, we delete
							// the oldest one.
						SkinnedDecalProjector l_DecalProjector = m_DecalProjectors [0];
						m_DecalProjectors.RemoveAt (0);
						m_DecalsMesh.RemoveProjector (l_DecalProjector);
					}
					
						// Calculate the position and rotation for the new decal projector.
					Vector3 l_ProjectorPosition = l_RaycastHit.point - (m_DecalProjectorOffset * l_Ray.direction.normalized);
					Quaternion l_ProjectorRotation = ProjectorRotationUtility.ProjectorRotation (Camera.main.transform.forward, Vector3.up);
					
						// Randomize the rotation.
					Quaternion l_RandomRotation = Quaternion.Euler (0.0f, Random.Range (0.0f, 360.0f), 0.0f);
					l_ProjectorRotation = l_ProjectorRotation * l_RandomRotation;

						// We hit a collider. Next we have to find the mesh that belongs to the collider.
						// That step depends on how you set up your mesh filters and collider relative to
						// each other in the game objects. It is important to have a consistent way in order
						// to have a simpler implementation.
					
					SkinnedMeshRenderer l_SkinnedMeshRenderer = l_RaycastHit.collider.GetComponent <SkinnedMeshRenderer> ();

					if (l_SkinnedMeshRenderer != null) {
						Mesh l_Mesh = null;
						l_Mesh = l_SkinnedMeshRenderer.sharedMesh;
						
						if (l_Mesh != null) {
							
								// Create the decal projector.
							SkinnedDecalProjector l_DecalProjector = new SkinnedDecalProjector (l_ProjectorPosition, l_ProjectorRotation, m_DecalProjectorScale, m_CullingAngle, m_MeshOffset, m_UVRectangleIndex, m_UVRectangleIndex, CurrentColor, 0.0f);
							
								// Add the projector to our list and the decals mesh, such that both are
								// synchronized. All the mesh data that is now added to the decals mesh
								// will belong to this projector.
							m_DecalProjectors.Add (l_DecalProjector);
							m_DecalsMesh.AddProjector (l_DecalProjector);
							
								// Get the required matrices.
							Matrix4x4 l_WorldToMeshMatrix = l_RaycastHit.collider.renderer.transform.worldToLocalMatrix;
							Matrix4x4 l_MeshToWorldMatrix = l_RaycastHit.collider.renderer.transform.localToWorldMatrix;
							
								// Add the mesh data to the decals mesh, cut and offset it.
							m_DecalsMesh.Add (l_Mesh, l_SkinnedMeshRenderer.bones, l_SkinnedMeshRenderer.quality, l_WorldToMeshMatrix, l_MeshToWorldMatrix);						
							m_DecalsMeshCutter.CutDecalsPlanes (m_DecalsMesh);
							m_DecalsMesh.OffsetActiveProjectorVertices ();
							
								// The changes are only present in the decals mesh at the moment. We have
								// to pass them to the decals instance to visualize them.
							m_DecalsInstance.UpdateDecalsMeshes (m_DecalsMesh);
							
								// For the next hit, use a new uv rectangle. Usually, you would select the uv rectangle
								// based on the surface you have hit.
							NextUVRectangleIndex ();
							NextColorIndex ();
						}
					}
				}
			}
		}
	}
}
