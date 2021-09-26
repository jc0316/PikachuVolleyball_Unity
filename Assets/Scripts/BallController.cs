using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Sprite[] ballSpriteArray;
    public GameObject prevBall;
    public GameObject prevprevBall;
    public GameObject copyBall;
    public GameObject copyPowerBall;
    public GameObject Trail;
    public GameObject[] Trails = new GameObject[200];
    public GameObject PhysicsManager;
    public GameObject GameManager;


    Physics Physics_Controller;
    GameManagerController GameManager_Controller;

    //Ball stats
    public float gravity_amount = -1f;
    public float rotation_update_coefficient = 3.5f;

    //Ball variables
    public bool isPowerHit = false;
    public float ballVelocityX = 1/16f;
    public float ballVelocityY = 0;
    public float expectedLandingPointX = 0;
    public float expectedLandingPointX_PowerHit = 0;
    public float rotation = 0;
    public float fineRotation = 0;
    public float punchEffectX = 0;
    public float punchEffectY = 0;
    public float previousX = 0;
    public float previousY = 0;
    public float previousPreviousX = 0;
    public float previousPreviousY = 0;
    public int frame = 0;
    public int trailUpdateFreq = 2;

    public float futureBallX = 0;
    public float futureBallY = 0;

    public float copyballVelocityX = 1 / 16f;
    public float copyballVelocityY = 0;
    public float copyfutureBallX = 0;

    public float copyPowerBallVelocityX = 1 / 16f;
    public float copyPowerBallVelocityY = 0;
    public float copyfuturePowerBallX = 0;

    // Start is called before the first frame update
    void Start()
    {
        Physics_Controller = PhysicsManager.GetComponent<Physics>();
        GameManager_Controller = GameManager.GetComponent<GameManagerController>();
        initializeForNewRound(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        processCollisionBetweenBallAndWorldAndSetBallPosition();
        frame = frame + 1;
    }

    public void initializeForNewRound(bool isPlayer2Serve)
    {
        if (isPlayer2Serve)
        {
            ballVelocityX = 0;
            ballVelocityY = 0;
            this.transform.localPosition = new Vector3(-10f, 9.5f, this.transform.localPosition.z);
        }
        else
        {
            ballVelocityX = 0;
            ballVelocityY = 0;
            this.transform.localPosition = new Vector3(10f, 9.5f, this.transform.localPosition.z);
        }
        isPowerHit = false;
        frame = 0;
    }

    public void processCollisionBetweenBallAndWorldAndSetBallPosition()
    {
        if(frame % trailUpdateFreq == 0)
        {
            previousPreviousX = previousX;
            previousPreviousY = previousY;
            previousX = this.transform.position.x;
            previousY = this.transform.position.y;
        }

        prevBall.transform.position = new Vector3(previousX, previousY, this.transform.position.z);
        prevprevBall.transform.position = new Vector3(previousPreviousX, previousPreviousY, this.transform.position.z);
        if(isPowerHit)
        {
            prevBall.GetComponent<SpriteRenderer>().enabled = true;
            prevprevBall.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            prevBall.GetComponent<SpriteRenderer>().enabled = false;
            prevprevBall.GetComponent<SpriteRenderer>().enabled = false;
        }

        float futureFineRotation = fineRotation + ((ballVelocityX) * rotation_update_coefficient);
        if(futureFineRotation < 0)
        {
            futureFineRotation = futureFineRotation + 50;
        }
        else if(futureFineRotation > 50)
        {
            futureFineRotation = futureFineRotation - 50;
        }
        fineRotation = futureFineRotation;
        rotation = Mathf.Floor(fineRotation / 10);
        this.GetComponent<SpriteRenderer>().sprite = ballSpriteArray[(int)rotation];

        futureBallX = this.transform.localPosition.x + ballVelocityX;
        futureBallY = this.transform.localPosition.y + ballVelocityY;

        //Collision with boundaries
        if((futureBallX > (Utils.GROUND_HALF_WIDTH - Utils.BALL_RADIUS) / 16f) || (futureBallX < -(Utils.GROUND_HALF_WIDTH - Utils.BALL_RADIUS)*0.97f / 16f))
        {
            ballVelocityX = -ballVelocityX;
            Physics_Controller.EnableHit();
        }
        if(futureBallY > 9.5f)
        {
            ballVelocityY = -1/16f;
            Physics_Controller.EnableHit();
        }

        //Collision with pole
        if((Mathf.Abs(this.transform.localPosition.x) < Utils.NET_PILLAR_HALF_WIDTH/16f) && (this.transform.localPosition.y < -1.5f)) //NET_PILLAR_TOP_TOP_Y_COORD
        {
            if(this.transform.localPosition.y >= -2.5f) //NET_PILLAR_TOP_BOTTOM_Y_COORD
            {
                if(ballVelocityY < 0)
                {
                    ballVelocityY = -ballVelocityY;
                }
            }
            else
            {
                if(this.transform.localPosition.x < 0)
                {
                    ballVelocityX = -Mathf.Abs(ballVelocityX);
                }
                else
                {
                    ballVelocityX = Mathf.Abs(ballVelocityX);
                }    
            }
            Physics_Controller.EnableHit();
        }

        futureBallY = this.transform.localPosition.y + ballVelocityY;

        //Collision with ground
        if(futureBallY < -6.25f) //BALL_TOUCHING_GROUND_Y_COORD
        {
            ballVelocityY = -ballVelocityY;
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, -6.25f, this.transform.localPosition.z);
            Physics_Controller.EnableHit(); //Should comment this out
            ballTouchesGround();
        }

        this.transform.localPosition = new Vector3(this.transform.localPosition.x + ballVelocityX, futureBallY, this.transform.localPosition.z);
        ballVelocityY = ballVelocityY + gravity_amount/64f;
    }

    public void caculate_expected_landing_point_x_for()
    {
        copyBall.transform.localPosition = this.transform.localPosition;
        copyballVelocityX = ballVelocityX;
        copyballVelocityY = ballVelocityY;

        int loopCounter = 0;
        while(true)
        {
            loopCounter = loopCounter + 1;

            copyfutureBallX = copyballVelocityX + copyBall.transform.localPosition.x;
            //Collision with boundaries
            if ((copyfutureBallX > (Utils.GROUND_HALF_WIDTH - Utils.BALL_RADIUS) / 16f) || (copyfutureBallX < -(Utils.GROUND_HALF_WIDTH - Utils.BALL_RADIUS) * 0.97f / 16f))
            {
                copyballVelocityX = -copyballVelocityX;
            }
            if (copyBall.transform.localPosition.y + copyballVelocityY > 9.5f)
            {
                copyballVelocityY = -1 / 16f;
            }
            

            //Collision with pole
            if ((Mathf.Abs(copyBall.transform.localPosition.x) < Utils.NET_PILLAR_HALF_WIDTH / 16f) && (copyBall.transform.localPosition.y < -1.5f)) //NET_PILLAR_TOP_TOP_Y_COORD
            {
                if (copyBall.transform.localPosition.y >= -2.5f) //NET_PILLAR_TOP_BOTTOM_Y_COORD
                {
                    if (copyballVelocityY < 0)
                    {
                        copyballVelocityY = -copyballVelocityY;
                    }
                }
                else
                {
                    if (copyBall.transform.localPosition.x < 0)
                    {
                        copyballVelocityX = -Mathf.Abs(copyballVelocityX);
                    }
                    else
                    {
                        copyballVelocityX = Mathf.Abs(copyballVelocityX);
                    }
                }
            }

            copyBall.transform.localPosition = new Vector3(copyBall.transform.localPosition.x, copyBall.transform.localPosition.y + copyballVelocityY, copyBall.transform.localPosition.z);

            //Collision with ground
            if (copyBall.transform.localPosition.y < -6.25f || loopCounter >= Utils.INFINITE_LOOP_LIMIT) //BALL_TOUCHING_GROUND_Y_COORD
            {
                break;
            }

            copyBall.transform.localPosition = new Vector3(copyBall.transform.localPosition.x + copyballVelocityX, copyBall.transform.localPosition.y, copyBall.transform.localPosition.z);
            copyballVelocityY = copyballVelocityY + gravity_amount / 64f;

            //Trails[loopCounter] = Instantiate(Trail, copyBall.transform.position, Quaternion.identity);
        }

        for (int i = 0; i < Trails.Length; i++)
        {
            //Destroy(Trails[i].gameObject, 0.5f);
        }

        expectedLandingPointX = copyBall.transform.localPosition.x;
    }

    public void expectedLandingPointXWhenPowerHit(int userInputXDirection, int userInputYDirection)
    {
        copyPowerBall.transform.localPosition = this.transform.localPosition;
        copyPowerBallVelocityX = ballVelocityX;
        copyPowerBallVelocityY = ballVelocityY;

        if(this.transform.localPosition.x < 0)
        {
            copyPowerBallVelocityX = (Mathf.Abs(userInputXDirection) + 1) * 10 / 32f;
        }
        else
        {
            copyPowerBallVelocityX = -(Mathf.Abs(userInputXDirection) + 1) * 10 / 32f;
        }
        copyPowerBallVelocityY = Mathf.Abs(copyPowerBallVelocityY) * userInputYDirection * 2 / 2f;

        int loopCounter = 0;
        while(true)
        {
            loopCounter = loopCounter + 1;

            copyfuturePowerBallX = copyPowerBall.transform.localPosition.x + copyPowerBallVelocityX;
            if ((copyfuturePowerBallX > (Utils.GROUND_HALF_WIDTH - Utils.BALL_RADIUS) / 16f) || (copyfuturePowerBallX < -(Utils.GROUND_HALF_WIDTH - Utils.BALL_RADIUS) * 0.97f / 16f))
            {
                copyPowerBallVelocityX = -copyPowerBallVelocityX;
            }
            if (copyPowerBall.transform.localPosition.y + copyPowerBallVelocityY > 9.5f)
            {
                copyPowerBallVelocityY = -1 / 16f;
            }

            //Collision with pole
            if ((Mathf.Abs(copyPowerBall.transform.localPosition.x) < Utils.NET_PILLAR_HALF_WIDTH / 16f) && (copyPowerBall.transform.localPosition.y < -1.5f)) //NET_PILLAR_TOP_TOP_Y_COORD
            {
                if(copyPowerBall.transform.localPosition.y >= -2.5f)//NET_PILLAR_TOP_BOTTOM_Y_COORD
                {
                    copyPowerBallVelocityY = -copyPowerBallVelocityY;
                }
                else
                {
                    if(copyPowerBall.transform.localPosition.x < 0)
                    {
                        copyPowerBallVelocityX = -Mathf.Abs(copyPowerBallVelocityX);
                    }
                    else
                    {
                        copyPowerBallVelocityX = Mathf.Abs(copyPowerBallVelocityX);
                    }
                }
            }
            copyPowerBall.transform.localPosition = new Vector3(copyPowerBall.transform.localPosition.x, copyPowerBall.transform.localPosition.y + copyPowerBallVelocityY, copyPowerBall.transform.localPosition.z);
            if(copyPowerBall.transform.localPosition.y < -6.25f || loopCounter >= Utils.INFINITE_LOOP_LIMIT)
            {
                break;
            }
            copyPowerBall.transform.localPosition = new Vector3(copyPowerBall.transform.localPosition.x + copyPowerBallVelocityX, copyPowerBall.transform.localPosition.y, copyPowerBall.transform.localPosition.z);
            copyPowerBallVelocityY = copyPowerBallVelocityY + gravity_amount / 64f;
            expectedLandingPointX_PowerHit = copyPowerBall.transform.localPosition.x;
        }
    }

    public void ballTouchesGround()
    {
        if(!GameManager_Controller.practiceMode && !GameManager_Controller.roundOver)
        {
            if (this.transform.localPosition.x > 0) //Dropped on the right(player1)
            {
                GameManager_Controller.addScore(2);
            }
            else if (this.transform.localPosition.x <= 0) //Dropped on the left(player2)
            {
                GameManager_Controller.addScore(1);
            }
            FindObjectOfType<AudioManager>().Play("BallTouchGround");
        }
    }
}
