using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObjectController : MonoBehaviour {
	[FMODUnity.EventRef]
	public string pushSound;
	public float volume;
	private FMOD.Studio.EventInstance soundEvent;

	void Start () {
		soundEvent = FMODUnity.RuntimeManager.CreateInstance (pushSound);
	}

	void Update () {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (soundEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		soundEvent.setVolume (volume);
		if (gameObject.GetComponent<Rigidbody> ().velocity.magnitude > 0.1f) {
			FMOD.Studio.PLAYBACK_STATE state;
			soundEvent.getPlaybackState (out state);
			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
				soundEvent.start ();
			}
		}
	}
}
