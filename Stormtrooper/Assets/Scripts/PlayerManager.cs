using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script managing players

public class PlayerManager : MonoBehaviour {

    //Constante
    private static readonly float TIME_MAX_IN_SLOW_MO = 0.6f;
    [Range(5, 20)]
    [Tooltip("Speed of the player1, value 0 means the player1 can't move.")]
    public float speedX = 6;        //Change the speed value
    [Tooltip("Force of the player1 jump, value 0 means the player1 won't jump. Recommanded between 500 and 1000")]
    public float jumpSpeedY = 15;    //Change the jump speed value
    public float aerialControlSpeed = 0.1f; //Change how the player has control when jumping
    public float playerNumber;

    public  bool facingRight, isGrounded, canDoubleJump;
    private float speed;

    public KeyCode controlLeft,controlRight, controlUp, controlDown;
    private enum KeyEvent { KEY_DOWN, KEY, KEY_UP };
    private enum HDirection { LEFT, RIGHT, UP ,DOWN };

    private Animator animator;
    private Rigidbody2D rb;

    public CollisionObject collision;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        facingRight = false;
        canDoubleJump = false;
	}


    void Update()
    {
        //player jump
        if (GetInputKey(controlUp, KeyEvent.KEY_DOWN) && (isGrounded || canDoubleJump))
        {
            Jump();
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        MovePlayer();      //handle player1 movement
        Flip();            //change the direction of the animation if needed
    }

    private void Jump()
    {
        rb.velocity = (new Vector2(rb.velocity.x, jumpSpeedY)); //add a velocity Y
        canDoubleJump = false;
    }

    //TEST PROPULSION VELOCITY
    private void Propulse(HDirection direction)
    {
        rb.velocity = (new Vector2(((direction == HDirection.RIGHT) ? 1 : -1) * speedX * 3.0f, jumpSpeedY * 1.25f)); //add a velocity Y

        canDoubleJump = false;
    }

    private void SuperJump(HDirection direction)
    {
        rb.velocity = (new Vector2(rb.velocity.x, ((direction == HDirection.UP) ? 1 : -1) * jumpSpeedY * 2f));
    }

    //Code for player movement
    private void MovePlayer()
    {
        bool rightKey = Input.GetKey(controlRight);
        bool leftKey = Input.GetKey(controlLeft);

        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        float velocityX = rigidBody.velocity.x;

        if (rightKey == leftKey || Math.Abs(velocityX) > speedX)
        {
            if (isGrounded)
            {
                velocityX = 0;
            }
            else
            {
                if (velocityX < 0)
                {
                    velocityX = Mathf.Clamp(velocityX , float.NegativeInfinity, 0);
                }
                else if (velocityX > 0)
                {
                    velocityX = Mathf.Clamp(velocityX , 0, float.PositiveInfinity);
                }
            }
        }
        else if (rightKey && !leftKey)
        {
            if (isGrounded)
            {
                velocityX = speedX;
            }
            else
            {
                velocityX += (speedX * aerialControlSpeed);
                velocityX = Mathf.Clamp(velocityX, -speedX, speedX);
            }
        }
        else if (!rightKey && leftKey)
        {
            if (isGrounded)
            {
                velocityX = -speedX;
            }
            else
            {
                velocityX -= (speedX * aerialControlSpeed);
                velocityX = Mathf.Clamp(velocityX, -speedX, speedX);
            }
        }

        //velocityX = Mathf.Clamp(velocityX, -speedX, speedX);
        rigidBody.velocity = new Vector2(velocityX, rigidBody.velocity.y);
        speed = rigidBody.velocity.x;
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
            canDoubleJump = true;

            if (isGrounded || other.gameObject.GetComponent<PlayerManager>().isGrounded)
                return;

            ContactPoint2D contactPoint = other.GetContact(0);
            // La normale pointe vers l'objet courant.
            Vector2 normal = contactPoint.normal;

            // Vertical Collision if angle greater than 45°
            float tan = normal.y / normal.x;
            bool isVerticalCollision = Math.Abs(tan) > (Math.Sqrt(2) / 2);

            // In a vertical collision, first collider is on the left and the
            // second is on the right. In a vertical collisition, the first collider
            // is on the top and the second is at the bottom.
            bool isFirstCollider = isVerticalCollision ? (contactPoint.normal.y > 0)
                                                       : (contactPoint.normal.x < 0);

            PlayerManager otherPlayerManager = other.gameObject.GetComponent<PlayerManager>();

            PlayerManager firstCollider = isFirstCollider ? this : otherPlayerManager;
            PlayerManager otherCollider = isFirstCollider ? otherPlayerManager : this;

            if (CollisionManager.GetCollisionObject(firstCollider, otherCollider) != null)
                // Collision déjà initiée par l'autre objet.
                return;

            EnterComboState(firstCollider, otherCollider, isVerticalCollision);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {


        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Exit colision PLAYER");
            TimeManager.ExitSlowMotion();
            canDoubleJump = false;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "SlowMoObjects")
        {
            TimeManager.DoSlowMotion();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "SlowMoObjects")
        {
            TimeManager.ExitSlowMotion();
        }
    }

    private bool GetInputKey(KeyCode keyCode, KeyEvent keyEvent)
    {
        // Pendant la collision, la gestion du Input est faite par la collision.
        if (collision != null)
        {
            return false;
        }
            
        switch (keyEvent)
        {
            case KeyEvent.KEY:
                return Input.GetKey(keyCode);
            case KeyEvent.KEY_DOWN:
                return Input.GetKeyDown(keyCode);
            case KeyEvent.KEY_UP:
                return Input.GetKeyUp(keyCode);
            default:
                return false;
        }
    }

    static void EnterComboState(PlayerManager firstCollider, PlayerManager secondCollider, bool isVerticalCollision)
    {
        CollisionObject collision = CollisionManager.CreateCollisionObject(firstCollider, secondCollider);

        firstCollider.collision = collision;
        secondCollider.collision = collision;

        firstCollider.StartCoroutine(ComboStateCoroutine(firstCollider, secondCollider, isVerticalCollision));
    }

    static private IEnumerator ComboStateCoroutine(PlayerManager firstPlayer, PlayerManager secondPlayer, bool isVerticalCollision)
    {
        firstPlayer.StartCoroutine(TimeManager.DoSlowMotion(TIME_MAX_IN_SLOW_MO));

        KeyCode[] firstPlayerKeys = { firstPlayer.controlLeft, firstPlayer.controlRight,
                                     firstPlayer.controlUp, firstPlayer.controlDown };
        KeyCode[] secondPlayerKeys = { secondPlayer.controlLeft, secondPlayer.controlRight,
                                      secondPlayer.controlUp, secondPlayer.controlDown };

        List<KeyCode> firstPlayerEnteredKeys = new List<KeyCode>();
        List<KeyCode> secondPlayerEnteredKeys = new List<KeyCode>();
        while (true)
        {
            //Listen to first player input.
            for (int i = 0; i < firstPlayerKeys.Length; i++)
            {
                if (Input.GetKeyUp(firstPlayerKeys[i]))
                    firstPlayerEnteredKeys.Remove(firstPlayerKeys[i]);
                if (Input.GetKeyDown(firstPlayerKeys[i]))
                    firstPlayerEnteredKeys.Add(firstPlayerKeys[i]);
            }

            //Listen to second player input.
            for (int i = 0; i < secondPlayerKeys.Length; i++)
            {
                if (Input.GetKeyUp(secondPlayerKeys[i]))
                    secondPlayerEnteredKeys.Remove(secondPlayerKeys[i]);
                if (Input.GetKeyDown(secondPlayerKeys[i]))
                    secondPlayerEnteredKeys.Add(secondPlayerKeys[i]);
            }

            if (!TimeManager.isSlowedDown)
                break;

            if (firstPlayerEnteredKeys.Count != 0 && secondPlayerEnteredKeys.Count != 0)
                break;

            yield return null;
            continue;
        }

        TimeManager.ExitSlowMotion();

        // Process key combination.

        if (!isVerticalCollision)
        {
            if (firstPlayerEnteredKeys.Contains(firstPlayer.controlRight) && secondPlayerEnteredKeys.Contains(secondPlayer.controlRight))
            {
                Debug.Log("Propulse RIGHT");
                secondPlayer.Propulse(HDirection.RIGHT);
            }

            if (firstPlayerEnteredKeys.Contains(firstPlayer.controlLeft) && secondPlayerEnteredKeys.Contains(secondPlayer.controlLeft))
            {
                Debug.Log("Propulse LEFT");
                firstPlayer.Propulse(HDirection.LEFT);
            }

            if (firstPlayerEnteredKeys.Contains(firstPlayer.controlLeft) && secondPlayerEnteredKeys.Contains(secondPlayer.controlRight))
            {
                Debug.Log("Propulse EACH");
                firstPlayer.Propulse(HDirection.LEFT);
                secondPlayer.Propulse(HDirection.RIGHT);
            }
        }
        else
        {
            if (firstPlayerEnteredKeys.Contains(firstPlayer.controlUp) && secondPlayerEnteredKeys.Contains(secondPlayer.controlUp))
            {
                Debug.Log("SuperJump UP");
                firstPlayer.SuperJump(HDirection.UP);
            }

            if (firstPlayerEnteredKeys.Contains(firstPlayer.controlDown) && secondPlayerEnteredKeys.Contains(secondPlayer.controlDown))
            {
                Debug.Log("SuperJump DOWN");
                secondPlayer.SuperJump(HDirection.DOWN);
            }
        }

        ExitComboState(firstPlayer, secondPlayer);
    }
    static void ExitComboState(PlayerManager firstCollider, PlayerManager secondCollider)
    {
        firstCollider.collision = null;
        secondCollider.collision = null;

        CollisionManager.DeleteCollisionObject(firstCollider, secondCollider);


        //Give player opportunity to jump after Exiting Combo.
        //firstCollider.canDoubleJump = true;
        //secondCollider.canDoubleJump = true;
    }

    void UpdateHorizontalMovement()
    {
        bool rightKey = Input.GetKey(controlRight);
        bool leftKey = Input.GetKey(controlLeft);

        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        float velocityX = rigidBody.velocity.x;

        if (rightKey && !leftKey)
        {
            if (isGrounded)
            {
                velocityX = speedX;
            }
            else
            {
                velocityX += Time.deltaTime;
            }
        }
        else if (!rightKey && leftKey)
        {
            if (isGrounded)
            {
                velocityX = -speedX;
            }
            else
            {
                velocityX -= Time.deltaTime;
            }
        }
        else if (rightKey == leftKey)
        {
            if (isGrounded)
            {
                velocityX = 0;
            }
            else
            {
                if (velocityX < 0)
                {
                    velocityX = Mathf.Clamp(velocityX + Time.deltaTime , float.NegativeInfinity, 0);
                }
                else if (velocityX > 0)
                {
                    velocityX = Mathf.Clamp(velocityX - Time.deltaTime , 0, float.PositiveInfinity);
                }
            }
        }
        velocityX = Mathf.Clamp(velocityX, -speedX, speedX);
        rigidBody.velocity = new Vector2(velocityX, rigidBody.velocity.y);
    }
}
