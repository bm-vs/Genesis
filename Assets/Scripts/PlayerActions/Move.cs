using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move {
	private PlayerController player;
	private Vector3 direction;
	private Vector3 ledgePosition;
	private Vector3 ledgeNormal;

	public Move (PlayerController player) {
		this.player = player;
	}

	public void Input() {
		direction = new Vector3 (UnityEngine.Input.GetAxis ("Horizontal"), 0f, UnityEngine.Input.GetAxis ("Vertical"));
	}

	public void Action() {
		Rigidbody rigidbody = player.gameObject.GetComponent<Rigidbody> ();

		Transform cameraTransform = Camera.main.transform;
		Vector3 cameraRight = (new Vector3 (cameraTransform.right.x, 0f, cameraTransform.right.z)).normalized;
		Vector3 cameraForward = (new Vector3 (cameraTransform.forward.x, 0f, cameraTransform.forward.z)).normalized;
		player.moveDirection = (cameraRight * direction.x + cameraForward * direction.z) * Time.fixedDeltaTime * (player.onLedge ? player.ledgeSpeed : player.moveSpeed);

		if (player.onLedge) {
			rigidbody.velocity = Vector3.Project (new Vector3 (player.moveDirection.x, rigidbody.velocity.y, player.moveDirection.z), Vector3.Cross (ledgeNormal, Vector3.up));
			if (!player.sounds.CheckIfPlaying(PlayerSounds.CLIMB) && rigidbody.velocity.magnitude > 0.5f) {
				player.sounds.PlaySound (PlayerSounds.CLIMB);
			}
		} else {
			rigidbody.velocity = new Vector3 (player.moveDirection.x, rigidbody.velocity.y, player.moveDirection.z);
			player.gameObject.transform.LookAt (player.gameObject.transform.position + new Vector3(player.moveDirection.x, 0.0f, player.moveDirection.z));

			if (player.onMetal) {
				if (!player.sounds.CheckIfPlaying(PlayerSounds.STEPS_METAL) && player.running && !player.airborne) {
					player.sounds.PlaySound (PlayerSounds.STEPS_METAL);
				}
			} else if (!player.sounds.CheckIfPlaying(PlayerSounds.STEPS) && player.running && !player.airborne) {
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
		if (!player.onLedge) {
			player.animations.TriggerTransition (player.animations.HANG);
			player.hangStopped = true;
		}

		player.onLedge = true;
		player.gameObject.layer = 9; //PlayerLedge
		player.gameObject.GetComponent<Rigidbody> ().constraints |= RigidbodyConstraints.FreezePositionY;
		player.gameObject.transform.forward = - ledgeNormal.normalized;
		Debug.Log (player.z);
		if (ledgeNormal.x != 0) {
			player.gameObject.transform.position = new Vector3 (ledgePosition.x + ledgeNormal.x * player.z * player.ledgeHelper, ledgePosition.y - 1.0f, player.gameObject.transform.position.z);
		} else if (ledgeNormal.z != 0) {
			player.gameObject.transform.position = new Vector3 (player.gameObject.transform.position.x, ledgePosition.y - 1.0f, ledgePosition.z + ledgeNormal.z * player.z);
		}
	}

	public void removePlayerOnLedge() {
		player.onLedge = false;
		player.gameObject.GetComponent<Rigidbody> ().constraints &= ~RigidbodyConstraints.FreezePositionY;
		player.gameObject.layer = 0; //Default
		if (!player.jumping) {
			player.animations.TriggerTransition (player.animations.JUMP);
			player.jumping = true;
		}
	}
}
