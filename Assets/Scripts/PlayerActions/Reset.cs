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
				this.level.tag = "Level";
				this.level.name = "Level";
			}
        }
	}

	public void Died() {
		player.dead = true;
		player.animations.TriggerTransition (player.animations.DEAD);
	}
}