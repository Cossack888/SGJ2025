using UnityEngine;
using UnityEngine.InputSystem;

public class GroundedState2D : MovementState2D
{
    private float coyoteTime = 0.15f;
    private float coyoteTimer;
    public GroundedState2D(PlayerMovementController2D controller) : base(controller) { }

    public override void Enter()
    {
        coyoteTimer = coyoteTime;

        Debug.Log("Grounded State");
        BindStarted(InputActionType.Player_Jump, OnJump);
    }

    public override void Exit()
    {
        UnbindStarted(InputActionType.Player_Jump, OnJump);
    }

    public override void Tick()
    {
        if (!controller.IsGrounded())
        {
            coyoteTimer -= Time.deltaTime;
            if (coyoteTimer <= 0f)
            {
                controller.ChangeState(new FallingState2D(controller));
            }
        }
        else
        {
            coyoteTimer = coyoteTime;
        }
    }

    public override void FixedTick()
    {
        MoveX(MoveInput.x, controller.MoveSpeed);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        Debug.Log("Jump");
        if (controller.IsGrounded() || coyoteTimer > 0f)
        {
            controller.ChangeState(new JumpingState2D(controller));
        }
    }
}
