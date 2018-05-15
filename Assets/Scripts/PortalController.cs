using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {
	public GameObject[] activators;
	private bool open;

	void Start() {
		open = false;
	}

	void Update() {
		bool temp = true;
		if (!open) {
			foreach (GameObject activator in activators) {
				temp &= activator.GetComponent<ActivatorController> ().activated;
			}
		}

		// check if portal was opened this on this frame
		if (!open && temp) {
			open = true;
			foreach (Transform child in transform) {
				child.gameObject.SetActive (true);
			}
			foreach (GameObject activator in activators) {
				activator.GetComponent<ActivatorController> ().SetDone ();
			}
		}
	}
}
