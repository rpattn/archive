using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public Transform muzzle;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;

	public LayerMask enemy;

	Animator anim;

	float nextShotTime;

	public void Start() {
		anim = FindObjectOfType<Animator> ();
	}

	public void Shoot() {

		if (Time.time > nextShotTime) {
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newProjectile = Instantiate (projectile, muzzle.position, muzzle.rotation) as Projectile;
			newProjectile.SetSpeed (muzzleVelocity);
			//newProjectile.transform.parent = transform;
			anim.SetBool ("Shoot", true);

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, 1000f, enemy)) {
				hit.transform.gameObject.SendMessage ("Damage", hit.transform.position);
			}

			Invoke ("SetFalse", 0.1f);
		}
	}

	void SetFalse() {
		anim.SetBool("Shoot", false);
	}
}
