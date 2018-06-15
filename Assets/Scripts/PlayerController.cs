using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
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
		moveSpeed = 600.0f;
		ledgeSpeed = 300.0f;
		grabLedgeOffset = 0.5f;
		jumpForce = 32.0f;
		jumpHoldMultiplier = 1.5f;
		jumpHoldDuration = 1.0f;
		changeFormCooldown = 0.5f;
		bulletSpeed = 60.0f;
		attackCooldown = 0.3f;
		dashSpeed = 80.0f;
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

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "FallPlane") {
			reset.Died ();
		} else if (other.gameObject.tag == "Checkpoint") {
			reset.setCheckpoint (other.gameObject.transform.position);
			other.gameObject.SetActive (false);
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

	public bool IsAirborne() {
		int layerMask = 1 << 8;
		layerMask = ~layerMask;
		return !Physics.Raycast (transform.position, -Vector3.up, height / 2 + 0.01f, layerMask);
	}

	public void PlaySound(string type) {
		AudioSource[] sounds = gameObject.GetComponents<AudioSource> ();

		switch (type) {
		case "Walk":
			sounds [0].Play ();
			break;
		case "JumpStart":
			sounds [1].Play ();
			break;
		case "JumpImpact":
			sounds [2].Play ();
			break;
		case "MonkeyAttack":
			sounds [3].Play ();
			break;
		}
	}

	public void StopSound(string type) {
		AudioSource[] sounds = gameObject.GetComponents<AudioSource> ();

		switch (type) {
		case "Walk":
			sounds [0].Stop ();
			break;
		case "JumpStart":
			sounds [1].Stop ();
			break;
		case "JumpImpact":
			sounds [2].Stop ();
			break;
		case "MonkeyAttack":
			sounds [3].Stop ();
			break;
		}
	}

	public bool isPlayingSound(string type) {
		AudioSource[] sounds = gameObject.GetComponents<AudioSource> ();

		switch (type) {
		case "Walk":
			return sounds [0].isPlaying;
		case "JumpStart":
			return sounds [1].isPlaying;
		case "JumpImpact":
			return sounds [2].isPlaying;
		case "MonkeyAttack":
			return sounds [3].isPlaying;
		}

		return false;
	}
}
