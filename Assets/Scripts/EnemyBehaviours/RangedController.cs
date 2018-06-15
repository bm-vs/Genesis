using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedController : MonoBehaviour {
	private GameObject player;

	public bool melee;

	// Vision
	private Vector3 playerDirection;
	private bool onSight;
	public float visionRange;
	private float visionAngle;
	private Vector3 lastSeen;

	// Movement
	public float moveBackRange;
	public float speed;
	public float kiteSpeed;
	public float engageSpeed;
	public Vector3[] anchorPoints;
	public float[] anchorTransitionTimer;
	private float transitionTimer;
	private Vector3 previousPosition;
	private int positionIndex;

	// Damage received
	private bool hit;
	private float recoverTimer;

	// Attack
	private Object bulletPrefab;
	private float bulletSpeed;
	private float shootingCooldown;
	private float dashCooldown;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		bulletPrefab = Resources.Load ("Prefabs/BulletEnemy");
		bulletSpeed = 40.0f;
		shootingCooldown = 1.0f;
		dashCooldown = 2.0f;
		visionAngle = 70.0f;
		lastSeen = Vector3.zero;
		hit = false;
		recoverTimer = 0.5f;
		previousPosition = transform.position;
		transitionTimer = 0.0f;
	}

	void Update () {
		CheckPlayerOnSight ();
		DebugShowVision ();
		if (onSight && !melee) {
			Shoot ();
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
				AttackMovement ();
			} else {
				PatrolMovement ();
			}

			previousPosition = transform.position;
		} else if (rigidbody.velocity == Vector3.zero) {
			RecoverFromAttack ();
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

	bool IsGoingToFall (Vector3 movementDirection) {
		Vector3 dim = gameObject.GetComponent<Renderer> ().bounds.size / 2;
		Vector3 x = new Vector3 (dim.x, 0.0f, 0.0f) * 3f;
		Vector3 z = new Vector3 (0.0f, 0.0f, dim.z) * 3f;

		return (
			IsGoingToFallInDirection (movementDirection, x, dim.y) ||
			IsGoingToFallInDirection (movementDirection, - x, dim.y) ||
			IsGoingToFallInDirection (movementDirection, z, dim.y) ||
			IsGoingToFallInDirection (movementDirection, - z, dim.y) ||
			IsGoingToFallInDirection (movementDirection, x + z, dim.y) ||
			IsGoingToFallInDirection (movementDirection, x - z, dim.y) ||
			IsGoingToFallInDirection (movementDirection, - x + z, dim.y) ||
			IsGoingToFallInDirection (movementDirection, - x - z, dim.y));
	}

	bool IsGoingToFallInDirection (Vector3 movementDirection, Vector3 direction, float height) {
		if (Physics.Raycast (transform.position + transform.rotation * direction, -Vector3.up, height + 0.01f)) {
			return false;
		} else {
			Vector3 projection = Vector3.Project (movementDirection, direction);
			Debug.DrawRay (transform.position + transform.rotation * direction, Vector3.up * Vector3.Dot (projection, transform.rotation * direction) * 10, Color.blue);
			Debug.Log (Vector3.Dot (projection, transform.rotation * direction));
			return (Vector3.Dot (projection, transform.rotation * direction) >= 0);
		}
	}


	void CheckPlayerOnSight () {
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
	}

	void DebugShowVision() {
		Debug.DrawRay (transform.position, playerDirection * visionRange, Color.red);
		Debug.DrawRay (transform.position, Quaternion.AngleAxis(80, Vector3.up) * transform.forward * visionRange, Color.yellow);
		Debug.DrawRay (transform.position, Quaternion.AngleAxis(-80, Vector3.up) * transform.forward * visionRange, Color.yellow);
	}

	void Shoot () {
		shootingCooldown -= Time.deltaTime;
		if (shootingCooldown < 0f && playerDirection.magnitude < visionRange) {
			GameObject bullet = (GameObject) GameObject.Instantiate (bulletPrefab);
			bullet.GetComponent<BulletController> ().owner = gameObject;
			bullet.transform.position = gameObject.transform.position;
			bullet.GetComponent<Rigidbody>().velocity = playerDirection.normalized * bulletSpeed;
			shootingCooldown = 1.0f;
		}
	}


	void AttackMovement () {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
		Vector3 movementDirection = new Vector3 (playerDirection.x, 0.0f, playerDirection.z).normalized;
		if (!melee && playerDirection.magnitude <= moveBackRange && !IsGoingToFall(-movementDirection)) {
			rigidbody.velocity = -movementDirection * kiteSpeed;
		} else if (melee && playerDirection.magnitude <= visionRange && !IsGoingToFall(movementDirection)) {
			rigidbody.velocity = movementDirection * engageSpeed;
		}
	}

	void PatrolMovement () {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
		Vector3 pos = anchorPoints[positionIndex];

		// Change anchor point
		if ((previousPosition.x > pos.x && transform.position.x < pos.x) || (previousPosition.x < pos.x && transform.position.x > pos.x) || (previousPosition.z > pos.z && transform.position.z < pos.z) || (previousPosition.z < pos.z && transform.position.z > pos.z)) {
			if (transitionTimer <= 0.0f) {
				positionIndex = (positionIndex + 1) % anchorPoints.Length;
				pos = anchorPoints [positionIndex];
				transitionTimer = anchorTransitionTimer [positionIndex];
			}
		}

		// Rotate towards anchor point
		Vector3 target = new Vector3 (pos.x, transform.position.y, pos.z);
		transform.LookAt (target);

		// Move towards anchor point
		if (transitionTimer <= 0.0f) {
			Vector3 movementDirection = new Vector3 ((pos - transform.position).x, 0.0f, (pos - transform.position).z).normalized;
			if (!IsGoingToFall(movementDirection)) {
				rigidbody.velocity = movementDirection * speed;
			}
		} else {
			transitionTimer -= Time.fixedDeltaTime;
		}
	}

	void RecoverFromAttack () {
		if (recoverTimer < 0.0f) {
			hit = false;
			recoverTimer = 0.5f;
		} else {
			recoverTimer -= Time.fixedDeltaTime;
		}
	}
}
