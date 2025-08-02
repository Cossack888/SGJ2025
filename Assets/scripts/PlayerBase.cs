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
    protected virtual bool IsGrabbed => false;
    protected Rigidbody2D rb;
    protected PlayerInputHandler inputHandler;

    public bool isGrounded;
    protected bool facingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();

        inputHandler.InteractPressed += Interact;
        inputHandler.InteractReleased += OnInteractReleased;
    }

    protected virtual void Update()
    {
        HandleFlip();

        if (inputHandler.IsJumping && (IsGrounded() || IsGrabbed))
            Jump();

        if (inputHandler.IsAttacking)
            Attack();
    }

    protected virtual void FixedUpdate()
    {
        UpdateGrounded();
        HandleMovement();
    }

    protected virtual void HandleMovement()
    {
        if (!CanMove())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float speed = inputHandler.IsSprinting ? runSpeed : walkSpeed;
        Vector2 move = new Vector2(inputHandler.MoveInput.x * speed, rb.linearVelocity.y);
        rb.linearVelocity = move;
    }

    protected virtual bool CanMove()
    {
        return true;
    }

    private void HandleFlip()
    {
        if (inputHandler.MoveInput.x > 0 && !facingRight)
            Flip();
        else if (inputHandler.MoveInput.x < 0 && facingRight)
            Flip();
    }

    protected virtual bool IsGrounded()
    {
        return isGrounded;
    }

    protected void UpdateGrounded()
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

    // Abstract for monkey-specific logic
    protected abstract void Jump();
    protected abstract void Attack();
    protected abstract void SpecialAbility();
    protected abstract void Interact();

    // Optional to override
    protected virtual void OnInteractReleased() { }
}
