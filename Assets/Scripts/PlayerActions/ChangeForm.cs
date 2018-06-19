using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeForm {
	private PlayerController player;
	private float timer;
	private Material humanMaterial;
	private Material humanJointsMaterial;
	private Material monkeyMaterial;
	private Material monkeyJointsMaterial;

	public ChangeForm(PlayerController player) {
		this.player = player;
		this.timer = 0;
		this.humanMaterial = Resources.Load ("Materials/HumanMaterial", typeof(Material)) as Material;
		this.humanJointsMaterial = Resources.Load ("Materials/HumanJointsMaterial", typeof(Material)) as Material;
		this.monkeyMaterial = Resources.Load ("Materials/MonkeyMaterial", typeof(Material)) as Material;
		this.monkeyJointsMaterial = Resources.Load ("Materials/MonkeyJointsMaterial", typeof(Material)) as Material;
	}

	public void Input() {
		float currTime = Time.time;
		if (UnityEngine.Input.GetAxisRaw ("ChangeForm") != 0 && currTime - timer >= player.changeFormCooldown) {
			timer = currTime;
			player.isHuman = !player.isHuman;
			player.sounds.PlaySound (PlayerSounds.TRANSFORM);
			if (player.isHuman) {
				player.joints.GetComponent<Renderer> ().material = humanJointsMaterial;
				player.surface.GetComponent<Renderer> ().material = humanMaterial;
			} else {
				player.joints.GetComponent<Renderer> ().material = monkeyJointsMaterial;
				player.surface.GetComponent<Renderer> ().material = monkeyMaterial;
			}

			if (!player.running) {
				player.animations.TriggerTransition (player.animations.CHANGE_FORM);
			}
		}
	}
}
