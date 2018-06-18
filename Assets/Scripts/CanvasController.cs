using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour
{
	public CinematicCameraController cinematicCamera;

	void Update ()
	{
		if (UnityEngine.Input.GetKey(KeyCode.Return)) {
			cinematicCamera.start = true;
			gameObject.SetActive (false);
		}
	}
}

