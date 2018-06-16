using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public GameObject owner;
	private float soundTimer;
	private bool disabled;
	private float range;
	private Vector3 start;

	void Start() {
		soundTimer = 2.0f;
		disabled = false;
		Physics.IgnoreCollision (gameObject.GetComponent<Collider> (), owner.GetComponent<Collider> ());
		if (owner.tag == "Sniper") {
			range = 100f;
		} else {
			range = 35f;
		}
		start = transform.position;
	}

	void Update() {
		if (soundTimer <= 0f && disabled) {
			Destroy (gameObject);
		} else {
			soundTimer -= Time.deltaTime;
		}

		if ((transform.position - start).magnitude >= range) {
			Disable ();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other != null) {
			GameObject target = other.gameObject;
			if (target.tag != owner.tag && target.tag != "Portal") {
				Disable ();
			}
		} else {
			Disable ();
		}
	}

	void Disable () {
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<SphereCollider>().enabled = false;
		disabled = true;
	}
}
