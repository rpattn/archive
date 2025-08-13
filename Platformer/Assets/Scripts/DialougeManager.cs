using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialougeManager : MonoBehaviour {

	public Image textBG;
	public Text text;

	public Color textSolidColor;
	public float fadeTime;

	Color defaultColor;

	// Use this for initialization

	void Awake() {
		DontDestroyOnLoad (gameObject);
	}
	void Start () {
		defaultColor = textBG.color;
		text.color = defaultColor;
		CheckPointManager.OnCheckPointReached += OnCheckpoint;
	}
	
	void OnCheckpoint(string message) {
		if (textBG != null) {
			textBG.color = defaultColor;
		}
		text.color = defaultColor;
		text.text = message;
		StartCoroutine (ColourLerp ());
	}

	IEnumerator ColourLerp() {
		float time = 0;
		while (time < fadeTime) {
			textBG.color = Color.Lerp (textBG.color, textSolidColor, fadeTime * Time.deltaTime);
			text.color = Color.Lerp (text.color, Color.white, fadeTime * Time.deltaTime);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		yield return new WaitForSeconds (2);
		StartCoroutine (ColourLerpBack ());
	}

	IEnumerator ColourLerpBack() {
		float time = 0;
		while (time < fadeTime) {
			textBG.color = Color.Lerp (textBG.color, defaultColor, fadeTime * Time.deltaTime);
			text.color = Color.Lerp (text.color, defaultColor, fadeTime * Time.deltaTime);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}
}
