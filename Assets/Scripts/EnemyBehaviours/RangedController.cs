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

	// Movement
	public float moveBackRange;
	public float speed;
	public Vector3[] anchorPoints;
	private Vector3 previousPosition;
	private int positionIndex;

	// Damage received
	private bool hit;
	private float recoverTimer;

	// Bullet
	private Object bulletPrefab;
	private float bulletSpeed;
	private float shootingCooldown;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		bulletPrefab = Resources.Load ("Prefabs/BulletEnemy");
		bulletSpeed = 40.0f;
		shootingCooldown = 1.0f;
		visionAngle = 55.0f;
		lastSeen = Vector3.zero;
		hit = false;
		recoverTimer = 0.5f;
		previousPosition = transform.position;
	}

	void Update () {
		playerDirection = player.transform.position - transform.position;
		float playerAngle = Vector3.Angle (playerDirection, transform.forward);
		int layerMask = 1 << 11;
		layerMask = ~layerMask;
		RaycastHit hit;
		if (Physics.Raycast (transform.position, playerDirection, out hit, visionRange, layerMask)) {
			if (hit.collider.gameObject.tag == "Player" && playerAngle < visionAngle) {
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

		// Dont move the character during the knockback and recover time of a hit
		if (!hit) {
			if (onSight) {
				// Kite movement
				if (playerDirection.magnitude <= moveBackRange && OnGround()) {
					rigidbody.velocity = -new Vector3 (playerDirection.x, 0.0f, playerDirection.z).normalized * speed;
				}
			} else {
				// Patrol movement
				Vector3 pos = anchorPoints[positionIndex];
				if ((previousPosition.x > pos.x && transform.position.x < pos.x) || (previousPosition.x < pos.x && transform.position.x > pos.x) || (previousPosition.z > pos.z && transform.position.z < pos.z) || (previousPosition.z < pos.z && transform.position.z > pos.z)) {
					positionIndex = (positionIndex + 1) % anchorPoints.Length;
					pos = anchorPoints [positionIndex];
				}

				Vector3 anchorDirection = pos - transform.position;
				rigidbody.velocity = new Vector3 (anchorDirection.x, 0.0f, anchorDirection.z).normalized * speed * 1.5f;
				Vector3 target = new Vector3 (anchorDirection.x, transform.position.y, anchorDirection.z);
				transform.LookAt (target);
			}

			previousPosition = transform.position;
		} else if (rigidbody.velocity == Vector3.zero) {
			if (recoverTimer < 0.0f) {
				hit = false;
				recoverTimer = 0.5f;
			} else {
				recoverTimer -= Time.fixedDeltaTime;
			}
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

	public bool OnGround() {
		Vector3 dim = gameObject.GetComponent<Renderer> ().bounds.size / 2;
		Vector3 x = new Vector3 (dim.x, 0.0f, 0.0f) * 3;
		Vector3 z = new Vector3 (0.0f, 0.0f, dim.z) * 3;
		bool edge1 = Physics.Raycast (transform.position + x + z, -Vector3.up, dim.y + 0.01f);
		bool edge2 = Physics.Raycast (transform.position + x - z, -Vector3.up, dim.y + 0.01f);
		bool edge3 = Physics.Raycast (transform.position - x + z, -Vector3.up, dim.y + 0.01f);
		bool edge4 = Physics.Raycast (transform.position - x - z, -Vector3.up, dim.y + 0.01f);

		return (edge1 && edge2 && edge3 && edge4);
	}

	void Shoot (Vector3 playerOffset) {
		GameObject bullet = (GameObject) GameObject.Instantiate (bulletPrefab);
		bullet.GetComponent<BulletController> ().owner = gameObject;
		bullet.transform.position = gameObject.transform.position;
		bullet.GetComponent<Rigidbody>().velocity = playerOffset.normalized * bulletSpeed;
	}
}
