using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUController : MonoBehaviour
{
    public GameObject Ball;
    public GameObject Player1;

    //CPU stats
    public int computerBoldness = 0;

    //CPU variables
    public int computerWhereToStandBy = 0;// If it is 0, computer player stands by around the middle point of their side. If it is 1, computer player stands by adjecent to the net.
    public float virtualExpectedLandingPointX;
    public float expectedLandingPointX = 0;

    Player2Controller Player2_Controller;
    BallController Ball_Controller;

    // Start is called before the first frame update
    void Start()
    {
        Player2_Controller = this.GetComponent<Player2Controller>();
        Ball_Controller = Ball.GetComponent<BallController>();
        computerBoldness = Random.Range(0, 5);
        computerBoldness = 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Player2_Controller.hasControl)
        {
            letComputerDecideUserInput();
        }
        if (Ball_Controller.futureBallY < -6.25f)
        {
            computerBoldness = Random.Range(0, 5);
            //computerBoldness = 2;
        }
    }

    public void letComputerDecideUserInput()
    {
        Player2_Controller.outsideInput_x = 0;
        Player2_Controller.outsideInput_y = 0;
        Player2_Controller.outsidePowerHit = 0;
        virtualExpectedLandingPointX = Ball_Controller.expectedLandingPointX;
        float leftBoundary = -Utils.GROUND_HALF_WIDTH / 16f;
        float rightBoundary = 0;

        if ((Mathf.Abs(Ball.transform.localPosition.x - this.transform.localPosition.x) > 100/16f) && (Mathf.Abs(Ball_Controller.ballVelocityX * 16) < (computerBoldness + 5)))
        {
            if(((Ball_Controller.expectedLandingPointX <= leftBoundary) || (Ball_Controller.expectedLandingPointX >= 0)) && (computerWhereToStandBy == 0)) //Weird
            {
                virtualExpectedLandingPointX = leftBoundary + Utils.GROUND_HALF_WIDTH / 2 / 16f + (computerBoldness -  2)*2;
            }
        }

        int rng = Random.Range(0, 20);
        if (Mathf.Abs(virtualExpectedLandingPointX - this.transform.localPosition.x)*16 > computerBoldness + 8)
        {
            if(this.transform.localPosition.x < virtualExpectedLandingPointX)
            {
                Player2_Controller.outsideInput_x = 1;
            }
            else
            {
                Player2_Controller.outsideInput_x = -1;
            }
        }
        else if(rng == 0)
        {
            computerWhereToStandBy = Random.Range(0, 2);
        }

        if(Player2_Controller.state == 0)
        {
            //low speed interception
            if (
                Mathf.Abs(Ball_Controller.ballVelocityX) * 16 < computerBoldness + 3 &&
                Mathf.Abs(Ball.transform.localPosition.x - this.transform.localPosition.x) < Utils.PLAYER_HALF_LENGTH / 16f &&
                Ball.transform.localPosition.y < 9.5f + 36 / 16f &&
                -(Ball.transform.localPosition.y - 9.5f) * 16f < 10 * computerBoldness + 84 &&
                Ball_Controller.ballVelocityY < 0
               )
            {
                Player2_Controller.outsideInput_y = 1; //jump
            }

            if (
            Ball_Controller.expectedLandingPointX < rightBoundary + computerBoldness * 2 &&
            Mathf.Abs(Ball_Controller.expectedLandingPointX - this.transform.localPosition.x) * 16f > computerBoldness * 2  + Utils.PLAYER_LENGTH - 40&&
            Ball.transform.localPosition.x < rightBoundary + computerBoldness * 2 &&
            Ball.transform.localPosition.y < 9.5f - (174 / 16f) - computerBoldness/2 + 1
            )
            {
                Player2_Controller.outsidePowerHit = 1; //dive
                if (this.transform.localPosition.x < Ball_Controller.expectedLandingPointX + 0.1f)
                {
                    Player2_Controller.outsideInput_x = 1;
                }
                else if(this.transform.localPosition.x > Ball_Controller.expectedLandingPointX - 0.1f)
                {
                    Player2_Controller.outsideInput_x = -1;
                }
            }
        }
        else if(Player2_Controller.state == 1 || Player2_Controller.state == 2)
        {
            if(Mathf.Abs(Ball.transform.localPosition.x - this.transform.localPosition.x) > 8/16f)
            {
                if (this.transform.localPosition.x < (Ball_Controller.expectedLandingPointX + Ball.transform.localPosition.x)/2f)
                {
                    Player2_Controller.outsideInput_x = 1;
                }
                else if (this.transform.localPosition.x > (Ball_Controller.expectedLandingPointX + Ball.transform.localPosition.x) / 2f)
                {
                    Player2_Controller.outsideInput_x = -1;
                }
            }

            if(Mathf.Abs(Ball.transform.localPosition.x - this.transform.localPosition.x) < 48/16f && Mathf.Abs(Ball.transform.localPosition.y - this.transform.localPosition.y) < 48 / 16f)
            {
                bool willInputPowerHit = decideWhetherInputPowerHit();
                if(willInputPowerHit)
                {
                    Player2_Controller.outsidePowerHit = 1;
                    if(Mathf.Abs(Ball.transform.localPosition.x - this.transform.localPosition.x) < 80/16f && Player2_Controller.userInput_y != 1)
                    {
                        Player2_Controller.outsideInput_y = 1;
                        willInputPowerHit = decideWhetherInputPowerHit();
                    }
                }
            }
        }

        computerWhereToStandBy = 0;
    }

    public bool decideWhetherInputPowerHit()
    {
        expectedLandingPointX = 0;
        int rng = Random.Range(0, 2);
        if(rng == 0)
        {
            for(int xDirection = 1; xDirection > -1; xDirection--)
            {
                for (int yDirection = -1; yDirection < 2; yDirection++)
                {
                    Ball_Controller.expectedLandingPointXWhenPowerHit(xDirection, yDirection);
                    expectedLandingPointX = Ball_Controller.expectedLandingPointX_PowerHit;
                    if((expectedLandingPointX <= -Utils.GROUND_HALF_WIDTH / 16f || expectedLandingPointX >= 0) &&
                        Mathf.Abs(expectedLandingPointX - Player1.transform.localPosition.x)  > Utils.PLAYER_LENGTH/16f
                      )
                    {
                        //print("rng = 0 : X " + xDirection + " Y " + yDirection);
                        Player2_Controller.outsideInput_x = xDirection;
                        Player2_Controller.outsideInput_y = yDirection;
                        return true;
                    }
                }
            }
        }
        else
        {
            for (int xDirection = 1; xDirection > -1; xDirection--)
            {
                for (int yDirection = 1; yDirection > -2; yDirection--)
                {
                    Ball_Controller.expectedLandingPointXWhenPowerHit(xDirection, yDirection);
                    expectedLandingPointX = Ball_Controller.expectedLandingPointX_PowerHit;
                    if ((expectedLandingPointX <= -Utils.GROUND_HALF_WIDTH / 16f || expectedLandingPointX >= 0) &&
                        Mathf.Abs(expectedLandingPointX - Player1.transform.localPosition.x) > Utils.PLAYER_LENGTH / 16f
                      )
                    {
                        //print("rng = 1 : X " + xDirection + " Y " + yDirection);
                        Player2_Controller.outsideInput_x = xDirection;
                        Player2_Controller.outsideInput_y = yDirection;
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
