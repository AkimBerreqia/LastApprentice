using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 movement;
    private float runInput;
    private float climbInput;
    private float wallGrabDirection;
    private float playerHalfHeight;

    private RaycastHit2D isDiagonallyGrounded;
    private RaycastHit2D isDownGrounded;

    private void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Check the wall grab direction, to know on which side the player is able to climb
        if (runInput != 0)
        {
            wallGrabDirection = runInput;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (GetIsGrounded())
            {
                Jump();
            } else if (GetCanClimb())
            {
                WallJump();
            }
        }

        if (Input.GetButton("Climb") && GetCanClimb())
        {
            Climb();
        } else
        {
            Run();
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * .1f);
        } else if (rb.linearVelocity.y <= 0)
        {
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

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        movement.x = wallGrabDirection * speed * Time.deltaTime;
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
        return Physics2D.BoxCast(transform.position, spriteRenderer.bounds.size, 0, new Vector2(wallGrabDirection, 0), playerHalfHeight * .75f, LayerMask.GetMask("Ground"));
    }
}
