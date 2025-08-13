using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour{

	MapGenerator generator;
	GameObject meshObject;
	Vector2 position;
	Bounds bounds;
	int size;
	int[,] noiseMap;
	Block[,,] blockMap;

	public void GenerateTerrainChunk(Vector2 coord, int size, Transform parent, Material mat, float scale, float amplitude, MapGenerator gen) {
		generator = gen;
		position = coord * size;
		noiseMap = Noise.GenerateNoiseMap(size, size, scale, amplitude, position.x, position.y);
		blockMap = Noise.GenerateBlockMap(noiseMap); 
		bounds = new Bounds(position,Vector2.one * size);
		Vector3 positionV3 = new Vector3(position.x,0,position.y);
		this.size = size;

		meshObject = gameObject;
		meshObject.transform.position = positionV3;
		meshObject.transform.parent = parent;
		GenerateMesh ();
		SetVisible (false);
	}

	void GenerateMesh() {

		List<Vector3> verts = new List<Vector3> ();
		List<int> tris = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();
		Vector3 scale = Vector3.one;
		int vi = 0;       //vertIndex
		float e = 0.001f;                 //normal corerction
		float o = 0.249f;                 //combined offset o+e

		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				for (int z = 0; z < size; z++) {

					Block block = blockMap [x, y, z];

					if (block.visible == 1 && block.id != 0) {

						Vector3 pos = new Vector3 (x, block.height, z);
						float ox = block.ox;   //textOffset
						float oy = block.oy; 
						float tx = block.tx;     //topOffset
						float ty = block.ty; 
						float bx = block.bx;   //bottomOffset
						float by = block.by; 

						/*TopTris*/    //||
						if (BuildFace(x,y+1,z,block.height)) {
							verts.Add (BlockInfo.One (pos, scale)); //1
							verts.Add (BlockInfo.Two (pos, scale)); //2
							verts.Add (BlockInfo.Three (pos, scale)); //3
							verts.Add (BlockInfo.Four (pos, scale)); //4

							tris.AddRange (new int[] { 0 + vi, 1 + vi, 2 + vi, 0 + vi, 2 + vi, 3 + vi });
							vi += 4;

							uvs.Add (new Vector2 (tx + e, ty + o));
							uvs.Add (new Vector2 (tx + o, ty + o));
							uvs.Add (new Vector2 (tx + o, ty + e));
							uvs.Add (new Vector2 (tx + e, ty + e));
						}

						/*Bottom ----------------------------------------------------*/
						if (BuildFace(x,y-1,z,block.height)) {
							verts.Add (BlockInfo.Five (pos, scale)); //5
							verts.Add (BlockInfo.Six (pos, scale)); //6
							verts.Add (BlockInfo.Seven (pos, scale)); //7
							verts.Add (BlockInfo.Eight (pos, scale)); //8

							tris.AddRange (new int[] { 0 + vi, 2 + vi, 1 + vi, 0 + vi, 3 + vi, 2 + vi });
							vi += 4;

							uvs.Add (new Vector2 (bx + e, by + o));
							uvs.Add (new Vector2 (bx + o, by + o));
							uvs.Add (new Vector2 (bx + o, by + e));
							uvs.Add (new Vector2 (bx + e, by + e));
						}

						/*LeftTris ----------------------------------------------------*/
						if (BuildFace(x-1, y, z, block.height)) {
							verts.Add (BlockInfo.Two (pos, scale)); //2
							verts.Add (BlockInfo.Three (pos, scale)); //3
							verts.Add (BlockInfo.Six (pos, scale)); //6
							verts.Add (BlockInfo.Seven (pos, scale)); //7

							tris.AddRange (new int[] { 0 + vi, 2 + vi, 1 + vi, 1 + vi, 2 + vi, 3 + vi });
							vi += 4;

							uvs.Add (new Vector2 (ox + e, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + e));
							uvs.Add (new Vector2 (ox + e, oy + e));
						}

						/*Right -------------------------------------------------------*/
						if (BuildFace(x+1, y, z, block.height)) {
							verts.Add (BlockInfo.One (pos, scale)); //1
							verts.Add (BlockInfo.Four (pos, scale)); //4
							verts.Add (BlockInfo.Five (pos, scale)); //5
							verts.Add (BlockInfo.Eight (pos, scale)); //8

							tris.AddRange (new int[] { 0 + vi, 1 + vi, 2 + vi, 1 + vi, 3 + vi, 2 + vi });
							vi += 4;

							uvs.Add (new Vector2 (ox + e, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + e));
							uvs.Add (new Vector2 (ox + e, oy + e));
						}

						/*FrontTris --------------------------------------------------*/
						if (BuildFace(x,y,z-1, block.height)) {
							verts.Add (BlockInfo.One (pos, scale)); //1
							verts.Add (BlockInfo.Two (pos, scale)); //2
							verts.Add (BlockInfo.Five (pos, scale)); //5
							verts.Add (BlockInfo.Six (pos, scale)); //6

							tris.AddRange (new int[] { 0 + vi, 2 + vi, 3 + vi, 0 + vi, 3 + vi, 1 + vi });
							vi += 4;

							uvs.Add (new Vector2 (ox + e, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + e));
							uvs.Add (new Vector2 (ox + e, oy + e));
						}

						/*Back --------------------------------------------------------*/
						if (BuildFace(x,y,z+1, block.height)) {
							verts.Add (BlockInfo.Three (pos, scale)); //3
							verts.Add (BlockInfo.Four (pos, scale)); //4
							verts.Add (BlockInfo.Seven (pos, scale)); //7
							verts.Add (BlockInfo.Eight (pos, scale)); //8

							tris.AddRange (new int[] { 1 + vi, 0 + vi, 2 + vi, 1 + vi, 2 + vi, 3 + vi });
							vi += 4;

							uvs.Add (new Vector2 (ox + e, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + e));
							uvs.Add (new Vector2 (ox + e, oy + e));
						}
					}
				}
			}
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = verts.ToArray ();
		mesh.triangles = tris.ToArray ();
		mesh.uv = uvs.ToArray ();
		meshObject.GetComponent<MeshFilter> ().mesh = mesh;
		meshObject.GetComponent<MeshCollider> ().sharedMesh = mesh;
		meshObject.GetComponent<MeshRenderer> ().material = generator.mat;
	}

	public bool InBounds (int x, int y , int z)
	{

		if ((y < 0) || (y >= 7)) {
			return false;
		}

		if ((x < 0) || (z < 0) || (x >= size) || (z >= size)) {
			return false;
		}

		return true;
	}

	public bool BuildFace(int x, int y, int z, int height) {   //takes in +- x or y values 
		if (InBounds (x, y, z)) {
			if (blockMap [x, y, z].height != height || blockMap [x, y, z].id == 0  || blockMap [x,y,z].visible == 0) {
				return true;
			} else {
				return false;
			}
		} else {
			return true;
		}
	}
	public void SetBlock(int visible, Vector3 pos) {
		Vector3 coord = new Vector3 (pos.x - position.x, pos.y, pos.z - position.y);  //do trig here though
		int x = Mathf.FloorToInt (coord.x);
		int z = Mathf.FloorToInt (coord.z);
		print (x);
		for (int i = 0; i < blockMap.GetLength (1); i++) {
			blockMap [x, i, z].visible = 0;
		}
		GenerateMesh ();
	}

	public void UpdateTerrainChunk(Vector3 viewPos, float maxViewDst) {
		float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance (viewPos));
		bool visible = viewerDstFromNearestEdge <= maxViewDst;
		SetVisible (visible);
	}

	public void SetVisible(bool visible) {
		meshObject.SetActive (visible);
	}

	public bool IsVisible() {
		return meshObject.activeSelf;
	}

}
