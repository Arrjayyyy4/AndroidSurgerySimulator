//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012-2014 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.DecalSystem;

namespace Edelweiss.DecalSystem.Example {

	public class BulletLightmapExample : MonoBehaviour {
		
			// The prefab with the ready to use DS_Decals. The material and uv rectangles are set up.
		[SerializeField] private DS_Decals m_DecalsPrefab = null;
			
			// All the decals that were created at runtime and the maximum amount of decals.
		private List <DS_Decals> m_DecalsInstances = new List <DS_Decals> ();
		[SerializeField] private int m_MaximumNumberOfDecals = 50;
		
			// We need a decals mesh and a cutter to create the decal projectors at runtime.
		private DecalsMesh m_DecalsMesh;
		private DecalsMeshCutter m_DecalsMeshCutter;
		
			// We can't place the projector exactly at the place where the raycast hits a collider. Instead,
			// it needs to be moved back, such that the projection looks believable. This value indicates how
			// many units the projector needs to be moved back.
		[SerializeField] private float m_DecalProjectorOffset = 0.5f;

			// The size, culling angle and mesh offset for the decal projectors for the bullets we are about to
			// create at runtime.
		[SerializeField] private Vector3 m_DecalProjectorScale = new Vector3 (0.4f, 2.0f, 0.4f);
		[SerializeField] private float m_CullingAngle = 80.0f;
		[SerializeField] private float m_MeshOffset = 0.0f;
		
			// We iterate through all the defined uv rectangles. This one inicates which index we are using at
			// the moment.
			//
			// REMARK:
			// We use 7 as the first index because the previous ones are too dark and the lightmapping
			// is barely visible for them.
		private int m_UVRectangleIndex = 7;
		
			// Move on to the next uv rectangle index. We call this method after each projection. In practice
			// you certainly don't do that, but pick an appropriate one depending on the surface that was hit.
		private void NextUVRectangleIndex (DS_Decals a_DecalsInstance) {
			m_UVRectangleIndex = m_UVRectangleIndex + 1;
			if (m_UVRectangleIndex >= a_DecalsInstance.CurrentUvRectangles.Length) {
				m_UVRectangleIndex = 7;
			}
		}

		private void Start () {
			
				// Create the decals mesh and the cutter that are needed to compute the projections.
			m_DecalsMesh = new DecalsMesh ();
			m_DecalsMeshCutter = new DecalsMeshCutter ();
		}
		
		private void Update () {

			if (Input.GetKeyDown (KeyCode.C)) {
				
					// Remove all decals instances.
				foreach (DS_Decals l_DecalsInstance in m_DecalsInstances) {
					Destroy (l_DecalsInstance.gameObject);
				}
				m_DecalsInstances.Clear ();
			}

			if (Input.GetButtonDown ("Fire1")) {
				Ray l_Ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
				RaycastHit l_RaycastHit;
				
					// Terrains have no uv2, so we just skip them.
				if
					(Physics.Raycast (l_Ray, out l_RaycastHit, Mathf.Infinity) &&
					 l_RaycastHit.collider as TerrainCollider == null)
				{
					
						// Collider hit.
					
						// Make sure there are not too many decals instances.
					if (m_DecalsInstances.Count >= m_MaximumNumberOfDecals) {
						DS_Decals l_FirstDecalsInstance = m_DecalsInstances [0];
						Destroy (l_FirstDecalsInstance);
						m_DecalsInstances.RemoveAt (0);
					}
					
					
						// Instantiate the prefab and get its decals instance.
					DS_Decals l_DecalsInstance = Instantiate (m_DecalsPrefab) as DS_Decals;
					
						// Reuse the decals mesh, but be sure to initialize it always for the current
						// decals instance.
					m_DecalsMesh.Initialize (l_DecalsInstance);

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
					
					MeshCollider l_MeshCollider = l_RaycastHit.collider.GetComponent <MeshCollider> ();
					MeshFilter l_MeshFilter = l_RaycastHit.collider.GetComponent <MeshFilter> ();
					MeshRenderer l_MeshRenderer = l_RaycastHit.collider.GetComponent <MeshRenderer> ();
					
					if (l_MeshCollider != null || l_MeshFilter != null) {
						Mesh l_Mesh = null;
						if (l_MeshCollider != null) {
							
								// Mesh collider was hit. Just use the mesh data from that one.
							l_Mesh = l_MeshCollider.sharedMesh;
						} else if (l_MeshFilter != null) {
							
								// Otherwise take the data from the shared mesh.
							l_Mesh = l_MeshFilter.sharedMesh;
						}
						
						if (l_Mesh != null) {
							
								// Create the decal projector.
							DecalProjector l_DecalProjector = new DecalProjector (l_ProjectorPosition, l_ProjectorRotation, m_DecalProjectorScale, m_CullingAngle, m_MeshOffset, m_UVRectangleIndex, m_UVRectangleIndex);

								// Add the decals instance to our list and the projector
								// to the decals mesh.
								// All the mesh data that is now added to the decals mesh
								// will belong to this projector.
							m_DecalsInstances.Add (l_DecalsInstance);
							m_DecalsMesh.AddProjector (l_DecalProjector);
							
								// Get the required matrices.
							Matrix4x4 l_WorldToMeshMatrix = l_RaycastHit.collider.renderer.transform.worldToLocalMatrix;
							Matrix4x4 l_MeshToWorldMatrix = l_RaycastHit.collider.renderer.transform.localToWorldMatrix;
							
								// Add the mesh data to the decals mesh, cut and offset it.
							m_DecalsMesh.Add (l_Mesh, l_WorldToMeshMatrix, l_MeshToWorldMatrix);						
							m_DecalsMeshCutter.CutDecalsPlanes (m_DecalsMesh);
							m_DecalsMesh.OffsetActiveProjectorVertices ();
							
								// The changes are only present in the decals mesh at the moment. We have
								// to pass them to the decals instance to visualize them.
							l_DecalsInstance.UpdateDecalsMeshes (m_DecalsMesh);
							
								// Lightmapping
							l_DecalsInstance.DecalsMeshRenderers [0].MeshRenderer.lightmapIndex = l_MeshRenderer.lightmapIndex;
							l_DecalsInstance.DecalsMeshRenderers [0].MeshRenderer.lightmapTilingOffset = l_MeshRenderer.lightmapTilingOffset;
							
								// For the next hit, use a new uv rectangle. Usually, you would select the uv rectangle
								// based on the surface you have hit.
							NextUVRectangleIndex (l_DecalsInstance);
						}
					}
				}
			}
		}
	}
}