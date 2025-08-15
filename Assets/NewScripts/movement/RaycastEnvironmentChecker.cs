using UnityEngine;

/// <summary>
/// Wykrywa ziemiê i œciany po bokach bez u¿ycia MonoBehaviour.
/// </summary>
public class RaycastEnvironmentChecker2D
{
    private readonly float checkDistance;
    private readonly LayerMask groundLayer;
    private readonly LayerMask climbableLayer;
    private readonly Transform targetTransform;
    private readonly float wallRayHeightOffset;
    private readonly Transform groundCheck;
    public RaycastEnvironmentChecker2D(Transform targetTransform, Transform groundCheck, float checkDistance, float wallRayHeightOffset, LayerMask groundLayer, LayerMask climbableLayer)
    {
        this.targetTransform = targetTransform;
        this.checkDistance = checkDistance;
        this.wallRayHeightOffset = wallRayHeightOffset;
        this.groundLayer = groundLayer;
        this.climbableLayer = climbableLayer;
        this.groundCheck = groundCheck;
    }

    public bool IsGrounded()
    {
        Vector2 origin = groundCheck.position;
        return Physics2D.Raycast(origin, Vector2.down, checkDistance, groundLayer);
    }

    public bool IsTouchingWallLeft()
    {
        Vector2 origin = (Vector2)targetTransform.position + Vector2.up * wallRayHeightOffset;
        return Physics2D.Raycast(origin, Vector2.left, checkDistance, climbableLayer);
    }

    public bool IsTouchingWallRight()
    {
        Vector2 origin = (Vector2)targetTransform.position + Vector2.up * wallRayHeightOffset;
        return Physics2D.Raycast(origin, Vector2.right, checkDistance, climbableLayer);
    }
}
