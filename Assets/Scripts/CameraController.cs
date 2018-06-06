using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameController game { get; set; }
	public Vector3 playerOffset { get; set; }

	void Update () {
		Vector3 position = game.playerController.transform.position;
		transform.position = new Vector3(position.x, position.y, position.z) + playerOffset;
		transform.LookAt (game.playerController.transform);
	}
}
