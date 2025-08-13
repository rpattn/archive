using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	float speed = 10;
	public float life;
	float currentLifeTime;

	public void SetSpeed(float newSpeed) {
		speed = newSpeed;
		currentLifeTime = 0;
	}
	
	void Update () {
		if (currentLifeTime < life) {
			transform.Translate (Vector3.up * Time.deltaTime * speed);
			currentLifeTime += Time.deltaTime;
		} else {
			Destroy (gameObject);
		}
	}
}
