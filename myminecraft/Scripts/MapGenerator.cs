using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public const float maxViewDst = 80;
	public Transform viewer;

	public static Vector2 viewPos;
	public int chunkSize = 10;
	public Material mat;

	[Range(0,20)]
	public float scale;
	[Range(0,20)]
	public float amplitude;
	public Biome[] biomes;

	int chunksVisibleInViewDst;

	float delay = 0.5f;
	float timeSinceLastCall;

	public Dictionary<Vector2, Chunk> currentChunkDict = new Dictionary<Vector2, Chunk>();
	List<Chunk> chunksCurrentlyVisible = new List<Chunk>();

	public GameObject chunkObject;
	[Range(1,1000)]
	public int seed;

	public delegate void OnMapReady ();
	public static event OnMapReady onMapReady;

	//int[,] biomeMap;

	void Start() {
		SetUp ();  //for editor
	}

	void Update() {
		timeSinceLastCall += Time.deltaTime;
		if (timeSinceLastCall > delay) {
			//timeSinceLastCall = 0f;
			viewPos = new Vector2 (viewer.position.x, viewer.position.z);
			UpdateVisibleChunks ();
		}
	}

	public void SetUp() {
		timeSinceLastCall = delay + 1f;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
		StartCoroutine (WaitForMap ());
		//biomeMap = Noise.GenerateChunkMap (seed);
	}

	public void UpdateVisibleChunks() {

		int chunkPosX = Mathf.RoundToInt (viewPos.x / chunkSize);
		int chunkPosY = Mathf.RoundToInt (viewPos.y / chunkSize);

		for (int i = 0; i < chunksCurrentlyVisible.Count; i++) {
			chunksCurrentlyVisible [i].SetVisible (false);
		}
		chunksCurrentlyVisible.Clear ();

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (chunkPosX + xOffset, chunkPosY + yOffset);

				if (currentChunkDict.ContainsKey (viewedChunkCoord)) {
					currentChunkDict [viewedChunkCoord].UpdateTerrainChunk (viewPos, maxViewDst);
					if (currentChunkDict [viewedChunkCoord].IsVisible ()) {
						chunksCurrentlyVisible.Add (currentChunkDict [viewedChunkCoord]);
					}
				} else {
					GameObject chunk = Instantiate (chunkObject, transform) as GameObject;
					Chunk chunkClass = chunk.GetComponent<Chunk> ();
					//DetermineBiomeFromCoord
					chunkClass.GenerateTerrainChunk (viewedChunkCoord, chunkSize, transform, mat, scale, amplitude, this);
					currentChunkDict.Add (viewedChunkCoord, chunkClass);
				}

			}
		}

		timeSinceLastCall = 0f;
	}

	IEnumerator WaitForMap() {
		yield return new WaitForSeconds (2);
		if (onMapReady != null) {
			onMapReady ();
		}
		yield return null;
	}
}

[System.Serializable]
public struct Biome {

	public string name;
	public float heightModifier;
	public float scaleModifier;
	public int heightOffset;
	public float treeDenity;
}
