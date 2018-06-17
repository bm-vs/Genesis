using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot {
	private PlayerController player;
	private float timer;
	private Object bulletPrefab;

	public Shoot (PlayerController player) {
		this.player = player;
		this.timer = 0;
		this.bulletPrefab = Resources.Load ("Prefabs/Bullet");
	}

	public void Input () {
		float currTime = Time.time;
		float analogHorizontal = UnityEngine.Input.GetAxis ("ShootingHorizontal");
		float analogVertical = UnityEngine.Input.GetAxis ("ShootingVertical");
		float mouseButton = UnityEngine.Input.GetAxisRaw ("ShootingMouse");

		Vector3 direction;
		if (analogHorizontal != 0 || analogVertical != 0) {
			direction = getBulletDirectionAnalog(analogHorizontal, analogVertical);
		}
		else {
			direction = getBulletDirectionMouse();
		}
		player.gameObject.transform.LookAt (player.gameObject.transform.position + new Vector3(direction.x, 0.0f, direction.z));

		if (currTime - timer >= player.attackCooldown && (analogHorizontal != 0 || analogVertical != 0 || mouseButton != 0)) {
			timer = currTime;
			player.shooting = true;

			GameObject bullet = (GameObject) GameObject.Instantiate (bulletPrefab);
			bullet.GetComponent<BulletController> ().owner = player.gameObject;
			bullet.transform.position = player.gameObject.transform.position;
			bullet.GetComponent<Rigidbody>().velocity = direction * player.bulletSpeed;
			player.sounds.PlaySound (PlayerSounds.SHOOT);
		}
	}

	private Vector3 getBulletDirectionAnalog(float analogHorizontal, float analogVertical) {
		Transform cameraTransform = Camera.main.transform;
		Vector3 forward = new Vector3 (cameraTransform.forward.x, 0f, cameraTransform.forward.z);
		Vector3 right = new Vector3 (cameraTransform.right.x, 0f, cameraTransform.right.z);
		Vector3 direction = (forward.normalized * analogVertical + right.normalized * analogHorizontal).normalized;
		return direction;
	}

	// Sets default bullet initial speed when shooting with mouse
	private Vector3 getBulletDirectionMouse() {
		Plane plane = new Plane (Vector3.up, player.transform.position);
		Ray ray = Camera.main.ScreenPointToRay (UnityEngine.Input.mousePosition);
		float distance;
		if (plane.Raycast (ray, out distance)) {
			Vector3 hitPoint = ray.GetPoint (distance);
			Vector3 direction = (hitPoint-player.transform.position).normalized;
			return direction;
		}

		return Vector3.zero;
	}
}
