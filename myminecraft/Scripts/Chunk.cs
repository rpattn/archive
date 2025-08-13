using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;

public class Chunk : MonoBehaviour{

	MapGenerator generator;
	GameObject meshObject;
	Vector2 position;
	Bounds bounds;
	int size;
	//int[,] noiseMap;
	Block[,,] blockMap;
	public int minYVal = 0;

	private BackgroundWorker worker;//mesh
	private BackgroundWorker worker2;//blockMap
	public MeshData meshData;
	public MapData mapData;

	public void GenerateTerrainChunk(Vector2 coord, int size, Transform parent, Material mat, float scale, float amplitude, MapGenerator gen) {
		generator = gen;
		this.size = size;
		position = coord * size;
		bounds = new Bounds(position,Vector2.one * size);
		Vector3 positionV3 = new Vector3(position.x,0,position.y);

		meshObject = gameObject;
		meshObject.transform.position = positionV3;
		meshObject.transform.parent = parent;
		mapData = new MapData (scale, amplitude, gen.seed, gen.biomes);

		GenerateMapData (mapData);
		//blockMap = Noise.GenerateBlockMap(size, scale, amplitude, position.x, position.y, gen.seed, gen.biomes); 
		//GenerateMesh (false);
		//SetVisible (false);
	}
	//Method creates a new thread when generaate Mesh called -----------------------------------------------Threading Mesh--------------------------------------
	void GenerateMesh(bool update) {

		//Threading
		worker = new BackgroundWorker ();
		worker.DoWork += new DoWorkEventHandler (StartThreadData);
		worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler (ThreadMeshDataComplete);
		worker.RunWorkerAsync (update);
		//end
	}
	//Method is called when the worler is started
	private void StartThreadData(object sender, DoWorkEventArgs e) {
		BackgroundWorker worker = sender as BackgroundWorker;
		//int arg = (int)e.Argument;
		e.Result = ThreadMeshData ((bool)e.Argument, worker);
		if (worker.CancellationPending) {
			e.Cancel = true;
		}
	}
	//When meshData recieved, assign it to the components
	void ThreadMeshDataComplete(object sender, RunWorkerCompletedEventArgs e) {
		meshData = (MeshData)e.Result;
		Mesh mesh = new Mesh ();
		mesh.vertices = meshData.verts.ToArray ();
		mesh.triangles = meshData.tris.ToArray ();
		mesh.uv = meshData.uvs.ToArray ();
		mesh.normals = meshData.normals.ToArray ();
		meshObject.GetComponent<MeshCollider> ().sharedMesh = mesh;
		Vector3[] lightNormals = new Vector3[meshData.normals.Count];
		for (int i = 0; i < lightNormals.Length; i++) {
			lightNormals [i] = Vector3.up;
		}
		mesh.normals = lightNormals;
		meshObject.GetComponent<MeshFilter> ().mesh = mesh;
		//meshObject.GetComponent<MeshRenderer> ().material = generator.mat;
	}
	//Method to calculate meshData in seperate Thread
	private MeshData ThreadMeshData(bool isUpdate, BackgroundWorker bw) {
		List<Vector3> verts = new List<Vector3> ();
		List<int> tris = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();
		List<Vector3> normals = new List<Vector3> ();
		Vector3 scale = Vector3.one;
		int vi = 0;       //vertIndex
		float e = 0.001f;                 //normal corerction
		float o = 0.124f;       //combined offset o+e
		int mapHeight = blockMap.GetLength(1);
		if (isUpdate != true) {
			minYVal = (mapHeight / 2);  //starting y value that changes
		}
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < mapHeight; y++) {   //change to val
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
						if (BuildFace(x,y+1,z,mapHeight)) {
							verts.Add (BlockInfo.One (pos, scale)); //1
							verts.Add (BlockInfo.Two (pos, scale)); //2
							verts.Add (BlockInfo.Three (pos, scale)); //3
							verts.Add (BlockInfo.Four (pos, scale)); //4
							normals.AddRange(BlockInfo.UPNormals());

							tris.AddRange (new int[] { 0 + vi, 1 + vi, 2 + vi, 0 + vi, 2 + vi, 3 + vi });
							vi += 4;

							uvs.Add (new Vector2 (tx + e, ty + o));
							uvs.Add (new Vector2 (tx + o, ty + o));
							uvs.Add (new Vector2 (tx + o, ty + e));
							uvs.Add (new Vector2 (tx + e, ty + e));
						}

						/*Bottom ----------------------------------------------------*/
						if (BuildFace(x,y-1,z,mapHeight)) {
							verts.Add (BlockInfo.Five (pos, scale)); //5
							verts.Add (BlockInfo.Six (pos, scale)); //6
							verts.Add (BlockInfo.Seven (pos, scale)); //7
							verts.Add (BlockInfo.Eight (pos, scale)); //8
							normals.AddRange(BlockInfo.DownNormals());

							tris.AddRange (new int[] { 0 + vi, 2 + vi, 1 + vi, 0 + vi, 3 + vi, 2 + vi });
							vi += 4;

							uvs.Add (new Vector2 (bx + e, by + o));
							uvs.Add (new Vector2 (bx + o, by + o));
							uvs.Add (new Vector2 (bx + o, by + e));
							uvs.Add (new Vector2 (bx + e, by + e));
						}

						/*LeftTris ----------------------------------------------------*/
						if (BuildFace(x-1, y, z, mapHeight)) {
							verts.Add (BlockInfo.Two (pos, scale)); //2
							verts.Add (BlockInfo.Three (pos, scale)); //3
							verts.Add (BlockInfo.Six (pos, scale)); //6
							verts.Add (BlockInfo.Seven (pos, scale)); //7
							normals.AddRange(BlockInfo.LeftNormals());

							tris.AddRange (new int[] { 0 + vi, 2 + vi, 1 + vi, 1 + vi, 2 + vi, 3 + vi });
							vi += 4;

							uvs.Add (new Vector2 (ox + o, oy + o));
							uvs.Add (new Vector2 (ox + e, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + e));
							uvs.Add (new Vector2 (ox + e, oy + e));
						}

						/*Right -------------------------------------------------------*/
						if (BuildFace(x+1, y, z, mapHeight)) {
							verts.Add (BlockInfo.One (pos, scale)); //1
							verts.Add (BlockInfo.Four (pos, scale)); //4
							verts.Add (BlockInfo.Five (pos, scale)); //5
							verts.Add (BlockInfo.Eight (pos, scale)); //8
							normals.AddRange(BlockInfo.RightNormals());

							tris.AddRange (new int[] { 0 + vi, 1 + vi, 2 + vi, 1 + vi, 3 + vi, 2 + vi });
							vi += 4;

							uvs.Add (new Vector2 (ox + o, oy + o));
							uvs.Add (new Vector2 (ox + e, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + e));
							uvs.Add (new Vector2 (ox + e, oy + e));
						}

						/*FrontTris --------------------------------------------------*/
						if (BuildFace(x,y,z-1, mapHeight)) {
							verts.Add (BlockInfo.One (pos, scale)); //1
							verts.Add (BlockInfo.Two (pos, scale)); //2
							verts.Add (BlockInfo.Five (pos, scale)); //5
							verts.Add (BlockInfo.Six (pos, scale)); //6
							normals.AddRange(BlockInfo.ForwardNormals());

							tris.AddRange (new int[] { 0 + vi, 2 + vi, 3 + vi, 0 + vi, 3 + vi, 1 + vi });
							vi += 4;

							uvs.Add (new Vector2 (ox + o, oy + o));
							uvs.Add (new Vector2 (ox + e, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + e));
							uvs.Add (new Vector2 (ox + e, oy + e));
						}

						/*Back --------------------------------------------------------*/
						if (BuildFace(x,y,z+1, mapHeight)) {
							verts.Add (BlockInfo.Three (pos, scale)); //3
							verts.Add (BlockInfo.Four (pos, scale)); //4
							verts.Add (BlockInfo.Seven (pos, scale)); //7
							verts.Add (BlockInfo.Eight (pos, scale)); //8
							normals.AddRange(BlockInfo.BackNormals());

							tris.AddRange (new int[] { 1 + vi, 0 + vi, 2 + vi, 1 + vi, 2 + vi, 3 + vi });
							vi += 4;

							uvs.Add (new Vector2 (ox + o, oy + o));
							uvs.Add (new Vector2 (ox + e, oy + o));
							uvs.Add (new Vector2 (ox + o, oy + e));
							uvs.Add (new Vector2 (ox + e, oy + e));
						}
					}
				}
			}
			Thread.Sleep ((isUpdate)?0:50); //no sleep if block just changed
		}
		MeshData data = new MeshData ();
		data.verts = verts;
		data.tris = tris;
		data.uvs = uvs;
		data.normals = normals;
		return data;
		//thread safe ends here ----------------------------------------------------------------------------EndOfThreadingCode----------------------------------------------
	}


	//Method Sets Up BlockMap thread
	void GenerateMapData(MapData mapData) {
		worker2 = new BackgroundWorker ();
		worker2.DoWork += new DoWorkEventHandler (StartThreadMapData);
		worker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler (ThreadMapDataComplete);
		worker2.RunWorkerAsync (mapData);
	}
	//starts
	private void StartThreadMapData(object sender, DoWorkEventArgs e) {
		BackgroundWorker worker2 = sender as BackgroundWorker;
		//int arg = (int)e.Argument;
		e.Result = GenerateBlockMap ((MapData)e.Argument, worker2);
		if (worker2.CancellationPending) {
			e.Cancel = true;
		}
	}
	//oncomplete
	void ThreadMapDataComplete(object sender, RunWorkerCompletedEventArgs e) {
		blockMap = (Block[,,])e.Result;
		GenerateMesh (false);
		SetVisible (false);
	}
	//doesthework
	private Block[,,] GenerateBlockMap(MapData mapData, BackgroundWorker bw) {
		Block[,,] data = Noise.GenerateBlockMap(size, mapData.scale, mapData.amplitude, position.x, position.y, mapData.seed, mapData.biomes); 
		return data;
	}

	public bool InBounds (int x, int y , int z, int height)
	{

		if ((y < 0) || (y >= height)) {
			return false;
		}

		if ((x < 0) || (z < 0) || (x >= size) || (z >= size)) {
			return false;
		}

		return true;
	}

	public bool BuildFace(int x, int y, int z, int height) {   //takes in +- x or y values 
		if (InBounds (x, y, z, height)) {
			if (blockMap [x, y, z].id == 0  || blockMap [x,y,z].visible == 0) {  //blockMap [x, y, z].height != height || 
				return true;
			} else {
				return false;
			}
		} else {
			if (y > minYVal) {
				return true;
			} else {
				return false;
			}
		}
	}
	public void SetBlock(bool visible, Vector3 pos) {
		
		Vector3 coord = new Vector3 (pos.x - position.x, pos.y, pos.z - position.y);  //do trig here though
		int x = (int)Math.Round(coord.x, MidpointRounding.AwayFromZero)+1;
		int y = (int)Math.Round(coord.y, MidpointRounding.AwayFromZero);
		int z = (int)Math.Round(coord.z, MidpointRounding.AwayFromZero);
		int height = blockMap[x, y, z].height;
		blockMap [x, y, z] = new StoneBlock (height, visible);
		GenerateMesh (true);
	}

	public void SetBlockFalse(bool visible, Vector3 pos) {

		Vector3 coord = new Vector3 (pos.x - position.x, pos.y, pos.z - position.y);  //do trig here though
		int x = (int)Math.Round(coord.x, MidpointRounding.AwayFromZero) +1;
		int y = (int)Math.Round(coord.y, MidpointRounding.AwayFromZero);
		int z = (int)Math.Round(coord.z, MidpointRounding.AwayFromZero);
		//Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube), new Vector3(x + 0.5f,y + 0.1f,z + 0.1f), Quaternion.identity);
		if (y == 0) {
			return; //Cant mine bedrock
		}
		blockMap [x, y, z].id = 0;
		blockMap [x, y, z].visible = 0;
		if (y <= minYVal) {
			minYVal = y;
		}
		if (x == 0) {												//lowers the draw floor of surrounding chunks based on the lowest current value
			Vector2 poscoord = new Vector2 ((position.x - size) / size, (position.y / size));
			if (y <= generator.currentChunkDict [poscoord].minYVal) {
				//minYVal = y;
				generator.currentChunkDict [poscoord].minYVal = y - 1;
				generator.currentChunkDict [poscoord].GenerateMesh (true);
			}
		}
		if (z == 0) {
			Vector2 poscoord = new Vector2 ((position.x / size), ((position.y - size) / size));
			if (y <= generator.currentChunkDict [poscoord].minYVal) {
				//minYVal = y;
				generator.currentChunkDict [poscoord].minYVal = y - 1;
				generator.currentChunkDict [poscoord].GenerateMesh (true);
			}
		}
		if (x == size-1) {
			Vector2 poscoord = new Vector2 ((position.x + size) / size, (position.y / size));
			if (y <= generator.currentChunkDict [poscoord].minYVal) {
				//minYVal = y;
				generator.currentChunkDict [poscoord].minYVal = y - 1;
				generator.currentChunkDict [poscoord].GenerateMesh (true);
			}
		}
		if (z == size-1) {
			Vector2 poscoord = new Vector2 ((position.x / size), ((position.y + size) / size));
			if (y <= generator.currentChunkDict [poscoord].minYVal) {
				//minYVal = y;
				generator.currentChunkDict [poscoord].minYVal = y - 1;
				generator.currentChunkDict [poscoord].GenerateMesh (true);
			}
		}
		GenerateMesh (true);

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

public struct MeshData {
	public List<Vector3> verts;
	public List<int> tris;
	public List<Vector2> uvs;
	public List<Vector3> normals;
}

public struct MapData {
	public float scale;
	public float amplitude;
	public int seed;
	public Biome[] biomes;

	public MapData(float sc, float amp, int sd, Biome[] bime) {
		scale = sc;
		amplitude = amp;
		seed = sd;
		biomes = bime;
	}
}
