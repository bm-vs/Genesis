using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
	public GameObject owner;
	private string ownerTag;
	private float soundTimer;
	private bool disabled;
	public float range = 0.0f;
	private Vector3 start;

	void Start() {
		ownerTag = owner.tag;
		soundTimer = 2.0f;
		disabled = false;
		Physics.IgnoreCollision (gameObject.GetComponent<Collider> (), owner.GetComponent<Collider> ());
		if (range == 0.0f) {
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
		GameObject target = other.gameObject;
		if (target.tag != ownerTag && target.tag != "Portal" && target.tag != "Shot") {
			Disable ();
		}
	}

	void Disable () {
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<SphereCollider>().enabled = false;
		disabled = true;
	}
}
