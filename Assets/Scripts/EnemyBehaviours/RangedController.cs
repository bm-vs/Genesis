using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedController : MonoBehaviour {
	private GameObject player;

	// Vision
	private Vector3 playerDirection;
	private bool onSight;
	public float visionRange;
	private float visionAngle;
	private Vector3 lastSeen;

	private Object bulletPrefab;
	private float bulletSpeed;
	private float shootingCooldown;
	public float moveBackRange;
	public float speed;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		bulletPrefab = Resources.Load ("Prefabs/BulletEnemy");
		bulletSpeed = 40.0f;
		shootingCooldown = 1.0f;
		visionAngle = 45.0f;
	}

	void Update () {
		playerDirection = player.transform.position - transform.position;
		float playerAngle = Vector3.Angle (playerDirection, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, playerDirection, out hit)) {
			if (hit.collider.gameObject.tag == "Player" && hit.distance < visionRange && playerAngle < visionAngle) {
				lastSeen = hit.collider.gameObject.transform.position;
				onSight = true;
			}
			else {
				onSight = false;
			}
		}

		// Shoot
		if (onSight) {
			shootingCooldown -= Time.deltaTime;
			if (shootingCooldown < 0f && playerDirection.magnitude < visionRange) {
				Shoot (playerDirection);
				shootingCooldown = 1.0f;
			}
		}

		//MoveBack (-playerOffset);


	}

	void FixedUpdate () {
		// Rotate towards target
		if (onSight) {
			Vector3 target = new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z);
			transform.LookAt (target);
		}
	}

	void MoveBack (Vector3 playerOffset) {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
		if (playerOffset.magnitude <= moveBackRange) {
			rigidbody.velocity = playerOffset.normalized * speed;
		}
	}

	void Shoot (Vector3 playerOffset) {
		GameObject bullet = (GameObject) GameObject.Instantiate (bulletPrefab);
		bullet.GetComponent<BulletController> ().owner = gameObject;
		bullet.transform.position = gameObject.transform.position;
		bullet.GetComponent<Rigidbody>().velocity = playerOffset.normalized * bulletSpeed;
	}
}
