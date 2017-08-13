using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraControl : MonoBehaviour {

	public float mouseSensitivitiy = 5.0f;
	public float moveSpeed = 4.0f;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

	}
	
	// Update is called once per frame
	void Update () {


		// mouse
		float yaw = mouseSensitivitiy * Input.GetAxis("Mouse X");
		float pitch = -mouseSensitivitiy * Input.GetAxis("Mouse Y");
		transform.eulerAngles += new Vector3(pitch, yaw, 0.0f);


		if (Input.GetKey (KeyCode.W)) {
			transform.position += transform.forward * moveSpeed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.position -= transform.forward * moveSpeed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.A)) {
			transform.position -= transform.right * moveSpeed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.position += transform.right * moveSpeed * Time.deltaTime;
		}

		
	}
}
