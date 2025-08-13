using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(SecurityCamera))]
public class FOVeditor : Editor {

	void OnSceneGUI() {
		SecurityCamera fov = (SecurityCamera) target;
		Handles.color = Color.white;

		Handles.DrawWireDisc (fov.transform.position, Vector3.forward,  fov.viewRadius);

		Vector3 viewAngle = vec2To3(fov.DirFromAngle (-fov.viewAngle / 2f, false));

		Vector3 viewAngleB = vec2To3(fov.DirFromAngle (fov.viewAngle / 2f, false));

		Handles.DrawLine (fov.transform.position, fov.transform.position + viewAngle * fov.viewRadius);
		Handles.DrawLine (fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);


		Vector3 lookPos = vec2To3(fov.DirFromAngle (-fov.angleR, false));
		Vector3 lookPosB = vec2To3(fov.DirFromAngle (fov.angleL, false));
		Handles.color = Color.red;
		Handles.DrawLine (fov.transform.position, fov.transform.position + lookPos * fov.viewRadius);
		Handles.DrawLine (fov.transform.position, fov.transform.position + lookPosB * fov.viewRadius);
	}

	Vector3 vec2To3 (Vector2 angle) {
		return new Vector3 (angle.x, angle.y, 0f);
	}
}
