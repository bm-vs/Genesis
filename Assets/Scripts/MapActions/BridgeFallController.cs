using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeFallController : MonoBehaviour {
	public bool start;
	private bool fell;

	[FMODUnity.EventRef]
	public string fallingSound;
	public float fallingVolume;
	private FMOD.Studio.EventInstance fallingEvent;

	public float soundMaxDistance;

	void Start() {
		start = false;
		fell = false;
		fallingEvent = FMODUnity.RuntimeManager.CreateInstance (fallingSound);
	}

	void Update() {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (fallingEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		fallingEvent.setVolume (fallingVolume);
		fallingEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		FMOD.Studio.PLAYBACK_STATE fallingState;
		fallingEvent.getPlaybackState (out fallingState);
		if (fallingState != FMOD.Studio.PLAYBACK_STATE.PLAYING && start && !fell) {
			fallingEvent.start ();
			fell = true;
		}
	}
}
