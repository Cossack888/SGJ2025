using UnityEngine;

/// <summary>
/// Stan odpowiadaj¹cy za inicjacjê i fazê wznoszenia skoku.
/// Skok jest wykonywany od razu przy wejœciu w stan.
/// </summary>
public class JumpingState2D : AirborneState2D
{
    private readonly float fallThreshold = -0.1f;

    public JumpingState2D(PlayerMovementController2D controller) : base(controller) { }

    public override void Enter()
    {
        base.Enter();
        PerformJump();
    }

    public override void Tick()
    {
        if (controller.Rigidbody.linearVelocity.y < fallThreshold)
        {
            controller.ChangeState(new FallingState2D(controller));
        }
        if (controller.IsGrounded())
        {
            controller.ChangeState(new GroundedState2D(controller));
        }
    }

    private void PerformJump()
    {
        Vector2 velocity = controller.Rigidbody.linearVelocity;
        velocity.y = 0f;
        controller.Rigidbody.linearVelocity = velocity;
        controller.Rigidbody.AddForce(Vector2.up * controller.JumpForce, ForceMode2D.Impulse);
    }
}
