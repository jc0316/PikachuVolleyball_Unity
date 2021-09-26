using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controller : MonoBehaviour
{
    public Animator animator;

    //Player stats
    public float movementspeed_x = 6f;
    public float movementspeed_y = 16f; // jump
    public float divespeed_x = 10f;
    public float divespeed_y = 5f;
    public float gravity_amount = -1f;
    public float dive_cooldown = 10;
    public float dive_stun = 12;

    //Player variables
    public float playerVelocityX = 0;
    public float playerVelocityY = 0;
    public int state = 0;
    public bool isCollisionWithBallHappened = false;
    public int divingDirection = 0;
    public float lyingDownDurationLeft = -1;
    public int userInput_x = 0;
    public int userInput_y = 0;
    public int powerHit = 0;
    public bool hasControl = false;

    //Outside
    public int outsideInput_x = 0;
    public int outsideInput_y = 0;
    public int outsidePowerHit = 0;

    // Start is called before the first frame update
    void Start()
    {
        initializeForNewRound();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        processPlayerMovementAndSetPlayerPosition();
    }

    public void initializeForNewRound()
    {
        this.transform.localPosition = new Vector3(-11f, -6f, this.transform.localPosition.z);
    }

    public void processPlayerMovementAndSetPlayerPosition()
    {
        userInput_x = 0;
        userInput_y = 0;
        powerHit = 0;

        //lying down
        if (state == 4)
        {
            lyingDownDurationLeft = lyingDownDurationLeft - 1;
            if (lyingDownDurationLeft < -1)
            {
                state = 0;
                this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
            }
            return;
        }

        if (state == 0)
        {
            lyingDownDurationLeft = lyingDownDurationLeft - 1;
        }

        //left-right
        if (state < 5)
        {
            if (state < 3)
            {
                decideInput(out userInput_x, out userInput_y, out powerHit);
                playerVelocityX = userInput_x * movementspeed_x / 32f;
            }
            else
            {
                playerVelocityX = divingDirection * divespeed_x / 32f;
            }
        }
        float futurePlayerX = this.transform.position.x + playerVelocityX;
        this.transform.position = new Vector3(futurePlayerX, this.transform.position.y, this.transform.position.z);

        if (this.transform.localPosition.x > -Utils.PLAYER_HALF_LENGTH / 16f)
        {
            this.transform.localPosition = new Vector3(-Utils.PLAYER_HALF_LENGTH / 16f, this.transform.localPosition.y, this.transform.localPosition.z);
        }
        else if (this.transform.localPosition.x < -(Utils.GROUND_HALF_WIDTH - Utils.PLAYER_HALF_LENGTH) / 16f)
        {
            this.transform.localPosition = new Vector3(-(Utils.GROUND_HALF_WIDTH - Utils.PLAYER_HALF_LENGTH) / 16f, this.transform.localPosition.y, this.transform.localPosition.z);
        }

        //jump
        if (state < 3 && (userInput_y == 1) && Mathf.Approximately(this.transform.localPosition.y, Utils.RELATIVE_GROUND))
        {
            playerVelocityY = movementspeed_y / 32f;
            state = 1;
            FindObjectOfType<AudioManager>().Play("PlayerJump");
        }

        //gravity
        float futurePlayerY = this.transform.position.y + playerVelocityY;
        this.transform.position = new Vector3(this.transform.position.x, futurePlayerY, this.transform.position.z);
        if (this.transform.localPosition.y > Utils.RELATIVE_GROUND)
        {
            playerVelocityY = playerVelocityY + gravity_amount / 64f;
        }
        else
        {
            playerVelocityY = 0;
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, Utils.RELATIVE_GROUND, this.transform.localPosition.z);
            if (state == 3)
            {
                state = 4;
                lyingDownDurationLeft = dive_cooldown;
            }
            else
            {
                state = 0;
            }
        }



        //power-hit
        if (powerHit == 1)
        {
            if (state == 1)// if jump, power-hit
            {
                state = 2;
            }
            else if (state == 4)
            {

                return;
            }
            else if (state == 0 && userInput_x != 0 && lyingDownDurationLeft < -dive_stun)// else, dive
            {
                state = 3;
                divingDirection = userInput_x;
                playerVelocityY = divespeed_y / 32f;
                this.transform.localScale = new Vector3(divingDirection, this.transform.localScale.y, this.transform.localScale.z);
            }
        }

        animator.SetInteger("State", state);
    }

    public void decideInput(out int userInput_x, out int userInput_y, out int powerHit)
    {
        userInput_x = 0;
        userInput_y = 0;
        powerHit = 0;

        if (hasControl)
        {
            if (Input.GetKey(KeyCode.A))
            {
                userInput_x = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                userInput_x = 1;
            }
            else
            {
                userInput_x = 0;
            }

            if (Input.GetKey(KeyCode.W))
            {
                userInput_y = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                userInput_y = -1;
            }
            else
            {
                userInput_y = 0;
            }

            if (Input.GetKey(KeyCode.G))
            {
                powerHit = 1;
            }
        }
        else
        {
            userInput_x = outsideInput_x;
            userInput_y = outsideInput_y;
            powerHit = outsidePowerHit;
        }
    }

    public void outsideInput(int out_x = 0, int out_y = 0, int out_power = 0)
    {
        outsideInput_x = out_x;
        outsideInput_y = out_y;
        outsidePowerHit = out_power;
    }
}
