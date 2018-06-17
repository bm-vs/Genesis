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
	public static string RUN_MONKEY = "RunMonkey";
	public string RUN_STOP = "RunStop";
	public string RUN_STOP_MONKEY = "RunStopMonkey";
	public string CHANGE_FORM = "ChangeForm";
	public string DASH = "Dash";

	private string runDirection;

	void Start () {
		anim = gameObject.GetComponent<Animator> ();
	}

	public void TriggerTransitionSame (string animation) {
		anim.SetTrigger (animation);
		if (animation == JUMP || animation == JUMP_RUN) {
			anim.ResetTrigger (JUMP_LANDED);
		}

		Debug.Log (animation);
	}

	public void TriggerTransitionDiff (string animation) {
		anim.SetTrigger (animation);
		if (animation == RUN_STOP || animation == RUN_STOP_MONKEY) {
			runDirection = "";
			anim.ResetTrigger (RUN_FORWARD);
			anim.ResetTrigger (RUN_LEFT);
			anim.ResetTrigger (RUN_RIGHT);
			anim.ResetTrigger (RUN_BACKWARD);
			anim.ResetTrigger (RUN_MONKEY);
		}
		if (animation == JUMP || animation == JUMP_RUN) {
			anim.ResetTrigger (JUMP_LANDED);
		}

		Debug.Log (animation);
	}

	public void TriggerTransitionRun (Vector3 forward, Vector3 move, bool isHuman, bool afterJump) {
		string direction = null;
		if (isHuman) {
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
		} else {
			direction = PlayerAnimationsController.RUN_MONKEY;
		}

		if (direction != runDirection || afterJump) {
			TriggerTransitionDiff (direction);
			runDirection = direction;
		}
	}
}
