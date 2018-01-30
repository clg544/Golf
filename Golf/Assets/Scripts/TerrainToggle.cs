using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainToggle : MonoBehaviour {

    TerrainCollider terrColl;

    public void OnTriggerEnter(Collider coll)
    {
        print("Hit!");

        if (coll.tag == "Ball")
        {
            terrColl.enabled = false;
        }
    }
    public void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Ball")
        {
            terrColl.enabled = true;
        }
    }

    // Use this for initialization
    void Start () {
        terrColl = GameObject.FindGameObjectWithTag("TerrainTexture").GetComponent<TerrainCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
