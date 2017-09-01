using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

	public Color color;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float speed;
        if(transform.position.y<-4) {
            speed = 35.0f;
        } else {
            speed = 10.0f;
        }


        transform.RotateAround (Vector3.zero, Vector3.right, speed * Time.deltaTime);
		transform.LookAt (Vector3.zero);
	}

	public Vector3 GetPointLightPosition()
	{
		return this.transform.position;
	}
}
