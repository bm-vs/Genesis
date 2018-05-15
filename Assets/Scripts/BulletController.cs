using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public GameObject owner;
	private float soundTimer;
	private bool disabled;

	void Start() {
		soundTimer = 2.0f;
		disabled = false;
	}

	void Update() {
		if (soundTimer <= 0f && disabled) {
			Destroy (gameObject);
		} else {
			soundTimer -= Time.deltaTime;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != owner.tag && other.gameObject.tag != "Portal") {
			gameObject.GetComponent<MeshRenderer>().enabled = false;
			gameObject.GetComponent<SphereCollider>().enabled = false;
			disabled = true;
		}
	}
}
