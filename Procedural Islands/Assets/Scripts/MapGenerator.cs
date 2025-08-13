using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	const int mapChunkSize = 50;
	[Range(0,48)]
	public float noiseScale;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public AnimationCurve cutOffHeightCurve;

	public bool autoUpdate;

	public bool island;

	public TerrainType[] regions;

	public MeshFilter meshRenderer;

	public void GenerateMap() {
		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
		float[,] falloff = FallOff.GenerateFalloffMap (mapChunkSize);

		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				noiseMap [x, y] = Mathf.Clamp01(noiseMap [x, y] - falloff [x, y]);
			}
		}

		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						colourMap [y * mapChunkSize + x] = regions [i].colour;
						break;
					}
				}
			}
		}

		if (island) {
			meshRenderer.mesh = MeshGenerator.GenerateTerrainMesh (noiseMap, meshHeightMultiplier, meshHeightCurve, cutOffHeightCurve).CreateMesh ();
		}
			
	}

	void OnValidate() {
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
	}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}