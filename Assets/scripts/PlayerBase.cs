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
    public float airControlMultiplier = 0.2f;

    protected Rigidbody2D rb;
    protected PlayerInputHandler inputHandler;
    protected bool facingRight = true;
    protected bool hasJumpedThisFrame = false;
    public bool isGrounded;

    private float cachedJumpVelocityX = 0f;

    protected virtual bool IsGrabbed => false;

    /// <summary>
    /// Can be overridden by subclasses (e.g. BigMonkeyController)
    /// </summary>
    protected virtual bool IsJumpingFromLedge => false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    protected virtual void Update()
    {
        if (!IsGrabbed)
            HandleFlip();

        if (inputHandler.IsAttacking)
            Attack();

        if (inputHandler.IsInteracting)
            Interact();
    }

    protected virtual void FixedUpdate()
    {
        UpdateGrounded();

        if (IsJumpingFromLedge)
            return;

        if (!hasJumpedThisFrame)
            HandleMovement();

        hasJumpedThisFrame = false;
    }

    protected virtual void HandleMovement()
    {
        if (IsGrabbed && !hasJumpedThisFrame)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!CanMove() && !hasJumpedThisFrame)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float speed = inputHandler.IsSprinting ? runSpeed : walkSpeed;
        float moveInputX = inputHandler.MoveInput.x;

        float velocityX;

        if (IsGrounded())
        {
            velocityX = moveInputX * speed;
            cachedJumpVelocityX = velocityX;
        }
        else
        {
            float airInfluence = moveInputX * speed * airControlMultiplier;
            velocityX = cachedJumpVelocityX + airInfluence;
        }

        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
    }

    protected virtual bool CanMove() => true;

    private void HandleFlip()
    {
        float inputX = inputHandler.MoveInput.x;
        if (inputX > 0 && !facingRight)
            Flip();
        else if (inputX < 0 && facingRight)
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

    protected abstract void Jump();
    protected abstract void Attack();
    protected abstract void SpecialAbility();
    protected abstract void Interact();
}
