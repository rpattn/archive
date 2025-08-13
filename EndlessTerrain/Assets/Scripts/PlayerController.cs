using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Rigidbody body;

	public float walkSpeed;
	public float runSpeed;

	float moveSpeed;

	Vector2 velocity;
	Vector3 translation;

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		moveSpeed = walkSpeed;
	}

	void Update () {

		MovePlayer ();
		Jump ();

	}

	void FixedUpdate() {

		transform.Translate (translation, transform);
	}

	void MovePlayer() {

		if (Input.GetKey (KeyCode.LeftShift)) {
			moveSpeed = runSpeed;
		} else {
			moveSpeed = walkSpeed;
		}

		Vector2 input = new Vector2(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		velocity = input * moveSpeed;
		translation = new Vector3 (velocity.x * Time.deltaTime, 0f, velocity.y * Time.deltaTime);
	}

	void Jump() {

		RaycastHit hit;
		if (Physics.Raycast (transform.position, Vector3.down, out hit, 1f)) {
			if (Input.GetButtonDown ("Jump")) {
				body.AddForce (Vector3.up * 300f);
				print ("Jumop");
			}
		}
	}
}

