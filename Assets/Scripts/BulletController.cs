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
		Physics.IgnoreCollision (gameObject.GetComponent<Collider> (), owner.GetComponent<Collider> ());
	}

	void Update() {
		if (soundTimer <= 0f && disabled) {
			Destroy (gameObject);
		} else {
			soundTimer -= Time.deltaTime;
		}
	}

	void OnCollisionEnter(Collision other) {
		GameObject target = other.collider.gameObject;
		if (target.tag != owner.tag && target.tag != "Portal") {
			gameObject.GetComponent<MeshRenderer>().enabled = false;
			gameObject.GetComponent<SphereCollider>().enabled = false;
			disabled = true;
		}
	}
}
