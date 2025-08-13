using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Image gameOverImage;
	public GameObject gameOverText;

	public Color gameOverColour;
	public float fadeTime;

	public delegate void OnRespawnEvent ();
	public static event OnRespawnEvent OnRespawn;

	public CameraEffectsController effects;

	Color defaultColor;

	void Start() {
		defaultColor = gameOverImage.color;
		gameOverImage.color = gameOverColour;
		PlayerHealthManager.OnDeath += OnPlayerDeath;
		gameOverText.SetActive (false);
		StartCoroutine (ColourLerpBack ());
	}

	void OnPlayerDeath() {
		gameOverText.SetActive (true);
		StartCoroutine (ColourLerp ());
	}

	public void Quit() {
		SceneManager.LoadScene ("Menu");
	}

	IEnumerator ColourLerp() {
		float time = 0;
		while (time < 5) {
			gameOverImage.color = Color.Lerp (gameOverImage.color, gameOverColour, fadeTime * Time.deltaTime);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		if (OnRespawn != null) {
			OnRespawn ();
		}
		StartCoroutine (ColourLerpBack ());
	}

	IEnumerator ColourLerpBack() {
		float time = 0;
		gameOverText.SetActive (false);
		while (time < 3) {
			gameOverImage.color = Color.Lerp (gameOverImage.color, defaultColor, fadeTime * Time.deltaTime);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}
}
