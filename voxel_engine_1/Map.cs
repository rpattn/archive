using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public static Map currentWorld;

	public int seed = 0;
	[Range(20, 120)]
	public int viewRange = 30, HeightRange = 20;
	public float scale, amplitude;

	public Chunk chunkFab;

	public float randomOffset;

	float delay = 1f;
	float timeSinceLastCall;

	public Dictionary<Vector3, Chunk> currentChunkDict = new Dictionary<Vector3, Chunk>();

	public Transform viewer;

	public delegate void TickUpdate();
	public static event TickUpdate Tick;

	public Dictionary<Vector3, byte> blocks = new Dictionary<Vector3, byte> ();

	// Use this for initialization
	void Awake () {
		Cursor.visible = false;
		currentWorld = this;
		if (seed == 0)
			seed = Random.Range(0, int.MaxValue);
		randomOffset = Random.Range(0, 1000);
		timeSinceLastCall = delay + 1f;
		currentChunkDict = new Dictionary<Vector3, Chunk> ();
		//GenerateChunks ();
	}

	void Update() {
		if (timeSinceLastCall > delay) {
			StartCoroutine (UpdateChunks ());
			if (Tick != null) {
				Tick ();
			}
			timeSinceLastCall = 0f;
		} else {
			timeSinceLastCall += Time.deltaTime;
		}
	}

	IEnumerator UpdateChunks() {

		for (int x = -viewRange; x < viewRange; x += Chunk.width) {
			for (int z = -viewRange; z < viewRange; z += Chunk.width) {
				for (int y = -viewRange; y < viewRange; y += Chunk.height) {
					int currentChunkCoordX = Mathf.RoundToInt ((viewer.position.x + x) / Chunk.width);
					int currentChunkCoordY = Mathf.RoundToInt ((viewer.position.y + y) / Chunk.height);
					int currentChunkCoordZ = Mathf.RoundToInt ((viewer.position.z + z) / Chunk.width);
					Vector3 pos = new Vector3 (currentChunkCoordX * Chunk.width, currentChunkCoordY * Chunk.height, currentChunkCoordZ * Chunk.width);
					if (!(currentChunkDict.ContainsKey (pos))) {
						Chunk chunk = (Chunk)Instantiate (chunkFab, pos, Quaternion.identity);
						currentChunkDict.Add (pos, chunk);
						chunk.mapGen = this;
						chunk.offset = pos;
						chunk.seed = seed;
						chunk.transform.parent = transform;
					} else {
						currentChunkDict [pos].Enable ();
					}
				}
			}
			yield return new WaitForSeconds (0.1f);
		}

		yield return new WaitForSeconds (0.1f);
		timeSinceLastCall = 0f;
		yield return null;
	}


}
