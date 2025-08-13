using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
	public CharacterController body;
	public Rigidbody rigidBody;

	void Start () {
		body.enabled = false;
		rigidBody.useGravity = false;
		MapGenerator.onMapReady += MapReady;
	}
	
	void MapReady() {
		body.enabled = true;
		rigidBody.useGravity = true;
	}
}
