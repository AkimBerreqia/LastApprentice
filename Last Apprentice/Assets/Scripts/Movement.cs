using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float speed = 6f;

    [SerializeField] private float playerHalfHeight;
    
    [SerializeField] private Vector2 movement;
    [SerializeField] private float runInput;
    [SerializeField] private float climbInput;
    
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float wallJumpForce = 3f;

    [SerializeField] private bool isClimbing;
    [SerializeField] private bool startGrabbingWall;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool canJump;

    private void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
        isClimbing = false;
        startGrabbingWall = false;
        isJumping = false;
        canJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("GetCanClimb() : " + GetCanClimb());
        //Debug.Log("Gravity scale : " + rb.gravityScale);
        //Debug.Log("rb.linearVelocity.y : " + rb.linearVelocity.y);
        //Debug.Log("GetIsGrounded().distance : " + GetIsDownGrounded().distance);

        if (Input.GetButtonDown("Jump") && canJump)
        {
            isJumping = true;

            if (GetIsGrounded() && !isClimbing)
            {
                Jump();
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
                rb.gravityScale = 0f;
            }
            
            // Checks if the player is touching the wall or not
            if (GetCanLeftClimb() && GetCanLeftClimb().distance >= 0.01f)
            {
                // Move left while the player is not touching the wall
                transform.Translate(new Vector2(- speed * Time.deltaTime, 0));
            } else if (GetCanRightClimb() && GetCanRightClimb().distance >= 0.01f)
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
        } else if (rb.linearVelocityY <= -14f)
        {
            rb.linearVelocityY = -14f;
        } else if (rb.linearVelocity.y <= 0 && !isClimbing)
        {
            isJumping = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime);
        }
    }

    private void Run()
    {
        // Player moves with "A"/"D" or "left"/"right"
        runInput = Input.GetAxis("Horizontal");
        movement.x = runInput * speed * Time.deltaTime;

        transform.Translate(movement);
    }
    private void Climb()
    {
        if (rb.linearVelocity.y < 0 && startGrabbingWall)
        {
            canJump = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y - fallMultiplier * Physics2D.gravity.y * Time.deltaTime);
        } else if (rb.linearVelocity.y >= 0 && startGrabbingWall)
        {
            canJump = true;
            startGrabbingWall = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0f, rb.linearVelocity.y * 0f);
        } else
        {
            // Player moves with "W"/"S" or "up"/"down", only if doesn't slip
            climbInput = Input.GetAxis("Vertical");

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

    private void StopClimb()
    {
        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
        isClimbing = false;
        canJump = true;

        movement.y = 0f;
        transform.Translate(movement);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        if (GetCanLeftClimb())
        {
            rb.AddForce(Vector2.right, ForceMode2D.Impulse);
            movement.x = wallJumpForce * speed * Time.deltaTime;
        }
        else if (GetCanRightClimb())
        {
            rb.AddForce(Vector2.left, ForceMode2D.Impulse);
            movement.x = - wallJumpForce * speed * Time.deltaTime;
        }

        transform.Translate(movement);
        Jump();
    }

    private bool GetIsGrounded()
    {
        return GetIsDownGrounded() || GetIsDiagonallyGrounded();
    }

    private RaycastHit2D GetIsDownGrounded()
    {
        // Checks if there is a platform under the player only
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.down, playerHalfHeight * .75f, LayerMask.GetMask("Ground"));
    }

    private RaycastHit2D GetIsDiagonallyGrounded()
    {
        // "Vector2(-runInput, 0)" is used to help player jumping, even if he has passed a bit of the platform (he has more time to jump)
        // "playerHalfHeight * 1.5f" scales the range of the Raycast if the player scale changes
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.down + new Vector2(-runInput, 0), playerHalfHeight * .75f, LayerMask.GetMask("Ground"));
    }

    private bool GetCanClimb()
    {
        return GetCanLeftClimb() || GetCanRightClimb();
    }

    private RaycastHit2D GetCanLeftClimb()
    {
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.left, playerHalfHeight * .5f, LayerMask.GetMask("Ground"));
    }

    private RaycastHit2D GetCanRightClimb()
    {
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.right, playerHalfHeight * .5f, LayerMask.GetMask("Ground"));
    }
}
