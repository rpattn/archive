using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIO : MonoBehaviour {

	public float maxInteractDistance = 8f;

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButton (0) || Input.GetMouseButtonDown (1) || Input.GetMouseButtonDown (2)) {
			Ray ray = GetComponent<Camera> ().ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.5f));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, maxInteractDistance)) {
				Chunk chunk = hit.transform.GetComponent<Chunk> ();
				if (chunk == null) {
					return;
				}
				if (Input.GetMouseButton (0)) {
					Vector3 p = hit.point;
					p -= hit.normal / 2f;
					//int x = (int)Math.Round(p.x, MidpointRounding.AwayFromZero);
					//int y = (int)Math.Round(p.y, MidpointRounding.AwayFromZero);
					//int z = (int)Math.Round(p.z, MidpointRounding.AwayFromZero);
					//Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube), new Vector3(x,y,z), Quaternion.identity);
					chunk.SetBlockFalse (false, p);
					Debug.DrawLine (transform.position, p, Color.red, 20f);
				} 
				if (Input.GetMouseButtonDown (1)) {
					Vector3 p = hit.point;
					p += hit.normal / 2f;
					chunk.SetBlock (true, p);
					Debug.DrawLine (transform.position, p, Color.red, 20f);
				} 
			}
		}

	}
}

