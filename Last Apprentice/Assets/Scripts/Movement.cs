using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float minLinearVelocityY = -14f;

    [SerializeField] private float playerHalfHeight;
    
    [SerializeField] private Vector2 movement;
    [SerializeField] private float runInput;
    [SerializeField] private float climbInput;
    
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float wallJumpForce = 8f;

    [SerializeField] private bool isClimbing;
    [SerializeField] private RaycastHit2D canClimbRight;
    [SerializeField] private RaycastHit2D canClimbLeft;
    [SerializeField] private bool startGrabbingWall;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private bool isWallJumpingRight;
    [SerializeField] private bool isWallJumpingLeft;
    [SerializeField] private bool canJump;
    
    [SerializeField] private bool coyoteTimeAvailable;
    [SerializeField] private bool coyoteTimeCanStart;
    [SerializeField] private float coyoteTimeCurrentTime;
    [SerializeField] private float coyoteTimeDuration = 0.2f;

    [SerializeField] private RaycastHit2D rightWall;
    [SerializeField] private bool touchRightWall;
    [SerializeField] private RaycastHit2D leftWall;
    [SerializeField] private bool touchLeftWall;

    [SerializeField] private RaycastHit2D ceiling;
    [SerializeField] private bool touchCeiling;
    [SerializeField] private RaycastHit2D floor;
    [SerializeField] private bool touchFloor;

    private void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
        startGrabbingWall = false;
        isJumping = false;
        isWallJumping = false;
        isWallJumpingRight = false;
        isWallJumpingLeft = false;
        canJump = true;
        coyoteTimeAvailable = false;
        coyoteTimeCanStart = false;

        coyoteTimeCurrentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("GetCanClimb() : " + GetCanClimb());
        //Debug.Log("Gravity scale : " + rb.gravityScale);
        //Debug.Log("rb.linearVelocity.y : " + rb.linearVelocity.y);
        //Debug.Log("GetIsGrounded().distance : " + GetIsDownGrounded().distance);
        Debug.Log(ceiling.distance);
        Debug.Log(floor.distance);
        
        CoyoteTime();

        if (Input.GetButtonDown("Jump") && canJump)
        {
            isJumping = true;

            if ((GetIsGrounded(playerHalfHeight * .25f) || coyoteTimeAvailable) && !isClimbing)
            {
                Jump(jumpForce);
            } else if (GetCanClimb() && isClimbing)
            {
                StopClimb();
                WallJump();
            }
        }

        if (Input.GetButton("Climb") && GetCanClimb() && !isJumping)
        {
            if (!isClimbing)
            {
                startGrabbingWall = true;
                isClimbing = true;
                runInput = 0;
                rb.gravityScale = 0f;
            }
            
            // Checks if the player is touching the wall or not
            if (canClimbLeft && canClimbLeft.distance >= 0.01f)
            {
                // Move left while the player is not touching the wall
                transform.Translate(new Vector2(- speed * Time.deltaTime, 0));
            } else if (canClimbRight && canClimbRight.distance >= 0.01f)
            {
                // Move right while the player is not touching the wall
                transform.Translate(new Vector2(speed * Time.deltaTime, 0));
            } else
            {
                Climb();
            }
        } else if ((Input.GetButtonUp("Climb") || !GetCanClimb()) && isClimbing)
        {
            StopClimb();
        } else
        {
            Run();
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * .1f);
            coyoteTimeAvailable = false;
        } else if (rb.linearVelocityY <= minLinearVelocityY)
        {
            rb.linearVelocityY = minLinearVelocityY;
        } else if (rb.linearVelocity.y <= 0 && !isClimbing)
        {
            isJumping = false;
            isWallJumping = false;
            isWallJumpingLeft = false;
            isWallJumpingRight = false;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime);
        }
    }

    private void Run()
    {
        // Player moves with "A"/"D" or "left"/"right"
        runInput = Input.GetAxis("Horizontal");

        rightWall = GetTouchWall(Vector2.right, playerHalfHeight * .3f);
        leftWall = GetTouchWall(Vector2.left, playerHalfHeight * .3f);

        touchRightWall = rightWall;
        touchLeftWall = leftWall;

        // Check the collision with the walls (right and left)
        if ((touchRightWall && runInput > 0) || (touchLeftWall && runInput < 0))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0f, rb.linearVelocity.y);
        } else
        {
            // When wall jumping, the player can only run if it is in the same direction
            // as the wall jump direction
            if (!isWallJumping)
            {
                movement.x = runInput * speed * Time.deltaTime;
                transform.Translate(movement);
            }
            else if (isJumping || (isWallJumpingRight && runInput > 0) || (isWallJumpingLeft && runInput < 0))
            {
                movement.x = runInput * jumpForce * Time.deltaTime;
                transform.Translate(movement);
            }
        }
    }
    private void Climb()
    {
        // If there is a gap between the wall and the player, then the player is attracted to the wall
        // The two first conditions makes the player slip on the wall when grabbing it
        if (rb.linearVelocity.y < 0 && startGrabbingWall)
        {
            canJump = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y - fallMultiplier * Physics2D.gravity.y * Time.deltaTime);
        } else if (rb.linearVelocity.y >= 0 && startGrabbingWall)
        {
            canJump = true;
            startGrabbingWall = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
        } else
        {
            // Player moves with "W"/"S" or "up"/"down", only if doesn't slip
            climbInput = Input.GetAxis("Vertical");

            ceiling = GetTouchWall(Vector2.up, playerHalfHeight * .3f);
            floor = GetTouchWall(Vector2.down, playerHalfHeight * .3f);

            touchCeiling = ceiling;
            touchFloor = floor;

            if ((touchCeiling && climbInput > 0) || (touchFloor && climbInput < 0))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
            } else
            {
                // Climb up slower than climb down
                if (climbInput > 0)
                {
                    movement = new Vector2(0, .5f * climbInput * speed * Time.deltaTime);
                } else
                {
                    movement = new Vector2(0, climbInput * speed * Time.deltaTime);
                }

                transform.Translate(movement);
            }
        }
    }
    
    private void StopClimb()
    {
        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
        isClimbing = false;
        canJump = true;

        movement.y = 0f;
        transform.Translate(movement);
    }

    private void CoyoteTime()
    {
        if (!GetIsGrounded(playerHalfHeight * .1f) && coyoteTimeCanStart && coyoteTimeAvailable)
        {
            coyoteTimeCurrentTime = 0f;
            coyoteTimeCanStart = false;
        } else if (!GetIsGrounded(playerHalfHeight * .1f) && !coyoteTimeCanStart && coyoteTimeAvailable && coyoteTimeDuration - coyoteTimeCurrentTime > 0)
        {
            coyoteTimeCurrentTime += Time.deltaTime;
        } else if (GetIsGrounded(playerHalfHeight * .1f))
        {
            coyoteTimeAvailable = true;
            coyoteTimeCanStart = true;
        } else if (isClimbing || isJumping || (coyoteTimeDuration - coyoteTimeCurrentTime <= 0 && !GetIsGrounded(playerHalfHeight * .1f)))
        {
            coyoteTimeAvailable = false;
            coyoteTimeCanStart = false;
        }
    }

    private void Jump(float force)
    {
        coyoteTimeAvailable = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        isWallJumping = true;

        if (canClimbLeft)
        {
            isWallJumpingRight = true;
            rb.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
            //movement.x = wallJumpForce * speed * Time.deltaTime;
        } else if (canClimbRight)
        {
            isWallJumpingLeft = true;
            rb.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
            //movement.x = - wallJumpForce * speed * Time.deltaTime;
        }

        //transform.Translate(movement);
        Jump(wallJumpForce);
    }

    private RaycastHit2D GetTouchWall(Vector2 direction, float distance)
    {
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size * .75f, 0, direction, distance, LayerMask.GetMask("Ground"));
    }

    private RaycastHit2D GetIsGrounded(float distance)
    {
        // Checks if there is a platform under the player only
        return GetCanTriggerWall(Vector2.down, distance);
    }

    private bool GetCanClimb()
    {
        canClimbRight = GetCanTriggerWall(Vector2.right, playerHalfHeight * .5f);
        canClimbLeft = GetCanTriggerWall(Vector2.left, playerHalfHeight * .5f);

        return canClimbRight || canClimbLeft;
    }

    private RaycastHit2D GetCanTriggerWall(Vector2 direction, float distance)
    {
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, direction, distance, LayerMask.GetMask("Ground"));
    }
}
