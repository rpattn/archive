using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public AnimationCurve chunkXCurve;
	public AnimationCurve chunkYCurve;

	int chunkX;
	int chunkY;

	public int maxRoomWidth;
	[Range(5, 25)]
	public int attempts;

	public GameObject[] wallTiles;
	public GameObject[] groundTiles;

	public GameObject player;
	public GameObject end;

	public GameObject food;
	public GameObject drink;

	public AnimationCurve exitDistance;

	public Vector2 startPos;
	public Vector2 endPos;

	private int[,] map;

	public void Start() {
		GenerateTileMap ();
		GenerateOther ();
	}

	public void GenerateTileMap() {

		DestroyTiles ();

		chunkX = Mathf.RoundToInt( chunkXCurve.Evaluate(Random.Range (15, 75)) );
		chunkY = Mathf.RoundToInt( chunkXCurve.Evaluate(Random.Range (15, 75)) );

		map = new int[chunkX, chunkY];

		for (int x = 0; x < chunkX; x++) {
			for (int y = 0; y < chunkY; y++) {

				if (x == 0 || y == 0 || x == chunkX - 1 || y == chunkY - 1) {
					map [x, y] = 1; //1 = wall
				} else {
					map [x, y] = 1;
				}
			}
		}
			

		Queue<Vector2> stack = new Queue<Vector2> ();
		stack.Enqueue (new Vector2(1,1));

		while (stack.Count != 0) {
			Vector2 current = stack.Dequeue ();

			List<Vector2> neighbours = new List<Vector2> ();
			if (current.x < map.GetLength (0) - 2) {
				Vector2 newVec = new Vector2 (current.x + 2, current.y);
				if (map [(int)newVec.x, (int)newVec.y] == 1) {
					neighbours.Add (newVec);
				}
			}
			if (current.y < map.GetLength (1) - 2) {
				Vector2 newVec = new Vector2 (current.x, current.y + 2);
				if (map [(int)newVec.x, (int)newVec.y] == 1) {
					neighbours.Add (newVec);
				}
			}
			if (current.x > 1) {
				Vector2 newVec = new Vector2 (current.x - 2, current.y);
				if (map [(int)newVec.x, (int)newVec.y] == 1) {
					neighbours.Add (newVec);
				}
			}
			if (current.y > 1) {
				Vector2 newVec = new Vector2 (current.x, current.y - 2);
				if (map [(int)newVec.x, (int)newVec.y] == 1) {
					neighbours.Add (newVec);
				}
			}

			if (neighbours.Count != 0) {
				int random = Random.Range (0, neighbours.Count);
				Vector2 next = new Vector2 (neighbours [random].x, neighbours [random].y);
				map [(int)current.x, (int)current.y] = 0;
				map [(int)next.x, (int)next.y] = 0;
				if (current.x < next.x) {
					map [(int)next.x - 1, (int)next.y] = 0;
				}
				if (current.y < next.y) {
					map [(int)next.x, (int)next.y - 1] = 0;
				}
				if (current.x > next.x) {
					map [(int)next.x + 1, (int)next.y] = 0;
				}
				if (current.y > next.y) {
					map [(int)next.x, (int)next.y + 1] = 0;
				}
				stack.Enqueue (current);
				stack.Enqueue (next);
			}

			//stack.Enqueue (next);
		}
			
		for (int x = 0; x < chunkX; x++) {
			for (int y = 0; y < chunkY; y++) {

				if (x == 0 || y == 0 || x == chunkX - 1 || y == chunkY - 1) {
					map [x, y] = 1; //1 = wall
				}
			}
		}

		//Rooms Generation

		List<Vector4> Rooms = new List<Vector4> ();

		int newAttempts = Mathf.RoundToInt( (1/16f) * Mathf.Pow(chunkX+chunkY, 2) * (1/(float)attempts));
		print (newAttempts);

		for (int i = 0; i < newAttempts; i++) {
			int startx = Random.Range(2, chunkX - maxRoomWidth);
			int starty = Random.Range(2, chunkY - maxRoomWidth);
			int widthx = Random.Range(3, maxRoomWidth);
			int widthy = Random.Range(3, maxRoomWidth);

			startPos = new Vector2 (startx - (chunkX/2), starty - (chunkY/2));

			for (int x = startx; x < startx+ widthx; x++) {
				for (int y = starty; y < starty + widthy; y++) {

					map [x, y] = 0;
					Rooms.Add(new Vector4(startx,starty,widthx,widthy));
				}
			}
		}

		bool found = false;
		int remaining = Rooms.Count;

		while (!found && remaining > 0) {
			if (Vector2.Distance (startPos, new Vector2 (Rooms [remaining - 1].x - (chunkX/2), 
															Rooms [remaining - 1].y - (chunkY/2))) 
																> exitDistance.Evaluate(chunkX)) {
				found = true;
				endPos = new Vector2 (Rooms [remaining - 1].x - (chunkX/2), 
										Rooms [remaining - 1].y - (chunkY/2));
			}
			remaining -= 1;
		}

		if (!found) {
			Debug.LogError ("No Exit generated");
		}

		//Tile Generation

		for (int x = 0; x < chunkX; x++) {
			for (int y = 0; y < chunkY; y++) {

				if (map [x, y] == 0) {
					GameObject item = IsItem ();
					if (item != null) {
						GameObject tile = Instantiate (item, new Vector3 (x - (chunkX / 2), y - (chunkY / 2), x), 
							Quaternion.identity) as GameObject;
						tile.transform.parent = transform;
					}
				}

				if (map [x, y] == 1) {
					GameObject tile = Instantiate (GetTile(map[x,y]), new Vector3 (x - (chunkX / 2), y - (chunkY / 2), x), 
						Quaternion.identity) as GameObject;
					tile.transform.parent = transform;
				} else {
					GameObject tile = Instantiate (GetTile(map[x,y]), new Vector3 (x - (chunkX / 2), y - (chunkY / 2), x), 
						Quaternion.identity) as GameObject;
					tile.transform.parent = transform;
				}
			}
		}
			
	}

	public void GenerateOther() {
		Instantiate (player, startPos, Quaternion.identity);
		Instantiate (end, endPos, Quaternion.identity);
	}

	public GameObject GetTile (int isWall) {

		GameObject toReturn = null;

		if (isWall == 1) {

			int random = Random.Range (0, wallTiles.Length + 3);
			int i = (random <= 3) ? 0 : random - 4;

			toReturn = wallTiles [i];
		} else {
							
			int random = Random.Range (0, groundTiles.Length + 3);
			int i = (random <= 3) ? 0 : random - 4;

			toReturn = groundTiles [i];

		}

		return toReturn;
	}

	public GameObject IsItem() {
		GameObject toReturn = null;

		int item = Random.Range (0, 100);
		if (item == 1) {
			toReturn = food;
		} else if (item == 0) {
			toReturn = drink;
		}

		return toReturn;
	}

	public void DestroyTiles() {

		while (transform.childCount != 0) {
			DestroyImmediate (transform.GetChild (0).gameObject);
			DestroyImmediate (GameObject.FindGameObjectWithTag ("Player"));
			DestroyImmediate (GameObject.FindGameObjectWithTag ("Exit"));
		}
	}
		
}
