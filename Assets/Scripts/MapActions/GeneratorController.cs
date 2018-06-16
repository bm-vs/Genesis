using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : MonoBehaviour {
	public bool activated;
	private float health;

	void Start() {
		activated = false;
		health = 120.0f;
	}

	void Update() {
		if (health <= 0.0f) {
			activated = true;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Shot" && other.gameObject.GetComponent<BulletController> ().owner.tag == "Player") {
			health -= 20.0f;
		}
	}
}
