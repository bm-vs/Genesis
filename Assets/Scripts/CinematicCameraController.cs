using UnityEngine;
using System.Collections;

public class CinematicCameraController : MonoBehaviour
{
	public GameObject gameCamera;
	public GameObject player;

	public Vector3 firstStop;
	public Vector3 finalStop;
	public bool start = false;
	private bool next = false;
	public float speed;
	public float speed2;
	private bool started = false;
	private float startTime;
	private float length;
	private Vector3 startPos;

	void Start () {
		startPos = transform.position;
		length = Vector3.Distance (startPos, firstStop);
	}

	void Update () {
		bool prevNext = next;
		if (start) {
			if (!started) {
				startTime = Time.time;
				started = true;
			}

			float distCov = (Time.time - startTime) * speed;
			float frac = distCov / length;
			transform.position = Vector3.Lerp (startPos, firstStop, frac);
			if (transform.position.y >= player.transform.position.y) {
				transform.LookAt (player.transform);
			}
			if (transform.position == firstStop) {
				next = true;
			}
		}

		if (next) {
			if (!prevNext) {
				startTime = Time.time;
				length = Vector3.Distance (firstStop, finalStop);
			} else {
				float distCov = (Time.time - startTime) * speed2;
				float frac = distCov / length;
				transform.position = Vector3.Lerp (firstStop, finalStop, frac);
				transform.LookAt (player.transform);
				if (transform.position == finalStop) {
					gameObject.SetActive (false);
					gameCamera.SetActive (true);
				}
			}
		}
}
}