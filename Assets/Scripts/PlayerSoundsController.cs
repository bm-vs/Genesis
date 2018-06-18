using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSounds {
	SPLASH = 0,
	COLLISION = 1,
	SHOOT = 2,
	JUMP_IMPACT_METAL = 3,
	JUMP_IMPACT = 4,
	DASH_IMPACT = 5,
	STEPS = 6,
	STEPS_METAL = 7,
	DASH_START = 8,
	JUMP_START = 9,
	CLIMB = 10,
	BACKGROUND_SEA = 11,
	BACKGROUND_ELECTRIC = 12,
	CHECKPOINT = 13,
	START = 14,
	DAMAGE = 15
}

public class PlayerSoundsController : MonoBehaviour {
	[FMODUnity.EventRef]
	public string[] sounds;
	public float[] volumes;
	private List<FMOD.Studio.EventInstance> events;

	public float soundMaxDistance;

	void Start () {
		events = new List<FMOD.Studio.EventInstance> ();
		foreach (string sound in sounds) {
			events.Add (FMODUnity.RuntimeManager.CreateInstance (sound));
		}
	}

	void Update () {
		for (int i = 0; i < events.Count; i++) {
			SetProperties (events[i], volumes[i]);
		}
	}

	void SetProperties (FMOD.Studio.EventInstance soundEvent, float soundVolume) {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (soundEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		soundEvent.setVolume (soundVolume);
		soundEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);
	}

	public void PlaySound (PlayerSounds sound) {
		events [(int)sound].start ();
	}

	public bool CheckIfPlaying (PlayerSounds sound) {
		FMOD.Studio.PLAYBACK_STATE state;
		events [(int)sound].getPlaybackState (out state);
		return state == FMOD.Studio.PLAYBACK_STATE.PLAYING;
	}
}
