using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeleportController : MonoBehaviour {
	public GameObject target;
	public List<GameObject> teleporting;

	void Start() {
		teleporting = new List<GameObject>();
	}

	void OnTriggerEnter(Collider other) {
		GameObject teleported = other.gameObject;
		if (!teleporting.Contains (teleported)) {
			Vector3 delta = teleported.transform.position - transform.position;
			float rotationDiff = -Quaternion.Angle (transform.rotation, target.transform.rotation);
			teleported.transform.Rotate (Vector3.up, rotationDiff);
			teleported.transform.position = target.transform.position + Quaternion.Euler (0f, rotationDiff, 0f) * delta;
			target.GetComponent<TeleportController> ().teleporting.Add (teleported);
		}
	}

	void OnTriggerExit(Collider other) {
		GameObject teleported = other.gameObject;
		if (teleporting.Contains (teleported)) {
			teleporting.Remove (teleported);
		}
	}
}
