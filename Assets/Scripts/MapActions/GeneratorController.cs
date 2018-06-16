using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : MonoBehaviour {
	public bool activated;
	public float health;

	[FMODUnity.EventRef]
	public string electricSound;
	public float electricVolume;
	private FMOD.Studio.EventInstance electricEvent;

	[FMODUnity.EventRef]
	public string explosionSound;
	public float explosionVolume;
	private FMOD.Studio.EventInstance explosionEvent;

	public float soundMaxDistance;

	void Start() {
		activated = false;
		health = 120.0f;
		electricEvent = FMODUnity.RuntimeManager.CreateInstance (electricSound);
		explosionEvent = FMODUnity.RuntimeManager.CreateInstance (explosionSound);
	}

	void Update() {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (electricEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		electricEvent.setVolume (electricVolume);
		electricEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (explosionEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		explosionEvent.setVolume (explosionVolume);
		explosionEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, soundMaxDistance);

		FMOD.Studio.PLAYBACK_STATE electricState;
		electricEvent.getPlaybackState (out electricState);
		if (electricState != FMOD.Studio.PLAYBACK_STATE.PLAYING && !activated) {
			electricEvent.start ();
		}
			
		if (health <= 0.0f && !activated) {
			activated = true;
			explosionEvent.start ();
			electricEvent.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Shot" && other.gameObject.GetComponent<BulletController> ().owner.tag == "Player") {
			health -= 20.0f;
		}
	}
}
