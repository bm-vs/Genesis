using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset {
	private PlayerController player;
	private Vector3 checkpoint;
    private GameObject lastLevel;
    private GameObject level;
    private Transform game; // to add object as its children
	private float dyingTimeout;

	public bool generator0 = false;
	public bool generator1 = false;
	public bool generator2 = false;

    public Reset (PlayerController player) {
		this.player = player;
		this.dyingTimeout = 4.0f;
		this.checkpoint = player.transform.position;
        this.game = GameObject.FindGameObjectWithTag("Game").transform;
        this.level = GameObject.FindGameObjectWithTag("Level");
        this.lastLevel = GameObject.Instantiate(this.level, this.game);
        this.lastLevel.SetActive(false);
        this.lastLevel.tag = "LastLevel";
        this.lastLevel.name = "LastLevel";

    }

	public void setCheckpoint (Vector3 checkpoint) {
		this.checkpoint = checkpoint;
        GameObject.Destroy(this.lastLevel);
        this.lastLevel = GameObject.Instantiate(this.level, this.game);
        this.lastLevel.SetActive(false);
        this.lastLevel.tag = "LastLevel";
        this.lastLevel.name = "LastLevel";
    }

	public void Input() {
		if (UnityEngine.Input.GetAxisRaw ("Reset") != 0) {
			Died();
		}
	}

	public void Action() {
		if (player.dead) {
			if (dyingTimeout > 0.0f) {
				dyingTimeout -= Time.fixedDeltaTime;
			} else {
				if (player.isHuman) {
					player.animations.TriggerTransition (player.animations.RUN_STOP);
				} else {
					player.animations.TriggerTransition (player.animations.RUN_STOP_MONKEY);
				}
				dyingTimeout = 4.0f;
				player.dead = false;
				player.transform.position = checkpoint;
				GameObject.Destroy(this.level);
				this.level = GameObject.Instantiate(this.lastLevel, this.game);
				this.level.SetActive(true);
				GameObject.Find ("Generator0").GetComponent<GeneratorController> ().activated = generator0;
				GameObject.Find ("Generator1").GetComponent<GeneratorController> ().activated = generator1;
				GameObject.Find ("Generator2").GetComponent<GeneratorController> ().activated = generator2;
				//Debug.Log (generator0 + ";" + generator1 + ";" + generator2);

				this.level.tag = "Level";
				this.level.name = "Level";
				player.sounds.PlaySound (PlayerSounds.REVIVE);
				player.healthCanvas.text = new String ('|', 25);
			}
        }
	}

	public void Died() {
		player.dead = true;
		player.animations.TriggerTransition (player.animations.DEAD);
		player.sounds.PlaySound (PlayerSounds.DEAD);
		player.healthCanvas.text = "";
	}
}