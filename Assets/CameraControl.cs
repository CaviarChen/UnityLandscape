using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraControl : MonoBehaviour {

	public float mouseSensitivitiy = 5.0f;
	public float moveSpeed = 4.0f;
	public float rollSpeed = 100.0f;

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
		yaw += transform.eulerAngles.y;
		pitch += transform.eulerAngles.x;


		// pitch should be 0~90 or 270~360
		pitch = (pitch<=180.0f) ? Mathf.Min (90.0f, pitch) : Mathf.Max(pitch,270.0f);

		float roll = transform.eulerAngles.z;

		if (Input.GetKey (KeyCode.Q)) {
			roll += rollSpeed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.E)) {
			roll -= rollSpeed * Time.deltaTime;
		}

		transform.eulerAngles = new Vector3(pitch, yaw, roll);



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
