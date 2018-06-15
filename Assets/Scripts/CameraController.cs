using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject player;
	public Vector3 playerOffset = new Vector3(-30f, 45, -30f);

	void Update () {
		if (UnityEngine.Input.GetAxisRaw ("RotateCamera") != 0) {
			playerOffset = Quaternion.Euler(0, UnityEngine.Input.GetAxisRaw ("RotateCamera"), 0) * playerOffset;
		}

		PlayerController playerController = player.GetComponents<PlayerController> ()[0];
		Vector3 position = playerController.transform.position;
		transform.position = new Vector3(position.x, position.y, position.z) + playerOffset;
		transform.LookAt (playerController.transform);
	}
}
