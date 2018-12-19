using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script managing players

public class PlayerManager : MonoBehaviour {

    //Constante
    private readonly float TIME_MAX_IN_SLOW_MO = 0.6f;

    [Tooltip("Speed of the player1, value 0 means the player1 can't move. Recommanded between 5 and 20")]
    public float speedX;        //Change the speed value
    [Tooltip("Force of the player1 jump, value 0 means the player1 won't jump. Recommanded between 500 and 1000")]
    public float jumpSpeedY;    //Change the jump speed value

    public float playerNumber;

    public  bool facingRight, isGrounded, canDoubleJump;
    private float speed;

    public KeyCode controlLeft,controlRight, controlUp, controlDown;

    private Animator animator;
    private Rigidbody2D rb;
    
    public TimeManager timeManager;

    
   

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        timeManager = new TimeManager();

        facingRight = false;
        canDoubleJump = false;
	}
	
	// Update is called once per frame
	void Update ()
    {        
        MovePlayer(speed); //handle player1 movement
        Flip();            //change the direction of the animation if needed
        
        //left player1 movement
        if(Input.GetKey(controlLeft))
        {
            speed = -speedX;
        }        
        if(Input.GetKeyUp(controlLeft) && !Input.GetKey(controlUp))
        {
            speed = 0;
        }

        //right player1 movement
        if (Input.GetKey(controlRight))
        {
            speed = speedX;            
        }
        if (Input.GetKeyUp(controlRight) && !Input.GetKey(controlUp))
        {
            speed = 0;
        }

        //player1 jump
        if(Input.GetKeyDown(controlUp) && (isGrounded || canDoubleJump) )
        {
            Jump();
            //Propulse();
            Debug.Log("Jumping");            
        }
        
    }

    private void Jump()
    {
        rb.velocity = (new Vector2(rb.velocity.x, jumpSpeedY)); //add a velocity Y
        canDoubleJump = false;
    }

    //TEST PROPULSION VELOCITY
    private void Propulse()
    {
        rb.velocity = (new Vector2(10, jumpSpeedY + 10)); //add a velocity Y
        
        canDoubleJump = false;
    }

    //Code for player movement
    private void MovePlayer(float playerSpeed)
    {
        if(playerSpeed < 0 && isGrounded || playerSpeed > 0 && isGrounded)
        {
            animator.SetInteger("State", 1); //Set State to Idle1(walk)
        }
        if(playerSpeed == 0 && isGrounded)
        {
            animator.SetInteger("State", 0); //Set State to Idle0(idle)
        }
        if (!Input.GetKey(controlUp))
        {
            rb.velocity = new Vector3(speed, rb.velocity.y, 0); //Move horizontal movement player
        }else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
        
    }

    //Flip the player direction
    void Flip()
    {
        if(speed > 0 && !facingRight || speed < 0 && facingRight)
        {
            facingRight = !facingRight;
            
            Vector3 temp = transform.localScale;
            temp.x *= -1;

            transform.localScale = temp;
        }
    }

    //When the player enter in collision with other Collider2D
    void OnCollisionEnter2D(Collision2D other)
    {        
        if(other.gameObject.tag == "Player")
        {
            if(!isGrounded)
            {
                Debug.Log("Collision Player");
                canDoubleJump = true;
                StartCoroutine(timeManager.DoSlowMotion(TIME_MAX_IN_SLOW_MO));
            }                       
        }        
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            timeManager.ExitSlowMotion();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "SlowMoObjects")
        {
            timeManager.DoSlowMotion();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "SlowMoObjects")
        {
            timeManager.ExitSlowMotion();
        }
    }

    

}
