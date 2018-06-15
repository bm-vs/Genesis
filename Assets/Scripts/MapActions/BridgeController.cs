using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour {
	public GameObject firstPlank;
	public GameObject nextPlank;
	public bool falling;
	private float gravityCounter;
	private float nextPlankTimer;

	void Start() {
		gravityCounter = 120.0f;
		nextPlankTimer = 0.25f;
		falling = false;
	}

	void Update() {
		if (falling) {
			nextPlankTimer -= Time.deltaTime;
			if (nextPlankTimer <= 0 && nextPlank != null) {
				nextPlank.GetComponent<BridgeController>().StartFalling ();
			}
		}
	}

	void FixedUpdate() {
		gameObject.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, gravityCounter, 0));
	}

	void OnCollisionEnter(Collision collision) {
		if (!firstPlank.GetComponent<BridgeController> ().falling && collision.gameObject.tag != "Terrain") {
			firstPlank.GetComponent<BridgeController> ().StartFalling ();
		}
	}

	void StartFalling() {
		gravityCounter = 0.0f;
		falling = true;
		gameObject.GetComponent<Rigidbody> ().constraints &= ~RigidbodyConstraints.FreezePositionY;
	}
}
