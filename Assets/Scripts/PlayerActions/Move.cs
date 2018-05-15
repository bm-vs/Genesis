using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move {
	private PlayerController player;
	private Vector3 direction;
	private Vector3 ledgeNormal;

	public Move (PlayerController player) {
		this.player = player;
	}

	public void Input() {
		direction = new Vector3 (UnityEngine.Input.GetAxis ("Horizontal"), 0f, UnityEngine.Input.GetAxis ("Vertical"));
	}

	public void Action(float deltaTime) {
		Transform cameraTransform = Camera.main.transform;
		Vector3 cameraRight = (new Vector3 (cameraTransform.right.x, 0f, cameraTransform.right.z)).normalized;
		Vector3 cameraForward = (new Vector3 (cameraTransform.forward.x, 0f, cameraTransform.forward.z)).normalized;
		Vector3 move = (cameraRight * direction.x + cameraForward * direction.z) * deltaTime * player.moveSpeed;
		Rigidbody rigidbody = player.gameObject.GetComponent<Rigidbody> ();
		if (player.onLedge) {
			rigidbody.velocity = Vector3.Project (new Vector3 (move.x, rigidbody.velocity.y, move.z), Vector3.Cross (ledgeNormal, Vector3.up));
		} else {
			rigidbody.velocity = new Vector3 (move.x, rigidbody.velocity.y, move.z);
		}

		if ((player.isPlayingSound ("Walk") && rigidbody.velocity.magnitude == 0) || player.airborne || player.onLedge) {
			player.StopSound ("Walk");
		} else if (!player.isPlayingSound ("Walk") && rigidbody.velocity.magnitude != 0 && !player.airborne && !player.onLedge) {
			player.PlaySound ("Walk");
		}
	}

	public void CheckOnLedge(Collider collider) {
		if (!player.isHuman && player.gameObject.GetComponent<Rigidbody> ().velocity.y <= 0){
			MeshFilter filter = collider.gameObject.GetComponent<MeshFilter> ();
			ledgeNormal = filter.transform.TransformDirection(filter.mesh.normals [0]);
			setPlayerOnLedge ();
		} else {
			removePlayerOnLedge ();
		}
	}

	public void setPlayerOnLedge() {
		player.onLedge = true;
		player.gameObject.GetComponent<Rigidbody> ().constraints |= RigidbodyConstraints.FreezePositionY;
		player.gameObject.layer = 9; //PlayerLedge
	}

	public void removePlayerOnLedge() {
		player.onLedge = false;
		player.gameObject.GetComponent<Rigidbody> ().constraints &= ~RigidbodyConstraints.FreezePositionY;
		player.gameObject.layer = 0; //Default
	}
}
