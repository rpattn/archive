using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class CheckPointManager : MonoBehaviour {

	public LayerMask playerLayer;

	public Checkpoint[] checkpoints;

	public GameObject playerObject;

	public delegate void OnPlayerRespawn ();
	public static event OnPlayerRespawn OnRespawn;

	public delegate void OnReachedCheckPoint (string message);
	public static event OnReachedCheckPoint OnCheckPointReached;

	Checkpoint currentCheckpoint;

	void LateUpdate() {
		CheckForPlayer ();
		UIManager.OnRespawn += OnPlayerDeath;
	}

	void CheckForPlayer(){
		foreach (Checkpoint c in checkpoints) {
			if (Physics2D.OverlapArea (new Vector2(c.centre.x + c.width / 2, c.centre.y + c.height / 2), 
				new Vector2(c.centre.x - c.width / 2, c.centre.y - c.height / 2), playerLayer )) {
				if (currentCheckpoint.message != c.message) {
					currentCheckpoint = c;
					if (OnCheckPointReached != null) {
						OnCheckPointReached (c.message);
					}
				}
			}
		}
	}

	void OnPlayerDeath() {
		playerObject.transform.position = currentCheckpoint.centre;
		if (OnRespawn != null) {
			OnRespawn ();
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 0, 0, .5f);
		foreach (Checkpoint c in checkpoints) {
			Gizmos.DrawCube (c.centre, new Vector3(c.width, c.height, 0));
		}
	}
}

[System.Serializable]
public struct Checkpoint {

	public float width;
	public float height;
	public Vector3 centre;
	public string message;
}
