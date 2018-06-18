using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour {
	public GameObject canvas;
	private float timer = 0.0f;
	private bool start = false;
	public Renderer[] playerRenderers;

	void Update () {
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
			canvas.SetActive (true);
			timer = 4.0f;
			start = true;
		}
	}
}
