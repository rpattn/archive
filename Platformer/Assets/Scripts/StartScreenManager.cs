using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartScreenManager : MonoBehaviour {

	public RawImage fade;

	public Color fadeColour;
	public float fadeTime;

	public void MainGame() {
		StartCoroutine (Fade ());
	}

	public void Options() {
		SceneManager.LoadScene ("Options");
	}

	public void Quit() {
		Application.Quit ();
	}

	IEnumerator Fade() {
		float time = 0;
		while (time < 3) {
			fade.color = Color.Lerp (fade.color, fadeColour, fadeTime * Time.deltaTime);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		} 
		SceneManager.LoadScene ("Scene");
	}
}
