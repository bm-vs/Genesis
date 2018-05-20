using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset {
	private PlayerController player;
	private bool resetting;
	private Vector3 checkpoint;
    private GameObject lastLevel;
    private GameObject level;
    private Transform game; // to add object as its children

    public Reset (PlayerController player) {
		this.player = player;
		this.resetting = false;
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
			resetting = true;
		}
	}

	public void Action() {
		if (resetting) {
			resetting = false;
			player.transform.position = checkpoint;

            GameObject.Destroy(this.level);
            this.level = GameObject.Instantiate(this.lastLevel, this.game);
            this.level.SetActive(true);
            this.level.tag = "Level";
            this.level.name = "Level";
        }
	}

	public void Died() {
		resetting = true;
	}
}