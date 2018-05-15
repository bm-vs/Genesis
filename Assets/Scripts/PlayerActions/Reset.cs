using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset {
	private PlayerController player;
	private bool resetting;
	private Vector3 checkpoint;

	public Reset (PlayerController player) {
		this.player = player;
		this.resetting = false;
		this.checkpoint = player.transform.position;
	}

	public void setCheckpoint (Vector3 checkpoint) {
		this.checkpoint = checkpoint;
	}

	public void Input() {
		if (UnityEngine.Input.GetAxisRaw ("Reset") != 0) {
			resetting = true;
		}
	}

	public void Action() {
		if (resetting) {
			resetting = false;
			player.transform.position = checkpoint;
		}
	}

	public void Died() {
		resetting = true;
	}
}