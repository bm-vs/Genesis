using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimneyMainController : MonoBehaviour {
	public float cooldownTimer;
	public GameObject flame;
	private float flameTimer;
	private bool flameOn;
	private Vector3 flameOrigin;
	private bool operational;
	public GameObject[] chimneys;

	void Start() {
		flameOn = false;
		flameTimer = 1f;
		flameOrigin = flame.transform.position;
	}

	void Update() {
		operational = true;
		foreach (GameObject chimney in chimneys) {
			operational &= chimney.GetComponent<ChimneyController> ().operational;
		}

		// Sync flames
		cooldownTimer -= Time.deltaTime;
		if (cooldownTimer <= 0) {
			cooldownTimer = 6f;

			if (operational) {
				flameOn = true;
				flameTimer = 1f;
			}
		}

		// Control flame height
		if (flameOn && flame.transform.position.y < 89f) {
			flame.transform.Translate (0f, 0.5f, 0f);
		}

		if (flameTimer <= 0 || !operational) {
			flame.transform.position = flameOrigin;
		} else {
			flameTimer -= Time.deltaTime;
		}
	}
}
