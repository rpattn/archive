using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
	
	public static World currentWorld;
	
	public int chunkWidth = 20, chunkHeight = 128, seed = 0;
	public float viewRange = 30;

	public Transform viewer;
	public Dictionary<Vector3, Chunk> currentChunkDict = new Dictionary<Vector3, Chunk>();
	
	public Chunk chunkFab;

	public Vector3 position;

	float delay = 1f;
	float currentTime = 100f;
	bool start = true;
	
	// Use this for initialization
	void Awake () {
		Cursor.visible = false;
		currentWorld = this;
		if (seed == 0)
			seed = Random.Range(0, int.MaxValue);
	}

	void Update() {
		if (currentTime > delay) {
			StartCoroutine (UpdateChunks ());
			start = false;
			currentTime = 0f;
		} else {
			if (start) {
				currentTime += Time.deltaTime;
			}
		}
	}
	
	// Update is called once per frame
	IEnumerator UpdateChunks () {
		
		
		for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x+= chunkWidth)
		{
			for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z+= chunkWidth)
			{
				int currentChunkCoordX = Mathf.RoundToInt ((viewer.position.x + x) / chunkWidth);
				int currentChunkCoordZ = Mathf.RoundToInt ((viewer.position.z + z) / chunkWidth);
				position = new Vector3 (currentChunkCoordX * chunkWidth, 0, currentChunkCoordZ * chunkWidth);
				if (!(currentChunkDict.ContainsKey (position))) {
					/*Vector3 pos = new Vector3 (x, 0, z);
					pos.x = Mathf.Floor (pos.x / (float)chunkWidth) * chunkWidth;
					pos.z = Mathf.Floor (pos.z / (float)chunkWidth) * chunkWidth;
					print ("here");
					//Chunk chunk = Chunk.FindChunk (pos);
					//if (chunk != null)
					//	continue;*/
					Chunk chunk = (Chunk)Instantiate (chunkFab, position, Quaternion.identity);
					currentChunkDict.Add (position, chunk);
				}
				
			}
			yield return null;
		}

		start = true;
		yield return 0;

	}
}


