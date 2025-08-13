using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public const float maxViewDst = 100;
	public Transform viewer;

	public static Vector2 viewPos;
	public int chunkSize = 10;
	public Material mat;
	[Range(0,20)]
	public float scale;
	[Range(0,20)]
	public float amplitude;
	int chunksVisibleInViewDst;

	float delay = 0.5f;
	float timeSinceLastCall;

	Dictionary<Vector2, TerrainChunk> currentChunkDict = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> chunksCurrentlyVisible = new List<TerrainChunk>();

	public GameObject chunkObject;

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
					TerrainChunk chunkClass = chunk.GetComponent<TerrainChunk> ();
					chunkClass.GenerateTerrainChunk (viewedChunkCoord, chunkSize, transform, mat, scale, amplitude, this);
					currentChunkDict.Add (viewedChunkCoord, chunkClass);
				}

			}
		}
			
		timeSinceLastCall = 0f;
	}
}


public static class BlockInfo {

	public static Vector3 One(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (0 + pos.x, 1 + pos.y, 0 + pos.z), scale); //1
	}
	public static Vector3 Two(Vector3 pos, Vector3 scale) {
		return Vector3.Scale(new Vector3 (-1 + pos.x, 1 + pos.y, 0 + pos.z), scale); //2
	}
	public static Vector3 Three(Vector3 pos, Vector3 scale) {
		return Vector3.Scale(new Vector3 (-1 + pos.x, 1 + pos.y, 1 + pos.z), scale); //3
	}
	public static Vector3 Four(Vector3 pos, Vector3 scale) {
		return Vector3.Scale(new Vector3 (0 + pos.x, 1 + pos.y, 1 + pos.z), scale); //4
	}
	public static Vector3 Five(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (0 + pos.x, 0 + pos.y, 0 + pos.z), scale); //5
	}
	public static Vector3 Six(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (-1 + pos.x, 0 + pos.y, 0 + pos.z), scale); //6
	}
	public static Vector3 Seven(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (-1 + pos.x, 0 + pos.y, 1 + pos.z), scale); //7
	}
	public static Vector3 Eight(Vector3 pos, Vector3 scale) {
		return Vector3.Scale (new Vector3 (0 + pos.x, 0 + pos.y, 1 + pos.z), scale); //8
	}
}
