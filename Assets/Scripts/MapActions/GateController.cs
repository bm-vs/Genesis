using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour {
	public GameObject[] activators;
	private bool open;
	private float height;
	private float startingPosition;

	[FMODUnity.EventRef]
	public string openSound;
	public float openVolume;
	private FMOD.Studio.EventInstance openEvent;

	public float soundMaxDistance;

	void Start() {
		open = false;
		height = gameObject.GetComponent<MeshRenderer> ().bounds.size.y;
		startingPosition = transform.position.y;
		openEvent = FMODUnity.RuntimeManager.CreateInstance (openSound);
	}

	void Update() {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (openEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		openEvent.setVolume (openVolume);
		openEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		bool temp = true;
		if (!open) {
			foreach (GameObject activator in activators) {
				if (activator.GetComponent<ActivatorController> () != null) {
					temp &= activator.GetComponent<ActivatorController> ().activated;
				}
				else if (activator.GetComponent<GeneratorController> () != null) {
					temp &= activator.GetComponent<GeneratorController> ().activated;
				}
			}
		}

		if (!open && temp) {
			open = true;
			openEvent.start ();
			foreach (GameObject activator in activators) {
				if (activator.GetComponent<ActivatorController> () != null) {
					activator.GetComponent<ActivatorController> ().SetDone ();
				}
			}
		}

		if (open && transform.position.y < startingPosition + height) {
			transform.position += new Vector3(0.0f, 0.1f, 0.0f);
		}
	}
}
