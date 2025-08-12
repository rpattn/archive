using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SimplexNoise;

public class Chunk : MonoBehaviour {

	public static int width = 16;
	public static int height = 128;
	public static int groundOffset = 48;

	public byte[,,] map;
	public Mesh visualMesh;
	public MeshRenderer meshRenderer;
	protected MeshCollider meshCollider;
	protected MeshFilter meshFilter;

	public Vector3 offset;
	Vector3 oOffset; //unchanged
	public int seed;
	public Map mapGen;

	public static float scale {
		get { return Map.currentWorld.scale; }
	}

	public static float amplitude {
		get { return Map.currentWorld.amplitude; }
	}

	void Start () {
		oOffset = offset;
		offset.x += mapGen.randomOffset;
		offset.z += mapGen.randomOffset;
		meshRenderer = GetComponent<MeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();
		meshFilter = GetComponent<MeshFilter>();
		Map.Tick += UpdateVisibility;
		CalculateMapFromScratch();
		StartCoroutine (CreateVisualMesh ());
	}

	void CalculateMapFromScratch() {
		//check to load
		map = InfNoise.Generate(scale, amplitude, offset, seed);
	}

	public virtual IEnumerator CreateVisualMesh() {
		visualMesh = new Mesh();

		List<Vector3> verts = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> tris = new List<int>();


		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				for (int z = 0; z < width; z++)
				{
					if (map[x,y,z] == 0) continue;

					byte brick = map[x,y,z];
					// Left wall
					if (IsTransparent(x - 1, y, z))
						BuildFace (brick, new Vector3(x, y, z), Vector3.up, Vector3.forward, false, verts, uvs, tris);
					// Right wall
					if (IsTransparent(x + 1, y , z))
						BuildFace (brick, new Vector3(x + 1, y, z), Vector3.up, Vector3.forward, true, verts, uvs, tris);

					// Bottom wall
					if (IsTransparent(x, y - 1 , z))
						BuildFace (brick, new Vector3(x, y, z), Vector3.forward, Vector3.right, false, verts, uvs, tris);
					// Top wall
					if (IsTransparent(x, y + 1, z))
						BuildFace (brick, new Vector3(x, y + 1, z), Vector3.forward, Vector3.right, true, verts, uvs, tris);

					// Back
					if (IsTransparent(x, y, z - 1))
						BuildFace (brick, new Vector3(x, y, z), Vector3.up, Vector3.right, true, verts, uvs, tris);
					// Front
					if (IsTransparent(x, y, z + 1))
						BuildFace (brick, new Vector3(x, y, z + 1), Vector3.up, Vector3.right, false, verts, uvs, tris);


				}
			}
			//yield return 0;
		}

		visualMesh.vertices = verts.ToArray();
		visualMesh.uv = uvs.ToArray();
		visualMesh.triangles = tris.ToArray();
		visualMesh.RecalculateBounds();
		visualMesh.RecalculateNormals();

		meshFilter.mesh = visualMesh;

		meshCollider.sharedMesh = null;
		meshCollider.sharedMesh = visualMesh;

		yield return 0;
	}

	public virtual void BuildFace(byte brick, Vector3 corner, Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
	{
		int index = verts.Count;

		verts.Add (corner);
		verts.Add (corner + up);
		verts.Add (corner + up + right);
		verts.Add (corner + right);

		Vector2 uvWidth = new Vector2(0.125f, 0.125f);
		Vector2 uvCorner = new Vector2(0.00f, 0.00f);

		//uvCorner.x += (float)(brick - 1) / 4;

		uvs.Add(uvCorner);
		uvs.Add(new Vector2(uvCorner.x, uvCorner.y + uvWidth.y));
		uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y + uvWidth.y));
		uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y));

		if (reversed)
		{
			tris.Add(index + 0);
			tris.Add(index + 1);
			tris.Add(index + 2);
			tris.Add(index + 2);
			tris.Add(index + 3);
			tris.Add(index + 0);
		}
		else
		{
			tris.Add(index + 1);
			tris.Add(index + 0);
			tris.Add(index + 2);
			tris.Add(index + 3);
			tris.Add(index + 2);
			tris.Add(index + 0);
		}

	}
	public virtual bool IsTransparent (int x, int y, int z)
	{
		if ( y < 0) return false;
		byte brick = GetByte(x,y,z);
		switch (brick)
		{
		case 0: 
			return true;
		default:
			return false;
		}
	}
	public virtual byte GetByte (int x, int y , int z)
	{

		if ((y < 0) || (y >= height))
			return InfNoise.GetTheoreticalByte (scale, amplitude, offset, new Vector3 (x, y, z));

		if ( (x < 0) || (z < 0)  || (x >= width) || (z >= width))
		{
			if (mapGen.blocks.Count == 0) {
				return InfNoise.GetTheoreticalByte (scale, amplitude, offset, new Vector3 (x, y, z));
			} else {
				Vector3 worldBlockPos = new Vector3 (x + oOffset.x, y + oOffset.y, z + oOffset.z);
				if (mapGen.blocks.ContainsKey (worldBlockPos)) {
					//print ("ok");
					return mapGen.blocks [worldBlockPos];
				} else {
					return InfNoise.GetTheoreticalByte (scale, amplitude, offset, new Vector3 (x, y, z));
				}
			}
		}
		return map[x,y,z];
	}

	void UpdateVisibility() {
		if (meshRenderer.enabled == true) {
			if (Vector3.Distance (oOffset, mapGen.viewer.position) > 100) {
				meshRenderer.enabled = false;
				meshCollider.enabled = false;
			}
		}
	}

	public void Enable() {
		meshRenderer.enabled = true;
		meshCollider.enabled = true;
	}

	public void SetBlock(bool visible, Vector3 pos) {

		pos -= oOffset;
		int x = Mathf.FloorToInt (pos.x);
		int y = Mathf.FloorToInt (pos.y);
		int z = Mathf.FloorToInt (pos.z);
		if (map [x, y, z] != 0) {
			return;
		} else {
			map [x, y, z] = 1;
		}
		StartCoroutine (CreateVisualMesh ());
	}

	public void SetBlockFalse(bool visible, Vector3 pos) {
		pos -= oOffset;
		int x = Mathf.FloorToInt (pos.x);
		int y = Mathf.FloorToInt (pos.y);
		int z = Mathf.FloorToInt (pos.z);
		if ((x == 0) || (z == 0) || (x == width) || (z == width)) {
			Vector3 worldBlockPos = new Vector3 (x + oOffset.x, y + oOffset.y, z + oOffset.z);
			mapGen.blocks.Add (worldBlockPos, 0);
		}
		if (map [x, y, z] == 0) {
			return;
		} else {
			map [x, y, z] = 0;
		}
		StartCoroutine (CreateVisualMesh ());
	}
		
}
