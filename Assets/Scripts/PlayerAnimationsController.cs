using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour {
	static Animator anim;

	public string JUMP = "Jump";
	public string JUMP_RUN = "JumpRun";
	public string JUMP_LANDED = "JumpLanded";
	public static string RUN_FORWARD = "RunForward";
	public static string RUN_BACKWARD = "RunBackward";
	public static string RUN_LEFT = "RunLeft";
	public static string RUN_RIGHT = "RunRight";
	public string RUN_STOP = "RunStop";

	private string runDirection;

	void Start () {
		anim = gameObject.GetComponent<Animator> ();
	}

	public void TriggerTransition (string animation) {
		anim.SetTrigger (animation);
		if (animation == RUN_STOP) {
			runDirection = "";
			anim.ResetTrigger (RUN_FORWARD);
			anim.ResetTrigger (RUN_LEFT);
			anim.ResetTrigger (RUN_RIGHT);
			anim.ResetTrigger (RUN_BACKWARD);
		}
	}

	public void TriggerTransitionRun (Vector3 forward, Vector3 move) {
		string direction = null;
		float angle = Vector3.SignedAngle(forward, move, Vector3.up);

		if (Math.Abs (angle) >= 0f && Math.Abs (angle) <= 45f) {
			direction = PlayerAnimationsController.RUN_FORWARD;
		} else if (angle >= 45f && angle <= 135f) {
			direction = PlayerAnimationsController.RUN_LEFT;
		} else if (angle <= -45f && angle >= -135f) {
			direction = PlayerAnimationsController.RUN_RIGHT;
		} else if (Math.Abs (angle) >= 135f && Math.Abs (angle) <= 180f) {
			direction = PlayerAnimationsController.RUN_BACKWARD;
		}


		if (direction != runDirection) {
			TriggerTransition (direction);
			runDirection = direction;
		}
	}

}
