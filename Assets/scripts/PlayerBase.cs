using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class MonkeyControllerBase : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    protected Rigidbody2D rb;
    protected PlayerInputHandler inputHandler;

    protected bool isGrounded;
    protected bool facingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    protected virtual void Update()
    {
        HandleFlip();

        if (inputHandler.IsJumping && isGrounded)
            Jump();

        if (inputHandler.IsAttacking)
            Attack();

        if (inputHandler.IsInteracting)
            SpecialAbility();
    }

    protected virtual void FixedUpdate()
    {
        UpdateGrounded();

        float speed = inputHandler.IsSprinting ? runSpeed : walkSpeed;
        Vector2 move = new Vector2(inputHandler.MoveInput.x * speed, rb.linearVelocityY);
        rb.linearVelocity = move;
    }

    private void HandleFlip()
    {
        if (inputHandler.MoveInput.x > 0 && !facingRight)
            Flip();
        else if (inputHandler.MoveInput.x < 0 && facingRight)
            Flip();
    }

    private void UpdateGrounded()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    protected abstract void Jump();
    protected abstract void Attack();
    protected abstract void SpecialAbility();
}
