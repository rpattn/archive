using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseV2 {

	public static byte GetByte(float x, float y, float z, float scale, float amplitude, float offset) {
		if (y > 60) {
			float value = Mathf.PerlinNoise ((x + offset) / scale, (z + offset) / scale) * amplitude;
			if (value + 30 > y) {
				return 1;
			} else {
				return 0;
			}
		} else {
			return 1;
		}
	}
}
