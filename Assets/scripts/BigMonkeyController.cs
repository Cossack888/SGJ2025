using UnityEngine;

public class BigMonkeyController : MonkeyControllerBase
{
    [Header("Climb Grab")]
    public float gravityRestoreDelay = 0.5f;

    private bool isGrabbed = false;
    protected override bool IsGrabbed => isGrabbed;

    private Collider2D currentClimbTarget;
    private float gravityRestoreTimer = 0f;
    private bool pendingGravityRestore = false;
    private bool wantsToGrab = false;

    protected override void Awake()
    {
        base.Awake();
        inputHandler.InteractReleased += InteractReleased;
    }

    protected override void Update()
    {
        base.Update();

        if (wantsToGrab && !isGrabbed && currentClimbTarget != null)
        {
            Grab();
            wantsToGrab = false;
        }

        if (isGrabbed)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (pendingGravityRestore)
        {
            gravityRestoreTimer -= Time.deltaTime;
            if (gravityRestoreTimer <= 0f)
            {
                rb.gravityScale = 1f;
                pendingGravityRestore = false;
            }
        }
    }

    protected override bool CanMove()
    {
        return !isGrabbed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            currentClimbTarget = other;

            if (wantsToGrab && !isGrabbed)
            {
                Grab();
                wantsToGrab = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentClimbTarget)
        {
            currentClimbTarget = null;
        }
        ReleaseGrabImmediate();
    }

    protected override void Interact()
    {
        if (!isGrabbed && currentClimbTarget != null)
        {
            Grab();
        }
        else
        {
            wantsToGrab = true;
        }
    }

    private void InteractReleased()
    {
        if (isGrabbed)
        {
            ReleaseGrab();
        }

        wantsToGrab = false;
    }

    protected override void Jump()
    {
        if (isGrabbed)
        {
            ReleaseGrabImmediate();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void Grab()
    {
        isGrabbed = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        pendingGravityRestore = false;
    }

    private void ReleaseGrab()
    {
        isGrabbed = false;
        gravityRestoreTimer = gravityRestoreDelay;
        pendingGravityRestore = true;
    }

    private void ReleaseGrabImmediate()
    {
        isGrabbed = false;
        rb.gravityScale = 1f;
        pendingGravityRestore = false;
    }

    protected override void Attack()
    {
        Debug.Log("Big monkey attacks!");
    }

    protected override void SpecialAbility()
    {
        Debug.Log("Big monkey special!");
    }

    protected override bool IsGrounded()
    {
        return base.IsGrounded() || isGrabbed || pendingGravityRestore;
    }
}
