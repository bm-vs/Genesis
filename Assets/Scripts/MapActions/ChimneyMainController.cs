using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimneyMainController : MonoBehaviour {
	public float cooldownTimer;
	public GameObject flame;
	private float flameTimer;
	private bool flameOn;
	private Vector3 flameOrigin;
	private bool operational;
	public GameObject[] chimneys;

	[FMODUnity.EventRef]
	public string exhaustSound;
	public float exhaustVolume;
	private FMOD.Studio.EventInstance exhaustEvent;

	public float soundMaxDistance;

	void Start() {
		flameOn = false;
		flameTimer = 1f;
		flameOrigin = flame.transform.position;
		exhaustEvent = FMODUnity.RuntimeManager.CreateInstance (exhaustSound);
	}

	void Update() {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (exhaustEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		exhaustEvent.setVolume (exhaustVolume);
		exhaustEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		operational = true;
		foreach (GameObject chimney in chimneys) {
			operational &= chimney.GetComponent<ChimneyController> ().operational;
		}

		// Sync flames
		cooldownTimer -= Time.deltaTime;
		if (cooldownTimer <= 0) {
			cooldownTimer = 6f;

			if (operational) {
				flameOn = true;
				flameTimer = 1f;
				exhaustEvent.start ();
			}
		}

		// Control flame height
		if (flameOn && flame.transform.position.y < 89f) {
			flame.transform.Translate (0f, 0.5f, 0f);
		}

		if (flameTimer <= 0 || !operational) {
			flame.transform.position = flameOrigin;
			exhaustEvent.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		} else {
			flameTimer -= Time.deltaTime;
		}
	}
}
