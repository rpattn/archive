using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class OptionsScreenManager : MonoBehaviour {

	public Slider sensitivitySlider;
	public Slider useVisualEffects;

	float sensitivity;
	int visualEffects;

	// Use this for initialization
	void Start () {
		sensitivity = PlayerPrefs.GetFloat ("Sensitivity", 4);
		visualEffects = PlayerPrefs.GetInt ("Visual Effects", 1);
		SetSensitivity ();
		SetVisualEffects ();
	}

	public void SensitivityChange() {
		sensitivity = sensitivitySlider.value;
		PlayerPrefs.SetFloat ("Sensitivity", sensitivity);
	}

	public void VisualEffectsChange() {
		visualEffects = (int) useVisualEffects.value;
		PlayerPrefs.SetInt ("Visual Effects", visualEffects);
	}

	void SetSensitivity() {
		sensitivitySlider.value = sensitivity;
	}

	void SetVisualEffects() {
		useVisualEffects.value = visualEffects;
	}

	public void MainMenu() {
		SceneManager.LoadScene ("Menu");
	}
}
