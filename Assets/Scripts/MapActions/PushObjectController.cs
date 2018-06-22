using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObjectController : MonoBehaviour {

	[FMODUnity.EventRef]
	public string pushSound;
	public float volume;
	private FMOD.Studio.EventInstance soundEvent;

	[FMODUnity.EventRef]
	public string fallSound;
	public float fallVolume;
	private FMOD.Studio.EventInstance fallEvent;

	private float startY;

	void Start () {
		soundEvent = FMODUnity.RuntimeManager.CreateInstance (pushSound);
		fallEvent = FMODUnity.RuntimeManager.CreateInstance (fallSound);
		startY = GetComponent<Transform> ().position.y;
	}

	void Update () {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (soundEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		soundEvent.setVolume (volume);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (fallEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		fallEvent.setVolume (fallVolume);

		if (gameObject.GetComponent<Rigidbody> ().velocity.magnitude > 0.1f) {
			FMOD.Studio.PLAYBACK_STATE state;
			soundEvent.getPlaybackState (out state);
			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
				soundEvent.start ();
			}
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.tag == "Terrain" && GetComponent<Transform> ().position.y < startY) {
			fallEvent.start ();
		}
	}
}
