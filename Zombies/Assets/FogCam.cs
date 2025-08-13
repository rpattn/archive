﻿using UnityEngine;
using System.Collections;

public class FogCam : MonoBehaviour {
	private bool revertFogState = false;
	void OnPreRender() {
		revertFogState = RenderSettings.fog;
		RenderSettings.fog = enabled;
	}
	void OnPostRender() {
		RenderSettings.fog = revertFogState;
	}
}
