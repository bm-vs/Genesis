using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameController : MonoBehaviour {
	public float impulse;

	void OnTriggerEnter(Collider other) {
		other.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, impulse, 0), ForceMode.Impulse);
		other.GetComponent<PlayerController> ().jumping = true;
	}
}
