using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour {
	public Animator anim;

	public string SHOOT = "Shoot";
	public string WALK = "Walk";
	public string IDLE = "Idle";
	public string DASH = "Dash";
	public string RUN = "Run";
	public string DEAD = "Dead";

	private bool walking = false;
	private bool idling = true;
	private bool running = false;

	public void TriggerTransition (string animation) {
		if ((animation == WALK && walking) || (animation == IDLE && idling) || (animation == RUN && running)){
			return;
		}

		anim.SetTrigger (animation);
		walking = animation == WALK;
		idling = animation == IDLE;
		running = animation == RUN;
	}
}
