using UnityEngine;
using System.Collections;
using System;

public class cameracontrol : MonoBehaviour {

	public float moveSpeed = 10f;
	public float turnSpeed = 50f;
	public Vector3 prevMousePos;

	// Use this for initialization
	void Start () {
		prevMousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {

		//check camera translation
		translateCamera();

		//now check rotation
		Vector3 mousePos = Input.mousePosition;

		if (Input.GetMouseButton(1)) {
			Vector3 rotDist = mousePos-prevMousePos;
			rotDist.y = rotDist.y*-1; //this is to make it consistent with movement in axises
			float d = Mathf.Sqrt(Mathf.Pow(rotDist.x,2)+Mathf.Pow(rotDist.y,2));

			if(d != 0) {
				float xDeg = Mathf.Asin(rotDist.y/d);
				float yDeg = Mathf.Asin(rotDist.x/d);
				
				transform.Rotate(new Vector3(xDeg,yDeg,0)*turnSpeed*Time.deltaTime);
			}
		}

		prevMousePos = mousePos;
	}

	private void translateCamera() {
		if(Input.GetKey(KeyCode.W))
			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.S))
			transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.A))
			transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.D))
			transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.Q))
			transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.E))
			transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
	}
}
