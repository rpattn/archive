using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshGenerator {

	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, AnimationCurve cutCurve) {
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		MeshData meshData = new MeshData (width, height);
		int vertexIndex = 0;

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {

				float currentHeight = heightCurve.Evaluate(heightMap [x, y]) * heightMultiplier;
				float cutOffHeight = cutCurve.Evaluate(heightMap [x, y]) * heightMultiplier;
				/*
				meshData.vertices.Add (new Vector3 (topLeftX + x, currentHeight, topLeftZ - y));

				meshData.uvs.Add (new Vector2 (x / (float)width, y / (float)height));

				if (x < width - 1 && y < height - 1) {
					meshData.AddTriangle (vertexIndex, vertexIndex + width + 1, vertexIndex + width);
					meshData.AddTriangle (vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
				}*/

				int width2 = (width - 1) * 3;

				if (x < width - 1 && y < height - 1) {
					meshData.vertices.Add (new Vector3 (topLeftX + x, currentHeight, topLeftZ - y));
					meshData.vertices.Add (new Vector3 (topLeftX + x + 1, currentHeight, topLeftZ - y + 1));
					meshData.vertices.Add (new Vector3 (topLeftX + x, currentHeight, topLeftZ - y + 1));

					meshData.vertices.Add (new Vector3 (topLeftX + x + 1, currentHeight, topLeftZ - y + 1));
					meshData.vertices.Add (new Vector3 (topLeftX + x, currentHeight, topLeftZ - y));
					meshData.vertices.Add (new Vector3 (topLeftX + x + 1, currentHeight, topLeftZ - y));

					/*meshData.vertices.Add (new Vector3 (topLeftX + x + 1, currentHeight, topLeftZ - y));
					meshData.vertices.Add (new Vector3 (topLeftX + x + 1, currentHeight, topLeftZ - y + 1));
					meshData.vertices.Add (new Vector3 (topLeftX + x, currentHeight, topLeftZ - y + 1));*/

					meshData.AddTriangle (vertexIndex, vertexIndex + width2 + 1, vertexIndex + width2);
					meshData.AddTriangle (vertexIndex + width2 + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex += 4;

			}
		}

		return meshData;

	}
		
}

public class MeshData {
	public List<Vector3> vertices;
	public List<int> triangles;
	public List<Vector2> uvs;

	int triangleIndex;

	public MeshData(int meshWidth, int meshHeight) {
		vertices = new List<Vector3> ();
		uvs = new List<Vector2> ();
		triangles = new List<int> ();
	}

	public void AddTriangle(int a, int b, int c) {
		triangles.Add (a);
		triangles.Add (b);
		triangles.Add (c);
		triangleIndex += 3;
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals ();
		return mesh;
	}

}
