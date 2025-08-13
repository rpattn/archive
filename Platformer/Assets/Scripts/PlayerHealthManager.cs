using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthManager : HealthManager {

	public float _health;

	public void Start() {
		base.SetUp (_health);
		CheckPointManager.OnRespawn += base.Respawn;
		Player.OnBorder += base.Kill;
	}
		
}
