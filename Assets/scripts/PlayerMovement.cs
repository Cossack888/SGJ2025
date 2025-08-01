using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.3f;

    private Rigidbody2D rb;
    private PlayerInputHandler input;
    public bool isGrounded;

    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        // Obracanie postaci
        if (input.MoveInput.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (input.MoveInput.x < 0 && facingRight)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
        float move = input.MoveInput.x;
        float speed = input.IsSprinting ? runSpeed : walkSpeed;

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (input.IsJumping && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}
