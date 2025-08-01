using UnityEngine;

public class SmallMonkeyController : MonkeyControllerBase
{
    [Header("Banana Throwing")]
    public GameObject bananaPrefab;
    public Transform throwPoint;
    public float throwForce = 10f;
    public float throwCooldown = 0.5f;

    [Header("Trajectory")]
    public LineRenderer trajectoryRenderer;
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.1f;
    public float aimSensitivity = 0.2f;

    private float lastThrowTime;

    protected override void Update()
    {
        base.Update();
        HandleAiming();
    }

    protected override void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    protected override void Attack()
    {
        if (Time.time - lastThrowTime < throwCooldown)
            return;

        lastThrowTime = Time.time;

        Vector2 direction = GetAimDirection();
        GameObject banana = Instantiate(bananaPrefab, throwPoint.position, Quaternion.identity);
        banana.GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * throwForce;
    }

    private void HandleAiming()
    {
        Vector2 look = inputHandler.LookInput;

        if (look.magnitude > aimSensitivity)
        {
            Vector2 direction = look.normalized;
            ShowTrajectory(direction);
        }
        else
        {
            trajectoryRenderer.positionCount = 0;
        }
    }

    private Vector2 GetAimDirection()
    {
        Vector2 look = inputHandler.LookInput;

        if (look.magnitude > 0.1f)
            return look.normalized;
        else
            return facingRight ? Vector2.right : Vector2.left;
    }

    private void ShowTrajectory(Vector2 direction)
    {
        if (bananaPrefab == null || trajectoryRenderer == null) return;

        trajectoryRenderer.positionCount = trajectoryPoints;

        Vector3[] points = new Vector3[trajectoryPoints];
        Vector3 startPos = throwPoint.position;
        Vector2 velocity = direction.normalized * throwForce;
        float bananaGravityScale = 1f;
        Rigidbody2D bananaRb = bananaPrefab.GetComponent<Rigidbody2D>();
        if (bananaRb != null)
            bananaGravityScale = bananaRb.gravityScale;

        Vector2 gravity = Physics2D.gravity * bananaGravityScale;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * trajectoryTimeStep;
            Vector2 displacement = velocity * t + 0.5f * gravity * t * t;
            points[i] = startPos + (Vector3)displacement;
        }

        trajectoryRenderer.SetPositions(points);
    }

    protected override void SpecialAbility()
    {
        Debug.Log("Use Special Ability");
    }
}
