using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimneyController : MonoBehaviour {
	private bool activated;
	public bool operational;
	public float cooldownTimer;
	public GameObject flame;
	private float flameTimer;
	private bool flameOn;
	private Vector3 flameOrigin;

	void Start() {
		activated = false;
		operational = false;
		flameOn = false;
		flameTimer = 1f;
		flameOrigin = flame.transform.position;
	}

	void Update() {
		// Control chimney height
		if (activated) {
			if (gameObject.transform.position.y < 81.8f) {
				gameObject.transform.Translate (0f, 0.2f, 0f);
				operational = false;
			} else {
				operational = true;
			}
		} else if (!activated && gameObject.transform.position.y > 70.4f) {
			gameObject.transform.Translate (0f, -0.2f, 0f);
			operational = false;
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

	public void activate() {
		activated = !activated;
	}
}
