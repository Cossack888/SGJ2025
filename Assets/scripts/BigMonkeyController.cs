using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BigMonkeyController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    [Header("Climb Grab")]
    public float gravityRestoreDelay = 0.5f;
    public float climbSpeed = 2f;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private Animator animator;

    private bool facingRight = true;
    private bool isGrounded;
    private bool hasJumpedThisFrame = false;
    private bool isGrabbed = false;
    private bool isLedge = false;
    private Collider2D currentClimbTarget;
    private Bounds climbBounds;
    private bool free = true;
    public bool jumpFromLedge;
    private float jumpFromLedgeTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
        animator = GetComponentInChildren<Animator>();

        inputHandler.InteractReleased += InteractReleased;
        inputHandler.JumpPressed += Jump;
        inputHandler.Menu += MenuOpen;
        inputHandler.MenuClose += MenuClose;
        inputHandler.ReleaseFromNet += SetFree;
    }
    public void SetFree(bool state)
    {
        free = state;
    }
    private void Update()
    {
        if (free)
        {
            HandleFlip();

            if (inputHandler.IsAttacking)
                Attack();

            if (inputHandler.IsInteracting)
                Interact();

            if (jumpFromLedge)
            {
                jumpFromLedgeTimer += Time.deltaTime;
                if (jumpFromLedgeTimer > 1f)
                {
                    jumpFromLedge = false;
                    jumpFromLedgeTimer = 0f;
                }
            }

            if (isGrabbed && !isLedge && !jumpFromLedge)
            {
                float vertical = inputHandler.MoveInput.y;
                Vector2 newPosition = rb.position + Vector2.up * vertical * climbSpeed * Time.deltaTime;
                float clampedY = Mathf.Clamp(newPosition.y, climbBounds.min.y, climbBounds.max.y);
                rb.MovePosition(new Vector2(rb.position.x, clampedY));
            }
            UpdateAnimator();
        }
    }

    private void FixedUpdate()
    {
        UpdateGrounded();
        if (free)
        {
            if (!hasJumpedThisFrame)
                HandleMovement();
        }


        hasJumpedThisFrame = false;
    }

    private void HandleMovement()
    {
        if (isGrabbed)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float speed = inputHandler.IsSprinting ? runSpeed : walkSpeed;
        float inputX = inputHandler.MoveInput.x;
        float velocityX = inputX * speed;
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
    }
    private void UpdateAnimator()
    {
        if (isGrounded)
        {
            animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
        }
        else if (!isGrabbed && !isLedge)
        {
            animator.SetFloat("speed", 0f);
            animator.SetBool("Jumping", rb.linearVelocity.y > 0.1f);
            animator.SetBool("Falling", rb.linearVelocity.y < -0.1f);
        }
    }
    private void Jump()
    {
        if (hasJumpedThisFrame) return;
        if (!free) return;
        if (IsGrounded() || isGrabbed)
        {
            hasJumpedThisFrame = true;

            float finalJumpForce = jumpForce;

            // Sprawd� k�t pod postaci� tylko gdy jest na ziemi
            /*if (IsGrounded())
            {
                RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance + 0.1f, groundLayer);
                if (hit.collider != null)
                {
                    Vector2 normal = hit.normal;
                    float angleFromUp = Vector2.Angle(normal, Vector2.up); // 0� = pion, 90� = poziom

                    if (angleFromUp > 10f)
                    {
                        finalJumpForce *= 1.7f; // Wzmocnij skok
                        Debug.Log($"Skok wzmacniany: k�t {angleFromUp:F1}�, nowa si�a: {finalJumpForce}");
                    }
                }
            }*/

            if (isGrabbed)
            {
                ReleaseGrabImmediate();
                jumpFromLedge = true;
                float inputX = inputHandler.MoveInput.x;
                float speed = inputHandler.IsSprinting ? runSpeed : walkSpeed;
                float jumpVelocityX = inputX * speed;
                rb.gravityScale = 1f;
                rb.linearVelocity = new Vector2(jumpVelocityX, finalJumpForce);
                return;
            }

            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalJumpForce);

        }
    }


    private void Interact()
    {
        if (jumpFromLedge) return;

        if (!isGrabbed && currentClimbTarget != null)
            Grab();
    }

    private void InteractReleased()
    {
        if (isGrabbed)
            ReleaseGrab();
    }

    private void Grab()
    {
        isGrabbed = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        if (currentClimbTarget != null)
        {
            climbBounds = currentClimbTarget.bounds;
            isLedge = currentClimbTarget.GetComponent<Ledge>() != null;
            if (isLedge)
            {
                animator.SetBool("Grabing", true);
            }
            else
            {
                animator.SetBool("Hanging", true);
            }
        }
    }

    private void ReleaseGrab()
    {
        isGrabbed = false;
        rb.gravityScale = 1f;
        animator.SetBool("Grabing", false);
        animator.SetBool("Hanging", false);
    }

    private void ReleaseGrabImmediate()
    {
        isGrabbed = false;
        rb.gravityScale = 1f;
        animator.SetBool("Grabing", false);
        animator.SetBool("Hanging", false);
    }

    private void HandleFlip()
    {
        if (isGrabbed) return;

        float velocityX = rb.linearVelocity.x;

        if (Mathf.Abs(velocityX) < 02f) return;

        if (velocityX > 0 && !facingRight)
            Flip();
        else if (velocityX < 0 && facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void UpdateGrounded()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private bool IsGrounded()
    {
        return isGrounded || isGrabbed;
    }

    private void Attack()
    {
        animator.SetTrigger("Punch");
    }

    public void TriggerActionGame()
    {

    }


    private void OnDestroy()
    {
        inputHandler.InteractReleased -= InteractReleased;
        inputHandler.JumpPressed -= Jump;
    }

    public void NotifyEnteredGrabZone(Collider2D climbable)
    {

        currentClimbTarget = climbable;
    }

    public void NotifyExitedGrabZone(Collider2D climbable)
    {
        jumpFromLedge = false;
        if (climbable == currentClimbTarget)
        {
            currentClimbTarget = null;
            ReleaseGrabImmediate();
        }
    }
    public void MenuOpen()
    {
        LevelManager.Instance.OpenMenu();
        if (inputHandler != null)
        {
            MenuManager.Instance.AssignControllingInput(inputHandler);
        }
    }

    public void MenuClose()
    {
        LevelManager.Instance.CloseMenu();
    }
}
