using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchInput : MonoBehaviour {

	public PlayerInput playerInput;

	public int minMoveDst;

	Dictionary<int, Vector2> touchMoves = new Dictionary<int, Vector2> ();

	void Update() {

		foreach (Touch touch in Input.touches) {

			switch (touch.phase) {

			case (TouchPhase.Began):

				touchMoves.Add (touch.fingerId, Vector2.zero);
				break;
			
			case(TouchPhase.Moved) :

				if (touchMoves.ContainsKey (touch.fingerId)) {
					touchMoves [touch.fingerId] += touch.deltaPosition;

					if (touchMoves [touch.fingerId].x > minMoveDst) {
						playerInput.setDirInput (touchMoves [touch.fingerId]); //right
					} else if (touchMoves [touch.fingerId].x < -minMoveDst) {
						playerInput.setDirInput (touchMoves [touch.fingerId]); //left
					}

					if (touchMoves [touch.fingerId].y > minMoveDst) {
						playerInput.screenJump ();
					}
				}
				break;

			case(TouchPhase.Stationary):
				playerInput.setDirInput (touchMoves [touch.fingerId]);
				break;
			
			case (TouchPhase.Ended) :

				if (touchMoves.ContainsKey (touch.fingerId)) {
					touchMoves.Remove (touch.fingerId);
				}
				playerInput.jumpInputUp ();
				break;

			}
		}
	}
}
