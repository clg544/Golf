using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CLUB
{
    public string name;
    public float angle;
    public float backspin;
    public float powerMod;
    public float maxDist;
    public bool isDriver;
}

public class GolfBallScript : MonoBehaviour {
    public bool devMode = true;

    private CLUB[] GOLFCLUBS;
    
    public GameObject dirPointer;
    public GameObject swingCam;
    public GameObject cameraTarget;
    public GameObject DirectionPointer;
    public Collider TeeColl;
    public Rigidbody myBody;
    public GolfBallStates myState;

    /* Swinging Variables */
    public bool canHit;
    public float maxSpeed;              // Absolute max speed of the ball
    public int curClub;
    public float curAngle;
    public float curPower;
    public float curBackspin;           // How much backspin is on the ball
    public float curDirection;
    public float curPowerMod;
    public bool curIsDriver;

    /* Lift & Drag Modifiers */
    public float dragCoefficent;
    public float liftCoefficent;
    
    /* Club Values */
    public void InitClubs()
    {
        GOLFCLUBS = new CLUB[19];

        /* Woods */
        GOLFCLUBS[0].name = "Driver";
        GOLFCLUBS[0].angle = 13.0f;
        GOLFCLUBS[0].backspin = 8000.0f;
        GOLFCLUBS[0].powerMod = 1.0f;
        GOLFCLUBS[0].maxDist = 427.0f;
        GOLFCLUBS[0].isDriver = true;

        GOLFCLUBS[1].name = "2-Wood";
        GOLFCLUBS[1].angle = 15.0f;
        GOLFCLUBS[1].backspin = 8000.0f;
        GOLFCLUBS[1].powerMod = .98f;
        GOLFCLUBS[1].maxDist = 410.0f;
        GOLFCLUBS[1].isDriver = true;

        GOLFCLUBS[2].name = "3-Wood";
        GOLFCLUBS[2].angle = 17.0f;
        GOLFCLUBS[2].backspin = 8000.0f;
        GOLFCLUBS[2].powerMod = .96f;
        GOLFCLUBS[2].maxDist = 392.0f;
        GOLFCLUBS[2].isDriver = true;

        GOLFCLUBS[3].name = "4-Wood";
        GOLFCLUBS[3].angle = 19.0f;
        GOLFCLUBS[3].backspin = 8000.0f;
        GOLFCLUBS[3].powerMod = 0.94f;
        GOLFCLUBS[3].maxDist = 374.0f;
        GOLFCLUBS[3].isDriver = true;

        GOLFCLUBS[4].name = "5-Wood";
        GOLFCLUBS[4].angle = 23.0f;
        GOLFCLUBS[4].backspin = 8000.0f;
        GOLFCLUBS[4].powerMod = 0.92f;
        GOLFCLUBS[4].maxDist = 348.0f;
        GOLFCLUBS[4].isDriver = true;

        GOLFCLUBS[5].name = "6-Wood";
        GOLFCLUBS[5].angle = 25.0f;
        GOLFCLUBS[5].backspin = 8000.0f;
        GOLFCLUBS[5].powerMod = 0.9f;
        GOLFCLUBS[5].maxDist = 330.0f;
        GOLFCLUBS[5].isDriver = true;

        GOLFCLUBS[6].name = "7-Wood";
        GOLFCLUBS[6].angle = 28.0f;
        GOLFCLUBS[6].backspin = 8000.0f;
        GOLFCLUBS[6].powerMod = .88f;
        GOLFCLUBS[6].maxDist = 306.0f;
        GOLFCLUBS[6].isDriver = true;

        /* Irons */
        GOLFCLUBS[7].name = "1-Iron";
        GOLFCLUBS[7].angle = 18.0f;
        GOLFCLUBS[7].backspin = 5500.0f;
        GOLFCLUBS[7].powerMod = .86f;
        GOLFCLUBS[7].maxDist = 295.0f;
        GOLFCLUBS[7].isDriver = false;

        GOLFCLUBS[8].name = "2-Iron";
        GOLFCLUBS[8].angle = 20.0f;
        GOLFCLUBS[8].backspin = 5500.0f;
        GOLFCLUBS[8].powerMod = .84f;
        GOLFCLUBS[8].maxDist = 280.0f;
        GOLFCLUBS[8].isDriver = false;

        GOLFCLUBS[9].name = "3-Iron";
        GOLFCLUBS[9].angle = 24.0f;
        GOLFCLUBS[9].backspin = 5500.0f;
        GOLFCLUBS[9].powerMod = .82f;
        GOLFCLUBS[9].maxDist = 259.0f;
        GOLFCLUBS[9].isDriver = false;

        GOLFCLUBS[10].name = "4-Iron";
        GOLFCLUBS[10].angle = 28.0f;
        GOLFCLUBS[10].backspin = 5500.0f;
        GOLFCLUBS[10].powerMod = .80f;
        GOLFCLUBS[10].maxDist = 238.0f;
        GOLFCLUBS[10].isDriver = false;

        GOLFCLUBS[11].name = "5-Iron";
        GOLFCLUBS[11].angle = 32.0f;
        GOLFCLUBS[11].backspin = 5500.0f;
        GOLFCLUBS[11].powerMod = .78f;
        GOLFCLUBS[11].maxDist = 215.0f;
        GOLFCLUBS[11].isDriver = false;

        GOLFCLUBS[12].name = "6-Iron";
        GOLFCLUBS[12].angle = 36.0f;
        GOLFCLUBS[12].backspin = 5500.0f;
        GOLFCLUBS[12].powerMod = .76f;
        GOLFCLUBS[12].maxDist = 192.0f;
        GOLFCLUBS[12].isDriver = false;

        GOLFCLUBS[13].name = "7-Iron";
        GOLFCLUBS[13].angle = 40.0f;
        GOLFCLUBS[13].backspin = 5500.0f;
        GOLFCLUBS[13].powerMod = .74f;
        GOLFCLUBS[13].maxDist = 170.0f;
        GOLFCLUBS[13].isDriver = false;

        GOLFCLUBS[14].name = "8-Iron";
        GOLFCLUBS[14].angle = 44.0f;
        GOLFCLUBS[14].backspin = 5500.0f;
        GOLFCLUBS[14].powerMod = .72f;
        GOLFCLUBS[14].maxDist = 148.0f;
        GOLFCLUBS[14].isDriver = false;

        GOLFCLUBS[15].name = "9-Iron";
        GOLFCLUBS[15].angle = 48.0f;
        GOLFCLUBS[15].backspin = 5500.0f;
        GOLFCLUBS[15].powerMod = .70f;
        GOLFCLUBS[15].maxDist = 127.0f;
        GOLFCLUBS[15].isDriver = false;

        /* Wedges */
        GOLFCLUBS[16].name = "PW";
        GOLFCLUBS[16].angle = 53.0f;
        GOLFCLUBS[16].backspin = 5500.0f;
        GOLFCLUBS[16].powerMod = .68f;
        GOLFCLUBS[16].maxDist = 104.0f;
        GOLFCLUBS[16].isDriver = false;

        GOLFCLUBS[17].name = "SW";
        GOLFCLUBS[17].angle = 58.0f;
        GOLFCLUBS[17].backspin = 5500.0f;
        GOLFCLUBS[17].powerMod = .66f;
        GOLFCLUBS[17].maxDist = 82.0f;
        GOLFCLUBS[17].isDriver = false;

        GOLFCLUBS[18].name = "Putter";
        GOLFCLUBS[18].angle = 0.0f;
        GOLFCLUBS[18].backspin = 0.0f;
        GOLFCLUBS[18].powerMod = .25f;
        GOLFCLUBS[18].maxDist = 35.0f;
        GOLFCLUBS[18].isDriver = false;

    }

    /*8
     *  Ball Physics Functions 
     */
    public float InitialVelocity(float clubAngle, float powerPercentage)
    {
        return ((-0.25f * clubAngle) + 60) * powerPercentage * curPowerMod;
    }
    public float InitialAngle(float clubAngle, float powerPercentage)
    {
        return (((4.0F / 5.0F) * clubAngle) + 6.0F);
    }
    public float BallAirDrag(float ballVelocity)
    {
        return (ballVelocity * ballVelocity * dragCoefficent);
    }
    public float BallAirUplift(float ballVelocity, float ballBackspin)
    {
        curBackspin -= (ballVelocity  * liftCoefficent * curBackspin);
        return (ballVelocity * liftCoefficent * curBackspin);
    }

    /**
     * void ReesetTee(): Disable tee collider (holds ball above ground
     */ 
    public void ResetTee()
    {
        TeeColl.enabled = true;
    }

    /**
     * void HitBall(float ball): Apply force pow to the ball, with physics functions applied
     * 
     *      float pow: How hard the ball was hit
     */
    public void HitBall(float pow)
    {
        TeeColl.enabled = false;

        float vel = InitialVelocity(curAngle, pow);
        float angle = curAngle;
        curBackspin = GOLFCLUBS[curClub].backspin;

        // Putting
        if (curAngle == 0.0F)
            angle = 0.0F;

        Vector3 shotVector = (DirectionPointer.transform.position - this.transform.position).normalized;
        shotVector = shotVector * vel;

        shotVector = Vector3.RotateTowards(shotVector, Vector3.up, angle * Mathf.Deg2Rad, 0);
        
        myBody.AddForce(shotVector);
    }

    /**
     *  void AddForce(Vector3 force): Add force to this RigidBody
     *  
     *      Vector3 force: Force to apply
     */
    public void AddForce(Vector3 force)
    {
        myBody.AddForce(force);
    }

    /**
     *  void BallCollision: Function to run everytime the ball hits something.
     */
    public void BallCollision()
    {
        curBackspin *= 0.5F;
    }

    /**
     *  string NextClub():  Switch selected club to the next option, if possible
     *  
     *      returns: the name of the new club.
     */ 
    public string NextClub()
    {
        if (curClub < GOLFCLUBS.Length - 1)
        {
            curClub++;

            curAngle = GOLFCLUBS[curClub].angle;
            curBackspin = GOLFCLUBS[curClub].backspin;
            curPowerMod = GOLFCLUBS[curClub].powerMod;
            curIsDriver = GOLFCLUBS[curClub].isDriver;

            Vector3 newPos = DirectionPointer.transform.localPosition;
            newPos.y = 0;

            DirectionPointer.transform.localPosition = newPos.normalized * (GOLFCLUBS[curClub].maxDist * (1 / transform.localScale.z));
        }

        return GOLFCLUBS[curClub].name;
    }

    /**
     *  string PrevClub():  Switch selected club to the previous option, if possible
     *  
     *      returns: the name of the new club.
     */
    public string PrevClub()
    {
        if (curClub > 0)
        {
            curClub--;

            curAngle = GOLFCLUBS[curClub].angle;
            curBackspin = GOLFCLUBS[curClub].backspin;
            curPowerMod = GOLFCLUBS[curClub].powerMod;
            curIsDriver = GOLFCLUBS[curClub].isDriver;

            Vector3 newPos = DirectionPointer.transform.localPosition;
            newPos.y = 0;
            
            DirectionPointer.transform.localPosition = newPos.normalized * (GOLFCLUBS[curClub].maxDist * (1 / transform.localScale.z));
        }

        return GOLFCLUBS[curClub].name;
    }

    /**
     *  string RotateLeft():  Move the SwingCam to the left, based on rotate speed.
     *  
     *      rotateSpeed: How fast to rotate the camera.
     */
    public void RotateLeft(float rotateSpeed)
    {
        dirPointer.transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);
        cameraTarget.transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);
        swingCam.transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);
    }

    /**
     *  string RotateRight():  Move the SwingCam to the right, based on rotate speed.
     *  
     *      rotateSpeed: How fast to rotate the camera.
     */
    public void RotateRight(float rotateSpeed)
    {
        dirPointer.transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
        swingCam.transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
        cameraTarget.transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }
    
    void Start () {
        canHit = devMode;
        InitClubs();

        curClub = 1;
        PrevClub();
    }
	
	void Update ()
    {
        float lift = BallAirUplift(myBody.velocity.magnitude, curBackspin);

        if (myState.isAirborne && curBackspin > 0)
        {
            myBody.AddForce(new Vector3(myBody.velocity.normalized.x * lift * dragCoefficent, lift, 
                    myBody.velocity.normalized.z * lift * dragCoefficent) * Time.deltaTime);
        }
	}
}
