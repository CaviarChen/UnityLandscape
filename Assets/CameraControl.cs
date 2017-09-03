using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraControl : MonoBehaviour {

	public TerrainGenerator generator;
	public float mouseSensitivitiy = 5.0f;
	public float moveSpeed = 4.0f;
	public float rollSpeed = 100.0f;
	public bool turnboundary = true;
	private Rigidbody rbody;
	private float endsize;
	private float height;
	private float offset = 0.1f;

	// Use this for initialization
	void Start () {

		endsize = 0.5f * (generator.size-1) * generator.unitSize;
		height = generator.heightRange;
		transform.position = new Vector3 (endsize,height+20,endsize);
		transform.LookAt (new Vector3(endsize,-height,-endsize));


		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		rbody = GetComponent<Rigidbody> ();
		rbody.freezeRotation = true;	// Prevent camera rotate after collision.

        Physics.defaultContactOffset = 0.001f;
	}
	
	// Update is called once per frame
	void Update () {


        print("fps");
        print(1/Time.deltaTime);

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

        rbody.velocity = Vector3.zero;

        Vector3 moveVec = new Vector3(0, 0, 0);

        if (Input.GetKey (KeyCode.W)) {
            moveVec += transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey (KeyCode.S)) {
            moveVec -= transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey (KeyCode.A)) {
            moveVec -= transform.right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey (KeyCode.D)) {
            moveVec += transform.right * moveSpeed * Time.deltaTime;
        }

        rbody.MovePosition(rbody.position + moveVec);

        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        x = Mathf.Max(x, -endsize);
        x = Mathf.Min(x, endsize);
        z = Mathf.Max(z, -endsize);
        z = Mathf.Min(z, endsize);

		transform.position = new Vector3(x, y, z);
        


	}
}
