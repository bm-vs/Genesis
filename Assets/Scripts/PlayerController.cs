using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	// Sounds and animations
	public PlayerSoundsController sounds;
	public PlayerAnimationsController animations;

	// Health
	private float health;
	private float velocityY;
	private float timeLastHit;

	// Actions
	private Move move;
	private Jump jump;
	private ChangeForm form;
	private Shoot shoot;
	private Dash dash;
	private Reset reset;

	// Status
	public bool airborne;
	public bool shooting;
	public bool dashing;
	public bool jumping;
	public bool isHuman;
	public bool onLedge;
	public bool dead;
	public bool running;

	// Attributes
	public float y;
	public float z;

	public float moveSpeed;
	public float ledgeSpeed;
	public float grabLedgeOffset;
	public float jumpForce;
	public float jumpHoldMultiplier;
	public float jumpHoldDuration;
	public float changeFormCooldown;
	public float bulletSpeed;
	public float attackCooldown;
	public float dashSpeed;
	public float dashCooldown;
	public float dashDuration;

	void Start () {
		gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation; // disable rotation through physics

		sounds = gameObject.GetComponent<PlayerSoundsController> ();
		animations = gameObject.GetComponent<PlayerAnimationsController> ();
		health = 100.0f;

		airborne = false;
		shooting = false;
		dashing = false;
		jumping = false;
		isHuman = true;
		onLedge = false;
		dead = false;
		running = false;

		y = gameObject.GetComponent<Renderer> ().bounds.size.y;
		z = gameObject.GetComponent<Renderer> ().bounds.size.z;
		moveSpeed = 600.0f;
		ledgeSpeed = 300.0f;
		grabLedgeOffset = 0.5f;
		jumpForce = 32.0f;
		jumpHoldMultiplier = 1.5f;
		jumpHoldDuration = 1.0f;
		changeFormCooldown = 0.5f;
		bulletSpeed = 60.0f;
		attackCooldown = 0.5f;
		dashSpeed = 80.0f;
		dashCooldown = 0.5f;
		dashDuration = 0.1f;
		velocityY = 0.0f;

		move = new Move (this);
		jump = new Jump (this);
		form = new ChangeForm (this);
		shoot = new Shoot (this);
		dash = new Dash (this);
		reset = new Reset (this);
    }

	void Update () {
		if (!dead) {
			if (!dashing) {
				move.Input ();
				jump.Input ();
				form.Input ();
			}
			if (isHuman) {
				shoot.Input ();
			} else if (!isHuman && !onLedge) {
				dash.Input ();
			}
			reset.Input ();
			DebugLookDirection ();
		}
	}

	void FixedUpdate() {
		if (!dead) {
			airborne = IsAirborne ();
			if (!dashing) {
				move.Action ();
				jump.Action ();
			}
			if (!onLedge) {
				dash.Action ();
			}

			float velF = gameObject.GetComponent<Rigidbody> ().velocity.y;
			if (velF - velocityY > 80.0f && velocityY < 0.0f && velF >= -1f) {
				updateHealth (-((4.0f / 7.0f) * (velF - velocityY) - (250.0f / 7.0f)));
			}
			velocityY = velF;

			if (timeLastHit > 0.0f) {
				timeLastHit -= Time.fixedDeltaTime;
			}

			if (health < 100.0f & timeLastHit < 0.0f) {
				updateHealth (100.0f / (5.0f / Time.fixedDeltaTime));
			}
		}
		else if (dashing) {
			Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
			rigidbody.velocity = Vector3.zero;
			rigidbody.useGravity = true;
			dashing = false;
		}

		reset.Action ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Shot" && other.gameObject.GetComponent<BulletController> ().owner.tag != "Player") {
			updateHealth (-20.0f);
		} else if (other.gameObject.tag == "Fire") {
			updateHealth (-20.0f);
		} else if (other.gameObject.tag == "Checkpoint") {
			reset.setCheckpoint (other.gameObject.transform.position);
			other.gameObject.SetActive (false);
		} else if (other.gameObject.tag == "Water") {
			sounds.PlaySound (PlayerSounds.SPLASH);
		}
    }

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Wall") {
			move.CheckOnLedge (other);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Wall") {
			move.removePlayerOnLedge ();
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "FallPlane") {
			reset.Died ();
		} else {
			RangedController enemy = collision.collider.gameObject.GetComponent<RangedController> ();
			if (enemy != null && enemy.dashing) {
				if (enemy.type == EnemyType.hybrid) {
					updateHealth (-50.0f);
				} else {
					updateHealth (-35.0f);
				}
			}
		}
	}

	public void updateHealth (float value) {
		health += value;
		//Debug.Log (health);
		if (value < 0.0f) {
			timeLastHit = 3.0f;
		}
		if (health <= 0.0f) {
			health = 100.0f;
			reset.Died ();
		}
	}

	void DebugLookDirection () {
		Debug.DrawRay (transform.position, transform.forward * 35.0f, Color.magenta);
	}

	public bool IsAirborne() {
		Vector3 dim = gameObject.GetComponent<Renderer> ().bounds.size / 2;
		int layerMask = 1 << 11;
		layerMask = ~layerMask;
		/*
		Vector3 x = new Vector3 (dim.x, 0.0f, 0.0f);
		Vector3 z = new Vector3 (0.0f, 0.0f, dim.z);
		bool edge1 = Physics.Raycast (transform.position + x + z, -Vector3.up, dim.y + 0.01f);
		bool edge2 = Physics.Raycast (transform.position + x - z, -Vector3.up, dim.y + 0.01f);
		bool edge3 = Physics.Raycast (transform.position - x + z, -Vector3.up, dim.y + 0.01f);
		bool edge4 = Physics.Raycast (transform.position - x - z, -Vector3.up, dim.y + 0.01f);
		return !(edge1 || edge2 || edge3 || edge4);
		*/
		return !Physics.Raycast (transform.position, -Vector3.up, dim.y + 0.01f, layerMask);
	}
}
