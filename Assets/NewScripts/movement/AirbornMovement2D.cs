using UnityEngine;

/// <summary>
/// Abstrakcyjna klasa bazowa dla wszystkich stanów powietrznych (airborne)
/// </summary>
public abstract class AirborneState2D : MovementState2D
{
    protected AirborneState2D(PlayerMovementController2D controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("Airborne Enter");
    }

    public override void Exit()
    {
        Debug.Log("Airborne Exit");
    }

    public override void FixedTick()
    {
        ApplyHorizontalMovement();
    }

    protected virtual void ApplyHorizontalMovement()
    {
        Vector2 currentVelocity = controller.Rigidbody.linearVelocity;
        currentVelocity.x = MoveInput.x * controller.MoveSpeed;
        controller.Rigidbody.linearVelocity = currentVelocity;
    }
}
