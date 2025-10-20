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

    [SerializeField] private RaycastHit2D isDiagonallyGrounded;
    [SerializeField] private RaycastHit2D isDownGrounded;

    [SerializeField] private bool grabLeft;
    [SerializeField] private bool grabRight;
    [SerializeField] private bool isClimbing;
    [SerializeField] private bool isJumping;

    private void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
        isClimbing = false;
        isJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("GetCanClimb() : " + GetCanClimb());
        Debug.Log("Gravity scale : " + rb.gravityScale);

        if (Input.GetButtonDown("Jump"))
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
            rb.gravityScale = 0f;
            isClimbing = true;
            
            // Checks if the player is touching the wall or not
            if (grabLeft && GetLeftWallDistance() >= 0.01f)
            {
                // Move left while the player is not touching the wall
                transform.Translate(new Vector2(- speed * Time.deltaTime, 0));
            } else if (grabRight && GetRightWallDistance() >= 0.01f)
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
        } else if (rb.linearVelocity.y <= 0)
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);

        // Player moves with "W"/"S" or "up"/"down"
        climbInput = Input.GetAxis("Vertical");
        movement = new Vector2(0, climbInput * speed * Time.deltaTime);

        transform.Translate(movement);
    }

    private void StopClimb()
    {
        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
        isClimbing = false;

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
        // Checks if there is a platform under the player only
        isDownGrounded = Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.down, playerHalfHeight * .75f, LayerMask.GetMask("Ground"));
        
        // "Vector2(-runInput, 0)" is used to help player jumping, even if he has passed a bit of the platform (he has more time to jump)
        // "playerHalfHeight * 1.5f" scales the range of the Raycast if the player scale changes
        isDiagonallyGrounded = Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.down + new Vector2(-runInput, 0), playerHalfHeight * .75f, LayerMask.GetMask("Ground"));

        return (isDownGrounded || isDiagonallyGrounded);
    }

    private bool GetCanClimb()
    {
        grabLeft = GetCanLeftClimb();
        grabRight = GetCanRightClimb();

        return grabLeft || grabRight;
    }

    private bool GetCanLeftClimb()
    {
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.left, playerHalfHeight * .5f, LayerMask.GetMask("Ground"));
    }

    private float GetLeftWallDistance()
    {
        float leftWallDistance = Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.left, playerHalfHeight * .5f, LayerMask.GetMask("Ground")).distance;
        return leftWallDistance;
    }

    private bool GetCanRightClimb()
    {
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.right, playerHalfHeight * .5f, LayerMask.GetMask("Ground"));
    }

    private float GetRightWallDistance()
    {
        float rightWallDistance = Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, Vector2.right, playerHalfHeight * .5f, LayerMask.GetMask("Ground")).distance;
        return rightWallDistance;
    }
}
