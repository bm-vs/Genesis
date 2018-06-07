using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameController game { get; set; }
	public Vector3 playerOffset = new Vector3(-30f, 45, -30f);

	void Update () {
		if (UnityEngine.Input.GetAxisRaw ("RotateCamera") != 0) {
			playerOffset = Quaternion.Euler(0, UnityEngine.Input.GetAxisRaw ("RotateCamera"), 0) * playerOffset;
		}

		Vector3 position = game.playerController.transform.position;
		transform.position = new Vector3(position.x, position.y, position.z) + playerOffset;
		transform.LookAt (game.playerController.transform);
	}
}
