using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallStates : MonoBehaviour {

    private Rigidbody myBody;
    private GolfBallScript myBallScript;
    public Collider myCollider;

    public enum GroundType
    {
        GREEN,
        FAIRWAY,
        ROUGH,
        SANDTRAP,
        EXTRAROUGH,
        ASPHAULT,
        DIRT
    }

    public bool isAirborne;
    public bool isOutOfBounds;

    public GroundType curGround;
    
    public void OnTriggerEnter(Collider coll)
    {
        myBallScript.BallCollision();

        if (coll.tag == "Fairway")
        {
            curGround = GroundType.FAIRWAY;
        }
    }

    public void OnTriggerStay(Collider coll)
    {
        if (coll.tag == "Fairway")
        {
            curGround = GroundType.FAIRWAY;
        }

        isAirborne = false;
    }

    public void OnTriggerExit(Collider coll)
    {
        switch (coll.tag)
        {
            default:
                isAirborne = true;
                break;
        }

    }

	// Use this for initialization
	void Start () {
        myBallScript = GetComponentInParent<GolfBallScript>();
        myBody = GetComponentInParent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
