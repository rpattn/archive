using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class CameraEffectsController : MonoBehaviour {

	public VignetteAndChromaticAberration vignette;
	public BloomOptimized bloom;

	public PlayerHealthManager playerHealthInstance;

	public bool applyEffects;

	float startIntensity;
	float startChromAberat;
	float startVignette;

	float currentIntensity;
	float targetIntensity;

	float currentCA;
	float targetCA;

	void Start() {
		startIntensity = bloom.intensity;
		startChromAberat = vignette.chromaticAberration;
		int current = PlayerPrefs.GetInt ("Visual Effects", 1);
		this.applyEffects = (current == 1) ? false : true;
		PlayerHealthManager.OnDamaged += OnTakeHit;
	}

	void OnTakeHit() {
		targetIntensity = 2;
		targetCA = 4;
	}

	public void ToggleEffects() {
		int current = PlayerPrefs.GetInt ("Visual Effects", 1);
		this.applyEffects = (current == 1) ? false : true;
	}

	void Update() {
		if (applyEffects) {
			bloom.enabled = true;
			vignette.enabled = true;
			if (!playerHealthInstance.StillDamaged ()) {
				targetIntensity = startIntensity;
				targetCA = startChromAberat;
			}
			bloom.intensity = Mathf.MoveTowards (currentIntensity, targetIntensity, 5f * Time.deltaTime);
			vignette.chromaticAberration = Mathf.MoveTowards (currentCA, targetCA, 5f * Time.deltaTime);
			currentIntensity = bloom.intensity;
			currentCA = vignette.chromaticAberration;
		} else {
			bloom.enabled = false;
			vignette.enabled = false;
		}
	}
}
