using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class HealthManager : MonoBehaviour {

	public float maxHealth;
	public float minHealth;

	public bool hasHealthBar;
	public Slider healthBar;

	public delegate void OnDeathEvent ();
	public static event OnDeathEvent OnDeath;

	public delegate void OnDamageEvent ();
	public static event OnDamageEvent OnDamaged;

	public delegate void OnGainHealthEvent ();
	public static event OnGainHealthEvent OnGainedHealth;

	float health;
	float oldHealth;
	public bool beingDamaged;

	public void TakeHealth(float health) {
		if (this.health - health > minHealth) {
			this.health -= health;
			beingDamaged = true;
			if (OnDamaged != null) {
				OnDamaged ();
			}
		} else {
			this.health = minHealth;
			beingDamaged = true;
			if (OnDeath != null) {
				OnDeath ();
			}
		}
		UpdateHealth ();
	}

	public void AddHealth (float health) {
		if (this.health + health <= maxHealth) {
			this.health += health;
			if (OnGainedHealth != null) {
				OnGainedHealth ();
			}
		} else {
			this.health = maxHealth;
		}
		UpdateHealth ();
	}

	void UpdateHealth() {
		healthBar.value = this.health;
	}

	public void Kill() {
		health = minHealth;
		if (OnDeath != null) {
			OnDeath ();
		}
	}

	public void SetUp(float health) {
		this.health = health;
		this.oldHealth = health;
		beingDamaged = false;

		if (hasHealthBar && healthBar != null) {
			healthBar.maxValue = this.maxHealth;
			healthBar.minValue = this.minHealth;
			healthBar.value = health;
		}

		StartCoroutine (CheckForHealthChange ());
	}

	public void Respawn() {
		this.health = maxHealth;
		UpdateHealth ();
	}

	public float GetHealth () {
		return this.health;
	}

	public bool StillDamaged() {
		return beingDamaged;
	}

	public bool isAlive() {
		return (health > minHealth) ? true : false;
	}

	IEnumerator CheckForHealthChange() {

		while (true) {
			if (health != oldHealth) {
				beingDamaged = true;
			} else {
				beingDamaged = false;
			}

			oldHealth = health;

			yield return new WaitForSeconds (0.5f);
		}
	}
}
