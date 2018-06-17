using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move {
	private PlayerController player;
	private Vector3 direction;
	private Vector3 ledgePosition;
	private Vector3 ledgeNormal;
	private Vector3 move;

	public Move (PlayerController player) {
		this.player = player;
	}

	public void Input() {
		bool stop = player.running;
		bool go = player.running;

		direction = new Vector3 (UnityEngine.Input.GetAxis ("Horizontal"), 0f, UnityEngine.Input.GetAxis ("Vertical"));
		Transform cameraTransform = Camera.main.transform;
		Vector3 cameraRight = (new Vector3 (cameraTransform.right.x, 0f, cameraTransform.right.z)).normalized;
		Vector3 cameraForward = (new Vector3 (cameraTransform.forward.x, 0f, cameraTransform.forward.z)).normalized;
		move = (cameraRight * direction.x + cameraForward * direction.z) * Time.deltaTime * player.moveSpeed;

		player.running = move.magnitude > 0.5f;
		stop = stop && !player.running;
		go = !go && player.running;
			
		controlAnimation (go, stop);
	}

	public void Action() {
		Rigidbody rigidbody = player.gameObject.GetComponent<Rigidbody> ();
		if (player.onLedge) {
			rigidbody.velocity = Vector3.Project (new Vector3 (move.x, rigidbody.velocity.y, move.z), Vector3.Cross (ledgeNormal, Vector3.up));
			if (!player.sounds.CheckIfPlaying(PlayerSounds.CLIMB) && rigidbody.velocity.magnitude > 0.5f) {
				player.sounds.PlaySound (PlayerSounds.CLIMB);
			}
		} else {
			rigidbody.velocity = new Vector3 (move.x, rigidbody.velocity.y, move.z);
			player.gameObject.transform.LookAt (player.gameObject.transform.position + new Vector3(move.x, 0.0f, move.z));
			if (!player.sounds.CheckIfPlaying(PlayerSounds.STEPS) && player.running && !player.airborne) {
				player.sounds.PlaySound (PlayerSounds.STEPS);
			}

		}
	}

	public void CheckOnLedge(Collider collider) {
		if (!player.isHuman && player.gameObject.GetComponent<Rigidbody> ().velocity.y <= 0){
			MeshFilter filter = collider.gameObject.GetComponent<MeshFilter> ();
			ledgeNormal = filter.transform.TransformDirection(filter.mesh.normals [0]);
			ledgePosition = collider.gameObject.transform.position;
			setPlayerOnLedge ();
		} else {
			removePlayerOnLedge ();
		}
	}

	public void setPlayerOnLedge() {
		player.onLedge = true;
		player.gameObject.layer = 9; //PlayerLedge
		player.gameObject.GetComponent<Rigidbody> ().constraints |= RigidbodyConstraints.FreezePositionY;
		player.gameObject.transform.forward = - ledgeNormal.normalized;
		if (ledgeNormal.x != 0) {
			player.gameObject.transform.position = new Vector3 (ledgePosition.x + ledgeNormal.x * player.z / 2.0f, player.gameObject.transform.position.y, player.gameObject.transform.position.z);
		} else if (ledgeNormal.z != 0) {
			player.gameObject.transform.position = new Vector3 (player.gameObject.transform.position.x, player.gameObject.transform.position.y, ledgePosition.z + ledgeNormal.z * player.z / 2.0f);
		}
	}

	public void removePlayerOnLedge() {
		player.onLedge = false;
		player.gameObject.GetComponent<Rigidbody> ().constraints &= ~RigidbodyConstraints.FreezePositionY;
		player.gameObject.layer = 0; //Default
	}

	public void controlAnimation(bool go, bool stop) {
		if (!player.airborne) {
			if (stop) {
				player.animations.TriggerTransition (player.animations.RUN_STOP);
			}
			else if (go) {
				player.animations.TriggerTransition (player.animations.RUN);
			}
		}
	}
}
