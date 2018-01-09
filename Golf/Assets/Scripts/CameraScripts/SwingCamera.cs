using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingCamera : MonoBehaviour {

    public Camera myCam;
    public GameObject target;

    public Vector3 CameraPosition;
    public float CameraVertRotation;

	// Use this for initialization
	void Start () {
        myCam = GetComponent<Camera>();
        myCam.transform.localPosition = CameraPosition;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(target.transform.position + (new Vector3(0, CameraVertRotation, 0)));
    }
}
