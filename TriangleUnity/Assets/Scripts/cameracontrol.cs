using UnityEngine;
using System.Collections;
using System;

/**
 * Script to be attatched to a camera for camera movement.
 * Keys: WASD for forward/left/back/right and QE for up/down.
 * Mouse is used for camera control.
 */

public class cameracontrol : MonoBehaviour {
	
	public float moveSpeed = 10f;
	public float turnSpeed = 50f;
	public float shiftSpeedIncreaseFactor = 4;
	public Vector3 prevMousePos;
	
	// Use this for initialization
	void Start () {
		prevMousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
		float ms, ts;
		
		if (Input.GetKey (KeyCode.LeftShift) == true) {
			ms = moveSpeed*shiftSpeedIncreaseFactor;
			ts = turnSpeed*shiftSpeedIncreaseFactor;
		} else {
			ms = moveSpeed;
			ts = turnSpeed;
		}
		
		
		//check camera translation
		translateCamera(ms);
		
		//now check rotation
		Vector3 mousePos = Input.mousePosition;
		
		if (Input.GetMouseButton(1)) {
			Vector3 rotDist = mousePos-prevMousePos;
			rotDist.y = rotDist.y*-1; //this is to make it consistent with movement in axises
			float d = Mathf.Sqrt(Mathf.Pow(rotDist.x,2)+Mathf.Pow(rotDist.y,2));
			
			if(d != 0) {
				float xDeg = Mathf.Asin(rotDist.y/d);
				float yDeg = Mathf.Asin(rotDist.x/d);
				
				transform.Rotate(new Vector3(xDeg,yDeg,0)*ts*Time.deltaTime);
			}
		}
		
		if(Input.GetKey (KeyCode.Z)) {
			transform.Rotate (new Vector3(0,0,1)*ts*Time.deltaTime);
		}
		
		if(Input.GetKey (KeyCode.C)) {
			transform.Rotate (new Vector3(0,0,-1)*ts*Time.deltaTime);
		}
		
		prevMousePos = mousePos;
	}
	
	private void translateCamera(float ms) {
		//ms is moveSpeed
		if(Input.GetKey(KeyCode.W))
			transform.Translate(Vector3.forward * ms * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.S))
			transform.Translate(-Vector3.forward * ms * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.A))
			transform.Translate(Vector3.left * ms * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.D))
			transform.Translate(Vector3.right * ms * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.Q))
			transform.Translate(Vector3.down * ms * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.E))
			transform.Translate(Vector3.up * ms * Time.deltaTime);

		if (Input.GetKeyDown(KeyCode.X))
			transform.rotation *= Quaternion.Euler(-90, 0, 0);
			//transform.Translate(Vector3.up * ms * Time.deltaTime);
	}
}
