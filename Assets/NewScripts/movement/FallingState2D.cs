using UnityEngine;

/// <summary>
/// Stan spadania postaci.
/// </summary>
public class FallingState2D : AirborneState2D
{
    public FallingState2D(PlayerMovementController2D controller) : base(controller) { }

    public override void Tick()
    {
        base.Tick();
        if (controller.IsGrounded())
        {
            controller.ChangeState(new GroundedState2D(controller));
        }
    }

    public override void FixedTick()
    {
        base.FixedTick();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entering Falling State");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exiting Falling State");
    }
}
