using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
//change names
public static class Noise {

	static float octaves = 2;
	static float persistance = 0.5f;
	static float lacunarity = 0.5f;

	/*public static int[,] GenerateChunkMap (int seed) {
		int[,] noiseMap = new int[100, 100];
		Random.InitState (seed);
		float randomVal = Random.value * 100f;
		float offsetX = randomVal + 550;
		float offsetY = randomVal + 550;

		for (int y = 0; y < 100; y++) {
			for (int x = 0; x < 100; x++) {
				float sampleX = (x + offsetX) / 10f * 1;
				float sampleY = (y + offsetY) / 10f * 1;
				float perlinValue = Mathf.PerlinNoise (sampleX, sampleY);
				if (perlinValue < 0.5f) {
					noiseMap [x, y] = 0;
				} else {
					noiseMap [x, y] = 1;
				}
			}
		}

		return noiseMap;
	}*/

	public static Block[,,] GenerateBlockMap(int width, float scale, float amplitude, float offsetX, float offsetY, int seed, Biome[] biomeValues) {

		int[,] noiseMap = new int[width,width];
		int[,] biomeMap = new int[width, width];

		if (scale <= 0) {
			scale = 0.0001f;
		}

		//Random.InitState (seed);
		//float randomVal = Random.value * 100f;
		System.Random rng = new System.Random(seed);
		float randomVal = rng.Next(0,1000);
		offsetX += randomVal + 550;
		offsetY += randomVal + 550;

		int min = int.MaxValue;
		int max = int.MinValue;

		for (int y = 0; y < width; y++) {
			for (int x = 0; x < width; x++) {

				float amplitudeOct = 1;
				float frequency = 1;
				int height = 0;
				int biomeInt = 0;

				float sampleXb = (x + offsetX) / 150 * 1; //scale the map
				float sampleYb = (y + offsetY) / 150 * 1;

				float perlinValueb = Mathf.PerlinNoise (sampleXb, sampleYb) * 4;
				if (perlinValueb > 1.5f && perlinValueb < 2.7f) {
					perlinValueb += 0.3f; //plains bias;
				} else if (perlinValueb < 0.75f) {
					perlinValueb -= 0.3f; //mountain bias
				}
				biomeInt = Mathf.RoundToInt (perlinValueb);
				biomeMap [x, y] = biomeInt;

				float scaleModifier = biomeValues [biomeInt].scaleModifier;

				for (int i = 0; i < octaves + 1; i++) {
					float sampleX = (x + offsetX) / (scale * scaleModifier) * frequency;
					float sampleY = (y + offsetY) / (scale * scaleModifier) * frequency;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * amplitude * biomeValues[biomeInt].heightModifier;
					height += Mathf.RoundToInt (perlinValue * amplitudeOct);

					amplitudeOct *= persistance;
					frequency *= lacunarity;
				}

				height += biomeValues [biomeInt].heightOffset;

				if (height > max) {
					max = height;
				}
				if (height < min) {
					min = height;
				}
				noiseMap [x, y] = height;
			}
		}
		Thread.Sleep (50);
		int newSize = max + 50;  // + air above you
			
		Block[,,] blockMap = new Block[width, newSize, width];

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < width; z++) {
				int height = noiseMap [x, z] + 28;  // + ground bellow you
				int biome = biomeMap[x,z];
				for (int y = 0; y < newSize; y++) {
					if (y == 0) {
						blockMap [x, y, z] = new BedRock (y, true);
						continue;
					}
					if (y <= height) {              //TODO: Random viens
						if (biome == 2) {  //2 plains
							if (y > height - 1) {
								blockMap [x, y, z] = new GrassBlock (y, true);
							} else if (y > height - 3) {
								blockMap [x, y, z] = new DirtBlock (y, true);
							} else {
								blockMap [x, y, z] = new StoneBlock (y, true);
							}
						} else if (biome == 1) { //hilly/normal
							if (y > height - 1) {
								blockMap [x, y, z] = new GrassBlock (y, true);
							} else if (y > height - 3) {
								blockMap [x, y, z] = new DirtBlock (y, true);
							} else {
								blockMap [x, y, z] = new StoneBlock (y, true);
							}
						} else if (biome == 0) { //mountanous
							if ((rng.Next(0,100)/100f) < 0.1f) { //allow coal to be found here more
								blockMap [x, y, z] = new CoalBlock (y, true);
							} else {
								blockMap [x, y, z] = new StoneBlock (y, true);
							}

						} else {  //desert
							if (y > height - 4) {
								blockMap [x, y, z] = new SandBlock (y, true);
							} else {
								blockMap [x, y, z] = new StoneBlock (y, true);
							}
						}
						if (y < height - 5) {
							if ((rng.Next(0,100)/100f) < 0.1f) {
								float spawnOre = (rng.Next (0, 75) / 100f);
								if (spawnOre > 0.75f) {
									blockMap [x, y, z] = new CoalBlock (y, true);
								} else if (spawnOre > 0.45f) {
									blockMap [x, y, z] = new IronOre (y, true);
								} else if ((spawnOre < 0.25f) && y < height - 10) {
									blockMap [x, y, z] = new DiamondOre (y, true);
								}
							}
						}
					} else if (y == height + 1) { //air or trees
						if ((rng.Next(0,100)/100f) / biomeValues[biome].treeDenity < 0.025f) { //tree likelyhood
							if ((x > 3) && (x < width - 3) && (z > 3) && (z < width - 3)) {
								blockMap [x, y, z] = new WoodBlock (y, true);
								blockMap [x, y + 1, z] = new WoodBlock (y + 1, true);
								blockMap [x, y + 2, z] = new WoodBlock (y + 2, true);
								blockMap [x, y + 3, z] = new WoodBlock (y + 3, true);
								blockMap [x + 1, y + 3, z] = new LeafBlock (y + 3, true);
								blockMap [x - 1, y + 3, z] = new LeafBlock (y + 3, true);
								blockMap [x, y + 3, z + 1] = new LeafBlock (y + 3, true);
								blockMap [x, y + 3, z - 1] = new LeafBlock (y + 3, true);
								blockMap [x, y + 4, z] = new LeafBlock (y + 4, true);
							} else {
								blockMap [x, y, z] = new ClearBlock (y, false);
							}
						} else {
							blockMap [x, y, z] = new ClearBlock (y, false);
						}
					} else {
						if (blockMap [x, y, z] == null) {
							blockMap [x, y, z] = new ClearBlock (y, false);
						}
					}
				}
				//blockMap [x, y, z] = new GrassBlock (height + y, true);
			}
			Thread.Sleep (7);
		}
		Thread.Sleep (50);
		//blockMap [0, 0, 0] = new GrassBlock (0, true);
		return blockMap;
	}

}
