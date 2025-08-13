using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//change names
public static class Noise {

	public static int[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, float amplitude, float offsetX, float offsetY) {
		int[,] noiseMap = new int[mapWidth,mapHeight];

		if (scale <= 0) {
			scale = 0.0001f;
		}

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				float sampleX = (x + offsetX) / scale;
				float sampleY = (y + offsetY) / scale;

				float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * amplitude;
				noiseMap [x, y] = Mathf.RoundToInt(perlinValue);
			}
		}

		return noiseMap;
	}

	public static Block[,,] GenerateBlockMap(int[,] noiseMap) {
		int width = noiseMap.GetLength (0);
		Block[,,] blockMap = new Block[width, width, width];

		for (int y = 0; y < width; y++) {
			for (int x = 0; x < width; x++) {
				for (int z = 0; z < width; z++) {
					if (y < 7 && y > 0) {  //fix
						blockMap [x, y, z] = new GrassBlock (noiseMap [x, z] + y, true);
					} else if (y == 0) {
						blockMap [x, y, z] = new ClearBlock (noiseMap [x, z] + y, false);
					}else {
						blockMap [x, y, z] = new ClearBlock (noiseMap [x, z] + y, false);
					}
				}
			}
		}
		//blockMap [0, 0, 0] = new GrassBlock (0, true);
		return blockMap;
	}

}