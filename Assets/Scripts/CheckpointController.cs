using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {

    private float initialY;

    private float delta;
    private bool isUp = true;
    private float upVelocity = 0.5f;

    // Use this for initialization
    void Start () {
        initialY = this.transform.position.y;
        delta = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
        var currentY = this.transform.position.y;
        if (currentY >= initialY + delta) {
            isUp = false;
        } else if (currentY <= initialY) {
            isUp = true;
        }
       
        this.transform.SetPositionAndRotation(new Vector3(this.transform.position.x, currentY + upVelocity * Time.deltaTime * (isUp ? 1 : -1), this.transform.position.z), this.transform.rotation);

    }
}
