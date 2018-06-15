using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorController : MonoBehaviour {
	private Material activatorOnMaterial;
	private Material activatorOffMaterial;
	public bool activated;
	public bool done;
	private float activatedTimer;

	void Start() {
		activatorOnMaterial = Resources.Load ("Materials/ActivatorOn", typeof(Material)) as Material;
		activatorOffMaterial = Resources.Load ("Materials/ActivatorOff", typeof(Material)) as Material;
		activated = false;
		done = false;
		activatedTimer = 2.0f;
	}

	void Update() {
		if (activated && !done) {
			activatedTimer -= Time.deltaTime;
			if (activatedTimer <= 0) {
				activated = false;
				activatedTimer = 2.0f;
				gameObject.GetComponent<Renderer>().material = activatorOffMaterial;
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Shot") {
			activated = true;
			gameObject.GetComponent<Renderer>().material = activatorOnMaterial;
		}
	}

	public void SetDone() {
		done = true;
		gameObject.GetComponent<Renderer>().material = activatorOnMaterial;
	}
}
