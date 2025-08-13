using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecurityCamera : MonoBehaviour {

	[Header("View Options")]
	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;

	[Header("PlayerOptions")]
	public PlayerHealthManager playerHealth;
	public float damage;

	[Header("Look Options")]
	[Range(0,360)]
	public float angleL;
	[Range(0,360)]
	public float angleR;
	[Range(1,20)]
	public float speed;
	public bool look;
	bool right;
	float startRot;

	[Header("Mesh Options")]
	public float meshResolution;
	public int edgeResolveIterations;
	public float maxEdgeDst;
	public LayerMask obstacleMask;
	public LayerMask playerMask;
	public bool findEdges;

	[Header("Object assignment")]
	public MeshFilter meshFilter;
	public MeshCollider meshCollider;
	public Material meshMaterial;
	public Color playerFoundColor;
	Mesh viewMesh;
	Color defaultMeshColour;
	bool playerAlive;

	void Start() {
		viewMesh = new Mesh ();
		viewMesh.name = "View Mesh";
		meshFilter.mesh = viewMesh;
		meshCollider.sharedMesh = viewMesh;
		startRot = transform.eulerAngles.z;
		defaultMeshColour = meshMaterial.color;
		PlayerHealthManager.OnDeath += OnPlayerDeath;
	}

	void Update() {
		MoveCamera ();
	}

	void LateUpdate() {
		playerAlive = playerHealth.isAlive ();
		if (playerAlive) {
			CheckForPlayer ();
		} else {
			meshMaterial.color = defaultMeshColour;
		}
		DrawFOV ();
	}

	void MoveCamera() {
		if (look) {
			if (right && transform.eulerAngles.z < startRot + angleR / 2) {
				transform.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));
			} else {
				right = false;
			}
			if (!right && transform.eulerAngles.z > startRot - (angleL / 2)) {
				transform.Rotate (new Vector3 (0,0, -speed * Time.deltaTime));
			} else {
				right = true;
			}
		}
	}

	void OnPlayerFound() {
		if (playerAlive) {
			playerHealth.TakeHealth (damage);
			meshMaterial.color = Color.Lerp (defaultMeshColour, playerFoundColor, 3 * Time.deltaTime);
		}
	}

	void OnPlayerDeath() {
		meshMaterial.color = defaultMeshColour;
		playerAlive = false;
		print (playerAlive);
	}

	void CheckForPlayer() {
		int stepCount = Mathf.RoundToInt (viewAngle * (meshResolution / 4f));
		float stepAngleSize = -viewAngle / stepCount;

		for (int i = 0; i <= stepCount; i++) {
			float angle = -transform.eulerAngles.z + viewAngle / 2 + stepAngleSize * i;
			//Debug.DrawLine (transform.position, transform.position + DirFromAngle3 (angle, true) * viewRadius, Color.red);
			Vector3 dir = DirFromAngle3 (angle, true);

			RaycastHit2D hit = Physics2D.Raycast (GetPosition (), dir, viewRadius);

			if (hit.collider != null) {
				if (hit.collider.gameObject.tag == "Player") {
					OnPlayerFound ();
					//break;
				}
			}

		}

		if (!playerHealth.beingDamaged) {
			meshMaterial.color = Color.Lerp (meshMaterial.color, defaultMeshColour, 3 * Time.deltaTime);
		}
	}

	void DrawFOV () {
		int stepCount = Mathf.RoundToInt (viewAngle * meshResolution);
		float stepAngleSize = -viewAngle / stepCount;

		List<Vector3> viewPoints = new List<Vector3> ();
		ViewCastInfo oldViewCastInfo = new ViewCastInfo ();

		for (int i = 0; i <= stepCount; i++) {
			float angle = -transform.eulerAngles.z + viewAngle / 2 + stepAngleSize * i;
			//Debug.DrawLine (transform.position, transform.position + DirFromAngle3 (angle, true) * viewRadius, Color.red);
			ViewCastInfo newViewCast = ViewCast (angle);

			if (i > 0 && findEdges) {
				bool edgeNotValid = Mathf.Abs (oldViewCastInfo.dst - newViewCast.dst) > maxEdgeDst;
				if (oldViewCastInfo.hit != newViewCast.hit || (oldViewCastInfo.hit && newViewCast.hit && edgeNotValid)) {
					EdgeInfo edge = FindEdge (oldViewCastInfo, newViewCast);
					if (edge.pointA != Vector3.zero) {
						viewPoints.Add (edge.pointA);
					}
					if (edge.pointB != Vector3.zero) {
						viewPoints.Add (edge.pointB);
					}
				}
			}

			viewPoints.Add (newViewCast.point);
			oldViewCastInfo = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] verticies = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		verticies [0] = Vector3.zero;

		for (int i = 0; i < vertexCount - 2; i++) {
			verticies [i + 1] = transform.InverseTransformPoint (viewPoints [i]);

			triangles [i * 3] = i + 2;
			triangles [i * 3 + 1] = i + 1;
			triangles [i * 3 + 2] = 0;
		}

		viewMesh.Clear ();
		viewMesh.vertices = verticies;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals ();
	}

	EdgeInfo FindEdge (ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;

		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast (angle);

			bool edgeNotValid = Mathf.Abs (minViewCast.dst - newViewCast.dst) > maxEdgeDst;
			if (newViewCast.hit == minViewCast.hit && !edgeNotValid) {
				minAngle = angle;
				minPoint = newViewCast.point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}

	ViewCastInfo ViewCast (float globalAngle) {
		Vector3 dir = DirFromAngle3 (globalAngle, true);

		RaycastHit2D hit = Physics2D.Raycast (GetPosition (), dir, viewRadius, obstacleMask);

		if (hit.collider != null) {
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		} else {
			return new ViewCastInfo (false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
	}

	public Vector2 DirFromAngle (float angleDegrees, bool isGlobal) {
		angleDegrees -= (!isGlobal) ? transform.eulerAngles.z : 0; 
		return new Vector2 (Mathf.Sin (angleDegrees * Mathf.Deg2Rad), Mathf.Cos (angleDegrees * Mathf.Deg2Rad));
	}

	public Vector3 DirFromAngle3 (float angleDegrees, bool isGlobal) {
		angleDegrees -= (!isGlobal) ? transform.eulerAngles.z : 0; 
		return new Vector3 (Mathf.Sin (angleDegrees * Mathf.Deg2Rad), Mathf.Cos (angleDegrees * Mathf.Deg2Rad), 0);
	}

	Vector2 GetPosition () {
		return new Vector2 (transform.position.x, transform.position.y);
	}

	public struct ViewCastInfo {
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo (bool hit, Vector3 point, float dst, float angle) {
			this.hit = hit;
			this.angle = angle;
			this.point = point;
			this.dst = dst;
		}
	}

	public struct EdgeInfo {
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo (Vector3 pointA, Vector3 pointB) {
			this.pointA = pointA;
			this.pointB = pointB;
		}
	}
}
