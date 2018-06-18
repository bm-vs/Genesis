using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimneyController : MonoBehaviour {
	private bool activated;
	public bool operational;
	public float cooldownTimer;
	public GameObject flame;
	public GameObject particles;
	private float flameTimer;
	private bool flameOn;
	private Vector3 flameOrigin;

	[FMODUnity.EventRef]
	public string upSound;
	public float upVolume;
	private FMOD.Studio.EventInstance upEvent;

	[FMODUnity.EventRef]
	public string downSound;
	public float downVolume;
	private FMOD.Studio.EventInstance downEvent;

	[FMODUnity.EventRef]
	public string exhaustSound;
	public float exhaustVolume;
	private FMOD.Studio.EventInstance exhaustEvent;

	public float soundMaxDistance;

	void Start() {
		activated = false;
		operational = false;
		flameOn = false;
		flameTimer = 1f;
		flameOrigin = flame.transform.position;

		upEvent = FMODUnity.RuntimeManager.CreateInstance (upSound);
		downEvent = FMODUnity.RuntimeManager.CreateInstance (downSound);
		exhaustEvent = FMODUnity.RuntimeManager.CreateInstance (exhaustSound);
	}

	void Update() {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (upEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		upEvent.setVolume (upVolume);
		upEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (downEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		downEvent.setVolume (downVolume);
		downEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (exhaustEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		exhaustEvent.setVolume (exhaustVolume);
		exhaustEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);


		// Control chimney height
		if (activated) {
			if (gameObject.transform.position.y < 81.8f) {
				gameObject.transform.Translate (0f, 0.2f, 0f);
				operational = false;
			} else {
				operational = true;
			}
		} else if (!activated && gameObject.transform.position.y > 70.4f) {
			gameObject.transform.Translate (0f, -0.2f, 0f);
			operational = false;
		}

		// Sync flames
		cooldownTimer -= Time.deltaTime;
		if (cooldownTimer <= 0) {
			cooldownTimer = 6f;

			if (operational) {
				flameOn = true;
				flameTimer = 1f;
				exhaustEvent.start ();
				particles.SetActive (true);
			}
		}

		// Control flame height
		if (flameOn && flame.transform.position.y < 89f) {
			flame.transform.Translate (0f, 0.5f, 0f);
		}

		if (flameTimer <= 0 || !operational) {
			flame.transform.position = flameOrigin;
			exhaustEvent.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
			particles.SetActive (false);
		} else {
			flameTimer -= Time.deltaTime;
		}
	}

	public void activate() {
		activated = !activated;
		if (activated) {
			upEvent.start ();
		} else {
			downEvent.start ();
		}
	}
}
