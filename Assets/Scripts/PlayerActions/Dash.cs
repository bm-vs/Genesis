using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash {
	private PlayerController player;
	private float cooldownTimer;
	private float durationTimer;
	private Vector3 direction;
	private bool startDash;

	public Dash (PlayerController player) {
		this.player = player;
		this.cooldownTimer = 0;
		this.durationTimer = 0;
		this.startDash = false;
	}

	public void Input () {
		float currTime = Time.time;
		float analogHorizontal = UnityEngine.Input.GetAxis ("ShootingHorizontal");
		float analogVertical = UnityEngine.Input.GetAxis ("ShootingVertical");
		float mouseButton = UnityEngine.Input.GetAxisRaw ("ShootingMouse");

		if (currTime - cooldownTimer >= player.dashCooldown && (analogHorizontal != 0 || analogVertical != 0 || mouseButton != 0)) {
			cooldownTimer = currTime;
			startDash = true;
			player.dashing = true;
			if (analogHorizontal != 0 || analogVertical != 0) {
				setDashDirectionAnalog (analogHorizontal, analogVertical);
			} else {
				setDashDirectionMouse ();
			}
		}
	}

	public void Action () {
		float currTime = Time.time;
		Rigidbody rb = player.gameObject.GetComponent<Rigidbody> ();
		if (startDash) {
			startDash = false;
			rb.useGravity = false;
			durationTimer = currTime;
			rb.velocity = direction * player.dashSpeed;
			player.gameObject.transform.LookAt (player.gameObject.transform.position + new Vector3(direction.x, 0.0f, direction.z));
			player.sounds.PlaySound (PlayerSounds.DASH_START);
			player.animations.TriggerTransition (player.animations.DASH);
		}

		if (player.dashing && currTime - durationTimer >= player.dashDuration) {
			player.dashing = false;
			rb.velocity = Vector3.zero;
			rb.useGravity = true;
			if (player.running) {
				player.animations.TriggerTransitionRun (player.moveDirection, player.transform.forward, player.isHuman, true);
			}
		}
	}

	private void setDashDirectionAnalog (float analogHorizontal, float analogVertical) {
		Transform cameraTransform = Camera.main.transform;
		Vector3 forward = new Vector3 (cameraTransform.forward.x, 0f, cameraTransform.forward.z);
		Vector3 right = new Vector3 (cameraTransform.right.x, 0f, cameraTransform.right.z);
		direction = (forward.normalized * analogVertical + right.normalized * analogHorizontal).normalized;
	}

	private void setDashDirectionMouse () {
		Plane plane = new Plane (Vector3.up, player.transform.position);
		Ray ray = Camera.main.ScreenPointToRay (UnityEngine.Input.mousePosition);
		float distance;
		if (plane.Raycast (ray, out distance)) {
			Vector3 hitPoint = ray.GetPoint (distance);
			direction = (hitPoint-player.transform.position).normalized;
		}
	}
}
