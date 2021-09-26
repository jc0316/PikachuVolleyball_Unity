using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Ball;

    public Player1Controller Player1_Controller;
    public Player2Controller Player2_Controller;
    public BallController Ball_Controller;

    //Physics stats
    public float ballHeavyness = 6f;

    public bool isHappened = false;
    public bool player1CanHit = true;
    public bool player2CanHit = true;

    // Start is called before the first frame update
    void Start()
    {
        Player1_Controller = Player1.GetComponent<Player1Controller>();
        Player2_Controller = Player2.GetComponent<Player2Controller>();
        Ball_Controller = Ball.GetComponent<BallController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BallPhysics();
    }

    public bool isCollisionBetweenBallAndPlayerHappened_1()
    {
        float diff = Ball.transform.localPosition.x - Player1.transform.localPosition.x;
        if(Mathf.Abs(diff) <= Utils.PLAYER_HALF_LENGTH/16f)
        {
            diff = Ball.transform.localPosition.y - Player1.transform.localPosition.y;
            if(Mathf.Abs(diff) <= Utils.PLAYER_HALF_LENGTH / 16f)
            {
                player2CanHit = true;
                return true;
            }
        }
        return false;
    }

    public bool isCollisionBetweenBallAndPlayerHappened_2()
    {
        float diff = Ball.transform.localPosition.x - Player2.transform.localPosition.x;
        if (Mathf.Abs(diff) <= Utils.PLAYER_HALF_LENGTH / 16f)
        {
            diff = Ball.transform.localPosition.y - Player2.transform.localPosition.y;
            if (Mathf.Abs(diff) <= Utils.PLAYER_HALF_LENGTH / 16f)
            {
                player1CanHit = true;
                return true;
            }
        }
        return false;
    }

    public void processCollisionBetweenBallAndPlayer_1()
    {
        if(Ball.transform.localPosition.x < Player1.transform.localPosition.x)
        {
            Ball_Controller.ballVelocityX = -(Mathf.Abs(Ball.transform.localPosition.x - Player1.transform.localPosition.x) / ballHeavyness);
        }
        else if(Ball.transform.localPosition.x > Player1.transform.localPosition.x)
        {
            Ball_Controller.ballVelocityX = (Mathf.Abs(Ball.transform.localPosition.x - Player1.transform.localPosition.x) / ballHeavyness);
        }
        if(Mathf.Approximately(Ball_Controller.ballVelocityX, 0))
        {
            Ball_Controller.ballVelocityX = Random.Range(-1, 2) / 16f;
        }

        float ballAbsYVelocity = Mathf.Abs(Ball_Controller.ballVelocityY);
        Ball_Controller.ballVelocityY = ballAbsYVelocity;
        if(ballAbsYVelocity < 15/32f)
        {
            Ball_Controller.ballVelocityY = 15 / 32f;
        }

        //jump and power-hit
        if(Player1_Controller.state == 2 && player1CanHit)
        {
            Ball_Controller.isPowerHit = true;
            Ball_Controller.ballVelocityX = -(Mathf.Abs(Player1_Controller.userInput_x) + 1) * 10 / 32f;
            Ball_Controller.ballVelocityY = Player1_Controller.userInput_y * 2 / 2f;
            player1CanHit = false;
            FindObjectOfType<AudioManager>().Play("BallPowerHit");
            FindObjectOfType<AudioManager>().Play("PlayerHitBall");
        }
        else
        {
            Ball_Controller.isPowerHit = false;
            player1CanHit = true;
        }
    }

    public void processCollisionBetweenBallAndPlayer_2()
    {
        if (Ball.transform.localPosition.x < Player2.transform.localPosition.x)
        {
            Ball_Controller.ballVelocityX = (Mathf.Abs(Ball.transform.localPosition.x - Player2.transform.localPosition.x) / ballHeavyness);
        }
        else if (Ball.transform.localPosition.x > Player2.transform.localPosition.x)
        {
            Ball_Controller.ballVelocityX = (Mathf.Abs(Ball.transform.localPosition.x - Player2.transform.localPosition.x) / ballHeavyness);
        }
        if (Mathf.Approximately(Ball_Controller.ballVelocityX, 0))
        {
            Ball_Controller.ballVelocityX = Random.Range(-1, 2) / 16f;
        }

        float ballAbsYVelocity = Mathf.Abs(Ball_Controller.ballVelocityY);
        Ball_Controller.ballVelocityY = ballAbsYVelocity;
        if (ballAbsYVelocity < 15 / 32f)
        {
            Ball_Controller.ballVelocityY = 15 / 32f;
        }

        //jump and power-hit
        if (Player2_Controller.state == 2 && player2CanHit)
        {
            Ball_Controller.isPowerHit = true;
            Ball_Controller.ballVelocityX = (Mathf.Abs(Player2_Controller.userInput_x) + 1) * 10 / 32f;
            Ball_Controller.ballVelocityY = Player2_Controller.userInput_y * 2 / 2f;
            player2CanHit = false;
            FindObjectOfType<AudioManager>().Play("BallPowerHit");
            FindObjectOfType<AudioManager>().Play("PlayerHitBall");
        }
        else
        {
            Ball_Controller.isPowerHit = false;
            player2CanHit = true;
        }
    }

    public void BallPhysics()
    {
        if(isCollisionBetweenBallAndPlayerHappened_1() && player1CanHit)
        {
            if(Player1_Controller.isCollisionWithBallHappened == false)
            {
                processCollisionBetweenBallAndPlayer_1();
                Player1_Controller.isCollisionWithBallHappened = true;
            }
            else
            {
                Player1_Controller.isCollisionWithBallHappened = false;
            }
        }
        else if(isCollisionBetweenBallAndPlayerHappened_2() && player2CanHit)
        {
            if (Player2_Controller.isCollisionWithBallHappened == false)
            {
                processCollisionBetweenBallAndPlayer_2();
                Player2_Controller.isCollisionWithBallHappened = true;
            }
            else
            {
                Player2_Controller.isCollisionWithBallHappened = false;
            }
        }

        Ball_Controller.caculate_expected_landing_point_x_for();
    }

    public void EnableHit()
    {
        player1CanHit = true;
        player2CanHit = true;
    }
}
