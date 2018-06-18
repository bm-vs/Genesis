using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : MonoBehaviour {
	public bool activated;
	public float health;

	public GameObject status;
	public GameObject ok1;
	public GameObject ok2;
	public GameObject danger1;
	public GameObject danger2;
	public GameObject critical1;

	public GameObject explosion;

	private Material okMat;
	private Material dangerMat;
	private Material criticalMat;

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

		okMat = Resources.Load ("Materials/GeneratorOk", typeof(Material)) as Material;
		dangerMat = Resources.Load ("Materials/GeneratorDanger", typeof(Material)) as Material;
		criticalMat = Resources.Load ("Materials/GeneratorCritical", typeof(Material)) as Material;
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
			explosion.SetActive (true);
			electricEvent.stop (FMOD.Studio.STOP_MODE.IMMEDIATE);
			gameObject.GetComponent<Renderer> ().enabled = false;
			gameObject.GetComponent<Collider> ().enabled = false;
			status.SetActive (false);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Shot" && other.gameObject.GetComponent<BulletController> ().owner.tag == "Player") {
			health -= 20.0f;
			if (health == 100.0f) {
				ok1.GetComponent<Renderer>().material = okMat;
			} else if (health == 80.0f) {
				ok2.GetComponent<Renderer>().material = okMat;
			} else if (health == 60.0f) {
				danger1.GetComponent<Renderer>().material = dangerMat;
			} else if (health == 40.0f) {
				danger2.GetComponent<Renderer>().material = dangerMat;
			} else if (health == 20.0f) {
				critical1.GetComponent<Renderer>().material = criticalMat;
			}
		}
	}
}
