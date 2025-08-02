using UnityEngine;

public class SmallMonkeyController : MonkeyControllerBase
{
    [Header("Banana Throwing")]
    public GameObject bananaPrefab;
    public Transform throwPoint;
    public float minThrowForce = 5f;
    public float maxThrowForce = 15f;
    public float throwCooldown = 0.5f;
    public float maxChargeTime = 1.5f;

    [Header("Trajectory")]
    public LineRenderer trajectoryRenderer;
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.1f;

    private float lastThrowTime;
    private bool isCharging;
    private float chargeStartTime;
    private Vector2 lastValidAimDirection = Vector2.right;

    protected override void Awake()
    {
        base.Awake();
        inputHandler.AttackPressed += OnAttackPress;
        inputHandler.AttackReleased += OnAttackRelease;
    }
    private void Start()
    {
        lastValidAimDirection = facingRight ? Vector2.right : Vector2.left;
    }
    protected override void Update()
    {
        base.Update();

        if (isCharging)
        {
            float heldTime = Mathf.Clamp(Time.time - chargeStartTime, 0f, maxChargeTime);
            float currentForce = Mathf.Lerp(minThrowForce, maxThrowForce, heldTime / maxChargeTime);

            Vector2 look = inputHandler.LookInput;
            if (look.magnitude > 0.1f)
                lastValidAimDirection = look.normalized;

            ShowTrajectory(lastValidAimDirection, currentForce);
        }
    }

    protected override void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    protected override void Attack()
    {
        // Attack is handled by OnAttackPress/Release
    }

    public void OnAttackPress()
    {
        if (Time.time - lastThrowTime < throwCooldown)
            return;

        isCharging = true;
        chargeStartTime = Time.time;
        trajectoryRenderer.enabled = true;

        Vector2 look = inputHandler.LookInput;
        if (look.magnitude > 0.1f)
        {
            lastValidAimDirection = look.normalized;
        }
    }

    public void OnAttackRelease()
    {
        if (!isCharging)
            return;

        isCharging = false;
        trajectoryRenderer.enabled = false;

        float heldTime = Mathf.Clamp(Time.time - chargeStartTime, 0f, maxChargeTime);
        float currentForce = Mathf.Lerp(minThrowForce, maxThrowForce, heldTime / maxChargeTime);

        ThrowBanana(currentForce);
    }

    private void ThrowBanana(float force)
    {
        lastThrowTime = Time.time;

        GameObject banana = Instantiate(bananaPrefab, throwPoint.position, Quaternion.identity);
        banana.GetComponent<Rigidbody2D>().linearVelocity = lastValidAimDirection * force;
    }

    private void ShowTrajectory(Vector2 direction, float customForce)
    {
        if (bananaPrefab == null || trajectoryRenderer == null || throwPoint == null)
            return;

        trajectoryRenderer.useWorldSpace = true;
        trajectoryRenderer.positionCount = trajectoryPoints;

        Vector3[] points = new Vector3[trajectoryPoints];
        Vector3 startPos = throwPoint.position;

        float gravityScale = bananaPrefab.GetComponent<Rigidbody2D>()?.gravityScale ?? 1f;
        Vector2 gravity = Physics2D.gravity * gravityScale;
        Vector2 velocity = direction.normalized * customForce;

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


