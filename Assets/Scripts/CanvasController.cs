using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour
{
	public CinematicCameraController cinematicCamera;

	[FMODUnity.EventRef]
	public string startSound;
	public float startVolume;
	private FMOD.Studio.EventInstance startEvent;

	void Start () {
		startEvent = FMODUnity.RuntimeManager.CreateInstance (startSound);
	}

	void Update ()
	{
		FMODUnity.RuntimeManager.AttachInstanceToGameObject (startEvent, GameObject.FindGameObjectWithTag("Player").GetComponent<Transform> (), GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody> ());
		startEvent.setVolume (startVolume);
		startEvent.setProperty (FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, 40.0f);

		if (UnityEngine.Input.GetKey(KeyCode.Return)) {
			cinematicCamera.start = true;
			gameObject.SetActive (false);
			startEvent.start ();
		}
	}
}

