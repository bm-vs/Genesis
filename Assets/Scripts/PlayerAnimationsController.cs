using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour {
	static Animator anim;

	public string JUMP = "Jump";
	public string JUMP_RUN = "JumpRun";
	public string JUMP_LANDED = "JumpLanded";
	public string RUN = "Run";
	public string RUN_STOP = "RunStop";

	void Start () {
		anim = gameObject.GetComponent<Animator> ();
	}

	public void TriggerTransition (string animation) {
		anim.SetTrigger (animation);
	}

}
