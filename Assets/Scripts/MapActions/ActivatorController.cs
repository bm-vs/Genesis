using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorController : MonoBehaviour {
	private Material activatorOnMaterial;
	private Material activatorOffMaterial;
	public bool activated;
	public bool done;
	private float activatedTimer;

	[FMODUnity.EventRef]
	public string activationSound;
	public float activationSoundVolume;
	private FMOD.Studio.EventInstance activationEvent;

	[FMODUnity.EventRef]
	public string activationCompleteSound;
	public float activationCompleteSoundVolume;
	private FMOD.Studio.EventInstance activationCompleteEvent;

	public float soundMaxDistance;

	void Start() {
		activatorOnMaterial = Resources.Load ("Materials/ActivatorOn", typeof(Material)) as Material;
		activatorOffMaterial = Resources.Load ("Materials/ActivatorOff", typeof(Material)) as Material;
		activated = false;
		done = false;
		activatedTimer = 2.0f;
		activationEvent = FMODUnity.RuntimeManager.CreateInstance (activationSound);
		activationCompleteEvent = FMODUnity.RuntimeManager.CreateInstance (activationCompleteSound);
	}

	void Update() {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (activationEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		activationEvent.setVolume (activationSoundVolume);
		activationEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (activationCompleteEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		activationCompleteEvent.setVolume (activationCompleteSoundVolume);
		activationCompleteEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

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
			activationEvent.start ();
		}
	}

	public void SetDone() {
		done = true;
		gameObject.GetComponent<Renderer>().material = activatorOnMaterial;
		activationCompleteEvent.start ();
	}
}
