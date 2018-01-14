using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallStates : MonoBehaviour {

    private Rigidbody myBody;
    private GolfBallScript myBallScript;
    public Collider myCollider;
    
    public bool isAirborne;
    public bool isOutOfBounds;

    public Area curGround;
    
    public void OnTriggerEnter(Collider coll)
    {
        myBallScript.BallCollision();

        print(LayerMask.LayerToName(coll.gameObject.layer));
        print(coll.gameObject.tag);

        if(coll.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            switch (coll.gameObject.tag)
            {
                case "Green":
                    curGround = Area.GREEN;
                    break;
                case "Fairway":
                    curGround = Area.FAIRWAY;
                    break;
                case "Rough":
                    curGround = Area.ROUGH;
                    break;
                case "ExtraRough":
                    curGround = Area.EXTRA_ROUGH;
                    break;
                default:
                    throw new System.Exception
                        ("GolfBallStates:OnTriggerEnter:Unexpected Tag passed from ground layer:" 
                        + coll.gameObject.tag.ToString());
            }
        }
    }

    public void OnTriggerStay(Collider coll)
    {
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
