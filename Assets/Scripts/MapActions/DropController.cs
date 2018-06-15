using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour {
	public GameObject[] siblings;
	public bool falling;
	public bool activated;
	private float gravityCounter;
	private float activatedTimer;

	void Start() {
		gravityCounter = 120.0f;
		activatedTimer = 0.75f;
		falling = false;
	}

	void Update() {
		if (activated) {
			activatedTimer -= Time.deltaTime;
			if (activatedTimer <= 0) {
				StartFalling ();
			}
		}
	}

	void FixedUpdate() {
		gameObject.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, gravityCounter, 0));
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			activated = true;
			foreach (GameObject sibling in siblings) {
				sibling.GetComponent<DropController> ().StartFalling ();
			}
			GameObject[] dropPlane = GameObject.FindGameObjectsWithTag ("Drop");
			foreach (GameObject element in dropPlane) {
				DropController controller = element.GetComponent<DropController> ();
				if (!controller.activated) {
					controller.DropPlane ();
				}
			}
		}
	}

	void StartFalling() {
		gravityCounter = 0.0f;
		falling = true;
		gameObject.GetComponent<Rigidbody> ().constraints &= ~RigidbodyConstraints.FreezePositionY;
	}

	void DropPlane() {
		activated = true;
		activatedTimer = Random.value;
	}
}
