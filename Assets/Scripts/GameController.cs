using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public PlayerController playerController;
	public CameraController cameraController;

	void Start () {
		Physics.gravity = new Vector3 (0, -120f, 0);

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		GameObject camera = GameObject.FindGameObjectWithTag ("MainCamera");

		playerController = player.GetComponent<PlayerController> ();
		playerController.game = this;

		cameraController = camera.GetComponent<CameraController> ();
		cameraController.game = this;
		cameraController.playerOffset = camera.transform.position - player.transform.position;
	}
}
