using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public GameController game;

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

	// Attributes
	public float height;
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

		airborne = false;
		shooting = false;
		dashing = false;
		jumping = false;
		isHuman = true;
		onLedge = false;

		height = gameObject.GetComponent<Renderer> ().bounds.size.y;
		moveSpeed = 150.0f;
		ledgeSpeed = 75.0f;
		grabLedgeOffset = 0.5f;
		jumpForce = 8.0f;
		jumpHoldMultiplier = 1.5f;
		jumpHoldDuration = 1.0f;
		changeFormCooldown = 0.5f;
		bulletSpeed = 15.0f;
		attackCooldown = 0.3f;
		dashSpeed = 20.0f;
		dashCooldown = 0.5f;
		dashDuration = 0.1f;

		move = new Move (this);
		jump = new Jump (this);
		form = new ChangeForm (this);
		shoot = new Shoot (this);
		dash = new Dash (this);
		reset = new Reset (this);
	}

	void Update () {
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
	}

	void FixedUpdate() {
		airborne = IsAirborne ();
		if (!dashing) {
			move.Action (Time.deltaTime);
			jump.Action ();
		}
		if (!onLedge) {
			dash.Action ();
		}
		reset.Action ();
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "FallPlane") {
			reset.Died ();
		}
	}

	void OnCollisionStay(Collision other) {
		if (other.gameObject.tag == "Wall") {
			move.CheckOnLedge (other);
		}
	}

	void OnCollisionLeave(Collision other) {
		move.removePlayerOnLedge ();
	}

	public bool IsAirborne() {
		return !Physics.Raycast (transform.position, -Vector3.up, height / 2 + 0.01f);
	}
}
