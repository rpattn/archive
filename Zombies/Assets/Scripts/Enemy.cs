using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	Transform player;
	public GameObject particles;
	Vector3 hit;
	NavMeshAgent pathfinder;

	public float health;
	public float damage;
	float currentHealth;

	public Slider healthBar;

	public float moveSpeed;

	Spawner spawner;

	bool hasTarget;
	float myCollisionRadius;
	float targetCollisionRadius;
	float attackDistanceThreshold = .5f;

	void Start () {
		pathfinder = GetComponent<NavMeshAgent> ();
		player = FindObjectOfType<FirstPersonController> ().transform;
		currentHealth = health;
		healthBar.maxValue = health;
		healthBar.value = currentHealth;
		spawner = FindObjectOfType<Spawner> ();

		if (player != null) {

			hasTarget = true;
			myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
			targetCollisionRadius = player.GetComponent<CharacterController> ().radius;
			StartCoroutine (UpdatePath ());
		}
	}

	void Update () {
		if (player != null) {
			if (Vector3.Distance (player.position, transform.position) > 1.2f) {
				transform.position = Vector3.MoveTowards (transform.position, player.position, moveSpeed / 100);
				transform.LookAt (player);
			}
		} else {
			player = FindObjectOfType<FirstPersonController> ().transform;
		}

		if (healthBar.value == health) {
			healthBar.gameObject.SetActive (false);
		}
	}

	public void Damage(Vector3 pos) {
		//Invoke ("Destroy", 0.05f);
		//hit = pos;
		if (currentHealth - damage > 0) {
			healthBar.gameObject.SetActive (true);
			currentHealth -= damage;
			healthBar.value = currentHealth;
		} else {
			Invoke ("Destroy", 0.05f);
			hit = pos;
		}
	}

	public void Destroy() {
		Instantiate (particles, hit, Quaternion.identity);
		spawner.OnEnemyDeath ();
		Destroy (gameObject);
	}

	IEnumerator UpdatePath() {
		float refreshRate = .25f;

		while (hasTarget) {
			Vector3 dirToTarget = (player.position - transform.position).normalized;
			Vector3 targetPosition = player.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
			pathfinder.SetDestination (targetPosition);
			yield return new WaitForSeconds(refreshRate);
		}
	}
}
