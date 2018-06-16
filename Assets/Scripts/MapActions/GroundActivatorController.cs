using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundActivatorController : MonoBehaviour {
	private Material activatorOnMaterial;
	private Material activatorOffMaterial;
	private List<Collider> activated;
	public GameObject chimney1;
	public GameObject chimney2;

	void Start() {
		activatorOnMaterial = Resources.Load ("Materials/ActivatorOn", typeof(Material)) as Material;
		activatorOffMaterial = Resources.Load ("Materials/ActivatorOff", typeof(Material)) as Material;
		activated = new List<Collider>();
		gameObject.GetComponent<Renderer>().material = activatorOffMaterial;
	}

	void OnTriggerEnter(Collider other) {
		activated.Add (other);
		if (activated.Count == 1) {
			gameObject.GetComponent<Renderer> ().material = activatorOnMaterial;
			chimney1.GetComponent<ChimneyController> ().activate ();
			chimney2.GetComponent<ChimneyController> ().activate ();
		}
	}

	void OnTriggerExit(Collider other) {
		activated.Remove (other);
		if (activated.Count == 0) {
			gameObject.GetComponent<Renderer>().material = activatorOffMaterial;
			chimney1.GetComponent<ChimneyController> ().activate ();
			chimney2.GetComponent<ChimneyController> ().activate ();
		}
	}
}
