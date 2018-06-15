using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameController : MonoBehaviour {
	public float impulse;

	void OnCollisionEnter(Collision collision) {
		collision.gameObject.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, impulse, 0), ForceMode.Impulse);
		collision.gameObject.GetComponent<PlayerController> ().jumping = true;
	}
}
