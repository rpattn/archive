using UnityEngine;
using System.Collections;

public class GunLook : MonoBehaviour {

	public Transform cam;

	void Update () {
		transform.rotation = cam.rotation;
	}
}
