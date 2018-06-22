using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {melee, ranged, hybrid}

public class RangedController : MonoBehaviour {
	private EnemyAnimationController animations;
	public GameObject particles;
	public GameObject mesh;

	// Vital
	private GameObject player;
	public EnemyType type;
	public float health;
	private float deathTimer;

	// Vision
	private Vector3 playerDirection;
	public bool onSight;
	public float visionRange;
	private float visionAngle;
	private Vector3 lastSeen;
	private float loseSightTimeout;
	public GameObject[] buddies;

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
	private float velocityY;

	// Damage received
	private bool hit;
	private float recoverTimer;

	// Attack
	private Object bulletPrefab;
	private float bulletSpeed;
	private float shootingCooldown;
	private float dashCooldown;
	private float dashRange;
	private float dashSpeed; 
	public bool dashing;
	private float dashDuration;

	// Sounds
	[FMODUnity.EventRef]
	public string attackSound;
	public float attackVolume;
	private FMOD.Studio.EventInstance attackEvent;

	[FMODUnity.EventRef]
	public string attackRangedSound;
	public float attackRangedVolume;
	private FMOD.Studio.EventInstance attackRangedEvent;

	[FMODUnity.EventRef]
	public string walkSound;
	public float walkVolume;
	private FMOD.Studio.EventInstance walkEvent;

	[FMODUnity.EventRef]
	public string damageSound;
	public float damageVolume;
	private FMOD.Studio.EventInstance damageEvent;

	[FMODUnity.EventRef]
	public string deathSound;
	public float deathVolume;
	private FMOD.Studio.EventInstance deathEvent;

	public float soundMaxDistance;

	void Start () {
		animations = gameObject.GetComponent<EnemyAnimationController> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		bulletPrefab = Resources.Load ("Prefabs/BulletEnemy");
		bulletSpeed = 40.0f;
		shootingCooldown = 1.0f;
		dashCooldown = 2.0f;
		dashRange = 8f;
		dashSpeed = 80.0f;
		visionAngle = 60.0f;
		lastSeen = Vector3.zero;
		hit = false;
		recoverTimer = 0.5f;
		previousPosition = transform.position;
		transitionTimer = 0.0f;
		loseSightTimeout = 0.0f;
		velocityY = 0.0f;
		deathTimer = 2.0f;

		if (type == EnemyType.hybrid) {
			health = 80.0f;
		} else if (type == EnemyType.melee) {
			health = 30.0f;
		} else if (type == EnemyType.ranged) {
			health = 20.0f;
		}

		attackEvent = FMODUnity.RuntimeManager.CreateInstance (attackSound);
		attackRangedEvent = FMODUnity.RuntimeManager.CreateInstance (attackRangedSound);
		walkEvent = FMODUnity.RuntimeManager.CreateInstance (walkSound);
		damageEvent = FMODUnity.RuntimeManager.CreateInstance (damageSound);
		deathEvent = FMODUnity.RuntimeManager.CreateInstance (deathSound);
	}

	void Update () {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (attackEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		attackEvent.setVolume (attackVolume);
		attackEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		FMODUnity.RuntimeManager.AttachInstanceToGameObject (attackRangedEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		attackRangedEvent.setVolume (attackRangedVolume);
		attackRangedEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		FMODUnity.RuntimeManager.AttachInstanceToGameObject (walkEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		walkEvent.setVolume (walkVolume);
		walkEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		FMODUnity.RuntimeManager.AttachInstanceToGameObject (damageEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		damageEvent.setVolume (damageVolume);
		damageEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		FMODUnity.RuntimeManager.AttachInstanceToGameObject (deathEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		deathEvent.setVolume (deathVolume);
		deathEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		if (health > 0.0f) {
			CheckPlayerOnSight ();
			DebugShowVision ();

			shootingCooldown -= Time.deltaTime;
			if (onSight && type != EnemyType.melee) {
				Shoot ();
			}
		}
	}

	void FixedUpdate () {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
		if (health > 0.0f) {
			if (lastSeen != Vector3.zero) {
				// Rotate towards target
				Vector3 target = new Vector3 (lastSeen.x, transform.position.y, lastSeen.z);
				transform.LookAt (target);
			}

			if (onSight) {
				dashCooldown -= Time.fixedDeltaTime;
			} else {
				dashCooldown = 0.5f;
			}

			if (dashing) {
				if (dashDuration <= 0.0f) {
					rigidbody.velocity = Vector3.zero;
					rigidbody.useGravity = true;
					dashing = false;
				} else {
					dashDuration -= Time.fixedDeltaTime;
				}
			} else if (!hit) {
				if (onSight) {
					AttackMovement ();
				} else if (anchorPoints.Length > 0) {
					PatrolMovement ();
				}

				previousPosition = transform.position;
			} else if (rigidbody.velocity == Vector3.zero) {
				RecoverFromAttack ();
			}

			float velF = gameObject.GetComponent<Rigidbody> ().velocity.y;
			if (velF - velocityY > 50.0f && velocityY < 0.0f && velF >= -1f) {
				updateHealth (-80.0f);
			}
			velocityY = velF;
		} else if (dashing) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.useGravity = true;
			dashing = false;
		} else {
			deathTimer -= Time.fixedDeltaTime;
			mesh.transform.position = mesh.transform.position - new Vector3 (0.0f, 0.02f, 0.0f);
			if (deathTimer <= 0.0f) {
				Destroy (gameObject);
			}
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (health > 0.0f) {
			PlayerController player = collision.collider.gameObject.GetComponent<PlayerController> ();
			if (player != null && player.dashing) {
				updateHealth (-30.0f);
				hit = true;
				if (!onSight && player != null) {
					Vector3 target = new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z);
					transform.LookAt (target);
				}
			}
		}
	}

	void OnTriggerEnter (Collider other) {
		if (health > 0.0f && other.gameObject != null) {
			if (other.gameObject.tag == "Shot" && other.gameObject.GetComponent<BulletController> ().owner.tag == "Player") {
				updateHealth (-20.0f);
				hit = true;
				if (!onSight && player != null) {
					Vector3 target = new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z);
					transform.LookAt (target);
				}
			}
		}
	}

	public void updateHealth (float value) {
		health += value;
		if (health <= 0.0f) {
			animations.TriggerTransition (animations.DEAD);
			deathEvent.start ();
			particles.SetActive (true);
		} else if (value <= 0.0f) {
			damageEvent.start ();
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
			DebugWalkOnGround (projection, direction);
			return (Vector3.Dot (projection, transform.rotation * direction) >= 0.01);
		}
	}


	void CheckPlayerOnSight () {
		playerDirection = player.transform.position - transform.position;

		if (loseSightTimeout > 0.0f) {
			lastSeen = player.transform.position;
			loseSightTimeout -= Time.deltaTime;
			onSight = true;
		} else {
			bool buddyOnSight = false;
			foreach (GameObject buddy in buddies) {
				if (buddy != null) {
					Vector3 buddyDirection = buddy.transform.position - transform.position;
					float buddyAngle = Vector3.Angle (buddyDirection, transform.forward);
					int layerMask = 1 << 11;
					layerMask = ~layerMask;
					RaycastHit hit;
					if (Physics.Raycast (transform.position, buddyDirection, out hit, visionRange, layerMask)) {
						if (buddy != null && hit.collider.gameObject.GetComponent<RangedController> () != null && (buddy.GetComponent<RangedController> ().health <= 0.0f || buddy.GetComponent<RangedController> ().onSight) && buddyAngle < visionAngle) {
							lastSeen = player.transform.position;
							onSight = true;
							buddyOnSight = true;
						}
					}
				}
			}

			if (!buddyOnSight) {
				float playerAngle = Vector3.Angle (playerDirection, transform.forward);
				int layerMask = 1 << 11;
				layerMask = ~layerMask;
				RaycastHit hit;
				if (Physics.Raycast (transform.position, playerDirection, out hit, visionRange, layerMask)) {
					if (hit.collider.gameObject.tag == "Player" && playerAngle < visionAngle) {
						lastSeen = hit.collider.gameObject.transform.position;
						loseSightTimeout = 3.0f;
						onSight = true;
					} else {
						onSight = false;
					}
				} else {
					onSight = false;
				}
			}
		}
	}

	void Shoot () {
		if (shootingCooldown < 0f) {
			GameObject bullet = (GameObject) GameObject.Instantiate (bulletPrefab);
			bullet.GetComponent<BulletController> ().owner = gameObject;
			bullet.GetComponent<BulletController> ().range = visionRange;
			bullet.transform.position = gameObject.transform.position;
			bullet.GetComponent<Rigidbody>().velocity = playerDirection.normalized * bulletSpeed;
			shootingCooldown = 1.0f;
			attackEvent.start ();
			animations.TriggerTransition (animations.SHOOT);
		}
	}

	void Dash (Vector3 movementDirection) {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
		rigidbody.velocity = movementDirection * dashSpeed;
		dashing = true;
		dashCooldown = 2.0f;
		dashDuration = 0.1f;
		rigidbody.useGravity = false;
		attackRangedEvent.start ();
		animations.TriggerTransition (animations.DASH);
	}

	void AttackMovement () {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
		Vector3 movementDirection = new Vector3 (playerDirection.x, 0.0f, playerDirection.z).normalized;
		if (type == EnemyType.ranged && playerDirection.magnitude <= moveBackRange && !IsGoingToFall(-movementDirection)) {
			rigidbody.velocity = -movementDirection * kiteSpeed;

			FMOD.Studio.PLAYBACK_STATE walkState;
			walkEvent.getPlaybackState (out walkState);
			if (walkState != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
				walkEvent.start ();
			}
		} else if (type != EnemyType.ranged && !IsGoingToFall(movementDirection)) {
			if (playerDirection.magnitude <= dashRange && dashCooldown < 0f) {
				Dash (playerDirection.normalized);
			}
			else {
				rigidbody.velocity = movementDirection * engageSpeed;
				walkEvent.start ();
				animations.TriggerTransition (animations.RUN);

				FMOD.Studio.PLAYBACK_STATE walkState;
				walkEvent.getPlaybackState (out walkState);
				if (walkState != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
					walkEvent.start ();
				}
			}
		}
	}

	void PatrolMovement () {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
		Vector3 pos = anchorPoints[positionIndex];

		// Change anchor point
		float error = 0.00001f;
		if ((previousPosition.x > pos.x + error && transform.position.x < pos.x - error) || (previousPosition.x < pos.x - error && transform.position.x > pos.x + error) || (previousPosition.z > pos.z + error && transform.position.z < pos.z - error) || (previousPosition.z < pos.z - error && transform.position.z > pos.z + error)) {
			if (transitionTimer <= 0.0f) {
				positionIndex = (positionIndex + 1) % anchorPoints.Length;
				pos = anchorPoints [positionIndex];
				transitionTimer = anchorTransitionTimer [positionIndex];
			}
		}

		// Move and rotate towards anchor point
		if (transitionTimer <= 0.0f) {
			Vector3 movementDirection = new Vector3 ((pos - transform.position).x, 0.0f, (pos - transform.position).z).normalized;
			if (!IsGoingToFall (movementDirection)) {
				Vector3 target = new Vector3 (pos.x, transform.position.y, pos.z);
				transform.LookAt (target);
				rigidbody.velocity = movementDirection * speed;
				animations.TriggerTransition (animations.WALK);

				FMOD.Studio.PLAYBACK_STATE walkState;
				walkEvent.getPlaybackState (out walkState);
				if (walkState != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
					walkEvent.start ();
				}
			} else {
				positionIndex = (positionIndex + 1) % anchorPoints.Length;
			}
		} else {
			transitionTimer -= Time.fixedDeltaTime;
			animations.TriggerTransition (animations.IDLE);
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

	void DebugShowVision() {
		Debug.DrawRay (transform.position, playerDirection.normalized * visionRange, Color.red);
		Debug.DrawRay (transform.position, Quaternion.AngleAxis(60, Vector3.up) * transform.forward * visionRange, Color.yellow);
		Debug.DrawRay (transform.position, Quaternion.AngleAxis(-60, Vector3.up) * transform.forward * visionRange, Color.yellow);
	}

	void DebugWalkOnGround(Vector3 projection, Vector3 direction) {
		Debug.DrawRay (transform.position + transform.rotation * direction, Vector3.up * Vector3.Dot (projection, transform.rotation * direction) * 10, Color.blue);
	}
}
