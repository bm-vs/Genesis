using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour {
	public GameObject healthCanvas;
	public GameObject canvas;
	private float timer = 0.0f;
	private bool start = false;
	public Renderer[] playerRenderers;

	[FMODUnity.EventRef]
	public string endSound;
	public float endVolume;
	private FMOD.Studio.EventInstance endEvent;

	[FMODUnity.EventRef]
	public string portalSound;
	public float portalVolume;
	private FMOD.Studio.EventInstance portalEvent;

	void Start () {
		endEvent = FMODUnity.RuntimeManager.CreateInstance (endSound);
		portalEvent = FMODUnity.RuntimeManager.CreateInstance (portalSound);
	}

	void Update () {
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (endEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		endEvent.setVolume (endVolume);
		endEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, 40.0f);

		FMODUnity.RuntimeManager.AttachInstanceToGameObject (portalEvent, GetComponent<Transform> (), GetComponent<Rigidbody> ());
		portalEvent.setVolume (portalVolume);
		portalEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, 40.0f);

		if (start) {
			if (timer >= 0.0f) {
				timer -= Time.deltaTime;
			} else {
				SceneManager.LoadScene (SceneManager.GetActiveScene().name);
			}
		}
	}


	void OnCollisionEnter (Collision collision) {
		if (collision.collider.gameObject.tag == "Player") {
			collision.collider.gameObject.GetComponent<PlayerController>().cutscene = true;
			for (int i = 0; i < playerRenderers.Length; i++) {
				playerRenderers[i].enabled = false;
			}
			healthCanvas.SetActive (false);
			canvas.SetActive (true);
			timer = 4.0f;
			start = true;
			endEvent.start ();
			portalEvent.start ();
		}
	}
}
