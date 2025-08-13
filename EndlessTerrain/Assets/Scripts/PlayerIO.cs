using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIO : MonoBehaviour {

	public float maxInteractDistance = 8f;
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1) || Input.GetMouseButtonDown (2)) {
			Ray ray = GetComponent<Camera> ().ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.5f));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, maxInteractDistance)) {
				TerrainChunk chunk = hit.transform.GetComponent<TerrainChunk> ();
				if (chunk == null) {
					return;
				}
				if (Input.GetMouseButtonDown (0)) {
					Vector3 p = hit.point;
					p -= hit.normal / 4;
					chunk.SetBlock (0, p);
				} 
			}
		}
		
	}
}
