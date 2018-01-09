using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public Camera myCamera;
    public GameObject CameraTarget;

    private Transform myTarget;

    public Vector3 relativePos;
    public float rotation;

	// Use this for initialization
	void Start () {
        myTarget = CameraTarget.transform;
	}
	
	// Update is called once per frame
	void Update () {
        myCamera.transform.position = myTarget.position + relativePos;
        myCamera.transform.LookAt(myTarget.position + new Vector3(0, rotation, 0));
    }
}
