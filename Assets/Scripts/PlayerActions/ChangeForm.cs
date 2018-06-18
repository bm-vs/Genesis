using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeForm {
	private PlayerController player;
	private float timer;
	private Material humanMaterial;
	private Material monkeyMaterial;

	public ChangeForm(PlayerController player) {
		this.player = player;
		this.timer = 0;
		this.humanMaterial = Resources.Load ("Materials/HumanMaterial", typeof(Material)) as Material;
		this.monkeyMaterial = Resources.Load ("Materials/MonkeyMaterial", typeof(Material)) as Material;
	}

	public void Input() {
		float currTime = Time.time;
		if (UnityEngine.Input.GetAxisRaw ("ChangeForm") != 0 && currTime - timer >= player.changeFormCooldown) {
			timer = currTime;
			player.isHuman = !player.isHuman;
			/*
			if (player.isHuman) {
				player.joints.GetComponent<SkinnedMeshRenderer> ().rootBone = player.humanBody.transform;
				player.surface.GetComponent<SkinnedMeshRenderer> ().rootBone = player.humanBody.transform;
			}
			else {
				player.joints.GetComponent<SkinnedMeshRenderer> ().rootBone = player.monkeyBody.transform;
				player.surface.GetComponent<SkinnedMeshRenderer> ().rootBone = player.monkeyBody.transform;
			}
			*/

			if (!player.running) {
				player.animations.TriggerTransition (player.animations.CHANGE_FORM);
			}
		}
	}
}
