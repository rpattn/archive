using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InfNoise {

	public static byte[,,] Generate(float scale, float amplitude, Vector3 offset, int seed) {

		byte[,,] map = new byte[Chunk.width, Chunk.height, Chunk.width];
		//Random.InitState (seed);

		for (int y = 0; y < Chunk.height; y++) {
			for (int x = 0; x < Chunk.width; x++) {
				for (int z = 0; z < Chunk.width; z++) {
					if (y > Chunk.groundOffset) {
						float value = Mathf.PerlinNoise ((x + offset.x) / scale, (z + offset.z) / scale) * amplitude;
						if (value > y + offset.y) {
							map [x, y, z] = 1;
							//do simplex 3D here
						} else {
							map [x, y, z] = 0;
						}
					} else {
						map [x, y, z] = 1;
					}
				}
			}
		}

		return map;
	}

	public static byte GetTheoreticalByte (float scale, float amplitude, Vector3 originalOffset, Vector3 coord) {
		if (coord.y > Chunk.groundOffset) {
			float value = Mathf.PerlinNoise ((coord.x + originalOffset.x) / scale, (coord.z + originalOffset.z) / scale) * amplitude;
			if (value > coord.y + originalOffset.y) {
				return 1;
				//do simplex 3D here
			} else {
				return 0;
			}
		} else {
			return 1;
		}
	}
}
