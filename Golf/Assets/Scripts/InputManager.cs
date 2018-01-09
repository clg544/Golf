using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    private enum SliderState { OFF, WAITING, LEFT, RIGHT, COMPLETE};
    private SliderState curState = SliderState.OFF;
    Text clubName;
    Slider PowerSlider;
    float sliderZero = 0.1F;
    public float curPower;
   

    public float rotateSpeed;

    public GameObject golfball;
    private Rigidbody ballBody;
    public Camera swingCam;
    public GameObject cameraTarget;
    GolfBallScript ballScript;
    
    /**
     * void HandleSlider(): Defines what the slider should do on each update, based on curState.
     */
    public void HandleSlider()
    {
        switch (curState)
        {
            case (SliderState.OFF):
                break;

            case (SliderState.WAITING):
                break;

            case (SliderState.LEFT):
                PowerSlider.value += Time.deltaTime;

                if(PowerSlider.value >= 1.0F)
                {
                    curState = SliderState.RIGHT;
                }
                break;

            case(SliderState.RIGHT):
                PowerSlider.value -= Time.deltaTime;

                if (PowerSlider.value <= 0.0F)
                {
                    curState = SliderState.OFF;
                    PowerSlider.value = sliderZero;
                    
                    curState = SliderState.COMPLETE;
                }
                break;

            case (SliderState.COMPLETE):
                if(ballBody.velocity.magnitude <= 0.1f)
                {
                    curState = SliderState.WAITING;
                }
                break;

        }
    }

    /**
     * void EnableSwing(): The script will reset states
     */
    public void EnableSwing()
    {
        curState = SliderState.WAITING;
    }
    
	void Start () {
        GameObject[] UIObjs;

        ballScript = golfball.GetComponent<GolfBallScript>();
        ballBody = golfball.GetComponent<Rigidbody>();
        curState = SliderState.WAITING;

        UIObjs = GameObject.FindGameObjectsWithTag("UI");
        for(int i = 0; i < UIObjs.Length; i++)
        {
            if (UIObjs[i].name == "PowerSlider")
                PowerSlider = UIObjs[i].GetComponent<Slider>();

            if (UIObjs[i].name == "ClubName")
                clubName = UIObjs[i].GetComponent<Text>();
        }
	}
	
	void Update() {
        HandleSlider();
        
        if (Input.GetKeyDown(KeyCode.Space) && ballScript.canHit)
        {
            switch (curState)
            {
                // Swing is disabled
                case (SliderState.OFF):
                    break;

                // Start the swing sequence
                case (SliderState.WAITING):
                    curState = SliderState.LEFT;
                    break;
                
                // Value is chosen, switch to right
                case (SliderState.LEFT):
                    curPower = PowerSlider.value;
                    curState = SliderState.RIGHT;
                    break;

                // Accuracy chosen, or slider ran off edge. Swing.
                case (SliderState.RIGHT):
                    if(curPower == 0.0F)
                    {
                        curPower = PowerSlider.value;
                    }
                    else
                    {
                        ballScript.HitBall(curPower);
                        curPower = 0.0F;
                    }
                    break;
            }
        }

        // Move to next or prev club
        if (Input.GetKeyDown(KeyCode.E))
        {
            clubName.text = ballScript.NextClub();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            clubName.text = ballScript.PrevClub();
        }

        // Aim further left or right
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            ballScript.RotateLeft(rotateSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            ballScript.RotateRight(rotateSpeed);
        }
    }
}
