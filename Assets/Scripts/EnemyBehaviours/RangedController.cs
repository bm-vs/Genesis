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

	// Damage received
	private bool hit;

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
		lastSeen = Vector3.zero;
		hit = false;
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
	}

	void FixedUpdate () {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();

		if (lastSeen != Vector3.zero) {
			// Rotate towards target
			Vector3 target = new Vector3 (lastSeen.x, transform.position.y, lastSeen.z);
			transform.LookAt (target);
		}

		// Dont move the character during the knockback of a hit
		if (!hit) {
			if (onSight) {
				// Kite player
				if (playerDirection.magnitude <= moveBackRange) {
					rigidbody.velocity = -new Vector3 (playerDirection.x, 0.0f, playerDirection.z).normalized * speed;
				}
			}
		} else if (rigidbody.velocity == Vector3.zero) {
			hit = false;
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.tag != "Terrain") {
			hit = true;

			// Rotate to side of colision 
			if (!onSight) {
				Vector3 target = new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z);
				transform.LookAt (target);
			}
		}
	}

	void Shoot (Vector3 playerOffset) {
		GameObject bullet = (GameObject) GameObject.Instantiate (bulletPrefab);
		bullet.GetComponent<BulletController> ().owner = gameObject;
		bullet.transform.position = gameObject.transform.position;
		bullet.GetComponent<Rigidbody>().velocity = playerOffset.normalized * bulletSpeed;
	}
}
