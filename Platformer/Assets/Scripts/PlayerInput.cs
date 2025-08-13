using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;

	bool playerAlive;

	float sensitivity;

	void Start () {
		player = GetComponent<Player> ();
		playerAlive = true;
		PlayerHealthManager.OnDeath += OnPlayerDeath;
		CheckPointManager.OnRespawn += OnRespawn;
		sensitivity = PlayerPrefs.GetFloat ("Sensitivity", 4);
	}

	void Update () {
		if (playerAlive) {
			Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			player.SetDirectionalInput (directionalInput);

			if (Input.GetKeyDown (KeyCode.Space)) {
				player.OnJumpInputDown ();
			}
			if (Input.GetKeyUp (KeyCode.Space)) {
				player.OnJumpInputUp ();
			}
		} else {
			player.SetDirectionalInput (Vector2.zero);
		}
	}

	public void setDirInput(Vector2 screenInput) {
		if (playerAlive) {
			Vector2 dir = new Vector2 (Mathf.Clamp (screenInput.x / (Screen.width / sensitivity), -1, 1), screenInput.y / (Screen.width / sensitivity));
			player.SetDirectionalInput (dir);
		}
	}

	public void screenJump() {
		if (playerAlive) {
			player.OnJumpInputDown ();
		}
	}

	public void jumpInputUp() {
		if (playerAlive) {
			player.OnJumpInputUp ();
		}
	}

	void OnPlayerDeath() {
		playerAlive = false;
	}

	void OnRespawn() {
		playerAlive = true;
	}
}
