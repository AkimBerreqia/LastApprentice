using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 movement;
    private float input;
    private float playerHalfHeight;

    private void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Player moves with "A"/"D" or "left"/"right"
        input = Input.GetAxis("Horizontal");
        movement.x = input * speed * Time.deltaTime;

        transform.Translate(movement);

        if (Input.GetButtonDown("Jump") && GetIsGrounded())
        {
            Jump();
        }
    }

    private bool GetIsGrounded()
    {
        // "Vector2(-input, 0)" is used to help player jumping, even if he has passed a bit of the platform (he has more time to jump)
        // "playerHalfHeight * 1.5f" scales the range of the Raycast if the player scale changes
        return Physics2D.Raycast(transform.position, Vector2.down + new Vector2(-input, 0), playerHalfHeight * 1.5f, LayerMask.GetMask("Ground"));
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
